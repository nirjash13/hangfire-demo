using System.Threading.Tasks;

namespace HangFireCore
{
  public interface IScheduler
  {
    Task CreateScheduledJob();
  }
}
