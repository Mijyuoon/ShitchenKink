using Discord.Commands;

using ShitchenKink.Commands.Services;

namespace ShitchenKink.Commands.Modules;

[Group("spin")]
public class SpinModule : ModuleBase<SocketCommandContext>
{
    private readonly SpinnerService _spinner;

    public SpinModule(SpinnerService spinner)
    {
        _spinner = spinner;
    }

    [Command]
    public async Task DefaultAsync()
    {
        if (_spinner.CanSpin(Context.User))
        {
            _ = Task.Run(async () =>
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