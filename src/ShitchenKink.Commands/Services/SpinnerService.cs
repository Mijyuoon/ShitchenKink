using ConcurrentCollections;

using Discord;

using ShitchenKink.Commands.Data;
using ShitchenKink.Core.Services;

namespace ShitchenKink.Commands.Services;

public class SpinnerService
{
    private readonly SpinnerConfig _config;
    private readonly DispatchService _dispatch;

    private readonly ConcurrentHashSet<ulong> _spinning = new();

    public SpinnerService(SpinnerConfig config, DispatchService dispatch)
    {
        _config = config;
        _dispatch = dispatch;
    }

    public bool CanSpin(IUser user) => !_spinning.Contains(user.Id);

    public bool StartSpin(IUser user, Func<int, Task> doneAsync)
    {
        if (!_spinning.Add(user.Id)) return false;

        var spinTime = GetSpinTime();
        _dispatch.RunOnce(async () =>
        {
            await Task.Delay(spinTime);

            _spinning.TryRemove(user.Id);
            await doneAsync(spinTime.Seconds);
        });

        return true;
    }

    private TimeSpan GetSpinTime()
        => TimeSpan.FromSeconds(Random.Shared.Next(_config.MinimumTime, _config.MaximumTime + 1));
}