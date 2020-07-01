using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HangFireCore.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    #region Fields
    
    private static readonly string[] Summaries = new[]
    {
      "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> logger;
    private readonly IBackgroundJobClient backgroundJobClient;

    #endregion

    #region Constructor
    
    public WeatherForecastController(
      ILogger<WeatherForecastController> logger,
      IBackgroundJobClient backgroundJobs)
    {
      this.logger = logger;
      this.backgroundJobClient = backgroundJobs;
    }

    #endregion

    #region APIs
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
      var watch = System.Diagnostics.Stopwatch.StartNew();

      this.logger.LogInformation(string.Format("Entering controller at {0}.", DateTime.UtcNow));

      try
      {
        #region Sending the job on a scheduler. This will continue even after API returns 

        this.backgroundJobClient.Enqueue<IScheduler>(job => job.CreateScheduledJob());

        this.logger.LogInformation("Job has been queued");

        #endregion

        #region Controller APIs own stuffs

        var rng = new Random();
        var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
          Date = DateTime.Now.AddDays(index),
          TemperatureC = rng.Next(-20, 55),
          Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();

        #endregion

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        this.logger.LogInformation(string.Format("Returning from controller. Time taken: {0}. Time now: {1}", elapsedMs, DateTime.UtcNow));

        return Ok(result);
      }
      catch (Exception ex)
      {
        this.logger.LogError("Exception in Get method", ex);
        return StatusCode(500, ex.Message);
      }
    } 

    #endregion
  }
}
