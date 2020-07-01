using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HangFireCore
{
  public class Scheduler : IScheduler
  {
    #region Fields

    private readonly IConfiguration config;
    private readonly ILogger<Scheduler> logger;

    private readonly string fileName = @"MrMoneyMustache-PurposefulLifeAfterFinancialIndependence";

    #endregion

    #region Constructor

    public Scheduler(
      IConfiguration config,
      ILogger<Scheduler> logger
      )
    {
      this.config = config;
      this.logger = logger;
    }

    #endregion
    
    #region IScheduler 
    
    public async Task CreateScheduledJob()
    {
      logger.LogInformation(string.Format("Starting Scheduled job in background. Time now: {0}", DateTime.UtcNow));
      //Thread.Sleep(1000);
      var rng = new Random();

      // The folder for the roaming current user 
      string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

      // Combine the base folder with your specific folder....
      string specificFolder = Path.Combine(folder, "Scheduler");

      // CreateDirectory will check if folder exists and, if not, create it.
      // If folder exists then CreateDirectory will do nothing.
      
      Directory.CreateDirectory(specificFolder);

      var targetFilename = string.Format("{0}\\{1}-{2}.mp4", specificFolder, fileName, rng.Next(9999));

      var url = this.config.GetSection("MySettings").GetSection("url").Value;

      if (!File.Exists(targetFilename))
      {
        try
        {
          using (var client = new WebClient())
          {
            var uri = new Uri(url);
            client.DownloadFileAsync(uri, targetFilename);
          }
        }
        catch (Exception ex)
        {
          this.logger.LogError(ex.Message, ex);

          throw ex;
        }
      }
      else
      {
        this.logger.LogWarning("File exists with the same name");
      }

      logger.LogInformation(string.Format("Testing Scheduler from Background task runner. Hangfire is awesome. Time now: {0}", DateTime.UtcNow));
    } 

    #endregion
  }
}
