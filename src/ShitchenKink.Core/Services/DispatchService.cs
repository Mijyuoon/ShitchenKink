using Microsoft.Extensions.Logging;

namespace ShitchenKink.Core.Services;

public class DispatchService
{
    private readonly ILogger<DispatchService> _logger;

    public DispatchService(ILogger<DispatchService> logger)
    {
        _logger = logger;
    }

    public void RunOnce(Action callback)
    {
        _ = Task.Run(() =>
        {
            try
            {
                callback();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        });
    }

    public void RunOnce(Func<Task> callback)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await callback();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        });
    }

    private void LogException(Exception ex) =>
        _logger.LogError(ex, "An unhandled error occurred in a background task");
}