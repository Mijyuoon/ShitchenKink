using ConcurrentCollections;

using Discord;

namespace ShitchenKink.Commands.Services;

public class SpinnerService
{
    private const int MinimumTime = 10;
    private const int MaximumTime = 200;

    private readonly ConcurrentHashSet<ulong> _currentSpinners = new();

    public bool CanSpin(IUser user) => !_currentSpinners.Contains(user.Id);

    public void StartSpin(IUser user) => _currentSpinners.Add(user.Id);

    public void EndSpin(IUser user) => _currentSpinners.TryRemove(user.Id);

    public int GetSpinTime() => Random.Shared.Next(MinimumTime, MaximumTime + 1);
}