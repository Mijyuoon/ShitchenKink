using Discord;
using Discord.Commands;

namespace ShitchenKink.Commands.Modules;

public class BasicModule : ModuleBase<SocketCommandContext>
{
    [Command("help")]
    public Task HelpAsync() => ReplyAsync("Help yourself.");

    [Command("echo")]
    public Task EchoAsync([Remainder] string text) =>
        ReplyAsync($"*some dummy says:*\n{text}", allowedMentions: AllowedMentions.None);
}