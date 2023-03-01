using Discord;
using Discord.Commands;

using ShitchenKink.Commands.Services;
using ShitchenKink.Core.Services;

namespace ShitchenKink.Commands.Modules;

[Group("spin")]
[Alias("spim")]
public class SpinModule : ModuleBase<SocketCommandContext>
{
    private readonly DispatchService _dispatch;
    private readonly SpinnerService _spinner;

    public SpinModule(DispatchService dispatch, SpinnerService spinner)
    {
        _dispatch = dispatch;
        _spinner = spinner;
    }

    [Command]
    public async Task DefaultAsync([Remainder] string? _ = null)
    {
        if (_spinner.CanSpin(Context.User))
        {
            _dispatch.RunOnce(async () =>
            {
                _spinner.StartSpin(Context.User);
                await ReplyAsync("߷ Spinning Fidget Spinner…");

                var delay = _spinner.GetSpinTime();
                await Task.Delay(TimeSpan.FromSeconds(delay));

                _spinner.EndSpin(Context.User);
                await ReplyAsync($"߷ {Context.User.Mention}, your spinner spun for {delay} seconds! ߷");
            });
        }
        else
        {
            await ReplyAsync("You are too poor to afford a second spinner.");
        }
    }
}