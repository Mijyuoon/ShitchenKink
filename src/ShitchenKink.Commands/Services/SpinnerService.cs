using ConcurrentCollections;

using Discord;

using ShitchenKink.Commands.Data;

namespace ShitchenKink.Commands.Services;

public class SpinnerService
{
    private readonly SpinnerConfig _spinnerConfig;

    private readonly ConcurrentHashSet<ulong> _currentSpinners = new();

    public SpinnerService(SpinnerConfig spinnerConfig)
    {
        _spinnerConfig = spinnerConfig;
    }

    public bool CanSpin(IUser user) => !_currentSpinners.Contains(user.Id);

    public void StartSpin(IUser user) => _currentSpinners.Add(user.Id);

    public void EndSpin(IUser user) => _currentSpinners.TryRemove(user.Id);

    public int GetSpinTime() => Random.Shared.Next(_spinnerConfig.MinimumTime, _spinnerConfig.MaximumTime + 1);
}