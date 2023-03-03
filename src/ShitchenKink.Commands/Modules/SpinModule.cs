using Discord.Commands;

using JetBrains.Annotations;

using ShitchenKink.Commands.Services;

namespace ShitchenKink.Commands.Modules;

[Group("spin")]
[Alias("spim")]
[UsedImplicitly]
public class SpinModule : ModuleBase<SocketCommandContext>
{
    private readonly SpinnerService _spinner;

    public SpinModule(SpinnerService spinner)
    {
        _spinner = spinner;
    }

    [Command]
    [UsedImplicitly]
    public async Task DefaultAsync([Remainder] string? _ = null)
    {
        var spinUser = Context.User;
        var canSpin = _spinner.StartSpin(spinUser, async seconds =>
        {
            await ReplyAsync($"߷ {spinUser.Mention}, your spinner spun for {seconds} seconds! ߷");
        });

        await ReplyAsync(canSpin
            ? "߷ Spinning Fidget Spinner…"
            : "You are too poor to afford a second spinner.");
    }
}