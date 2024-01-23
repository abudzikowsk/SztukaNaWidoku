using Hangfire;

namespace SztukaNaWidoku.Jobs;

public static class StartupConfiguration
{
    public static void UseGetExhibitionsDataJob(this WebApplication webApplication)
    {
        RecurringJob.AddOrUpdate<GetExhibitionsDataJob>("getExhibitionsDataJob", s => s.Run(), Cron.Daily(3));
    }

    public static void UseDeleteAllExhibitionsDataJob(this WebApplication webApplication)
    {
        RecurringJob.AddOrUpdate<DeleteAllExhibitionsDataJob>("deleteAllDataJob", d => d.Run(), Cron.Daily(2,55));
    }
}