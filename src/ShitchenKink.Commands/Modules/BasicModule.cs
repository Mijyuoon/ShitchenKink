using Discord;
using Discord.Commands;

using ShitchenKink.Core.Services;

namespace ShitchenKink.Commands.Modules;

public class BasicModule : ModuleBase<SocketCommandContext>
{
    private readonly BotCommandService _botCommand;

    public BasicModule(BotCommandService botCommand)
    {
        _botCommand = botCommand;
    }

    [Command("help")]
    public async Task HelpAsync() =>
        await ReplyAsync("Help yourself.");

    [Command("echo")]
    public async Task EchoAsync([Remainder] string text) =>
        await ReplyAsync($"*some dummy says:*\n{text}", allowedMentions: AllowedMentions.None);

    [Command("prefix")]
    public async Task PrefixAsync()
    {
        var prefixes = String.Join("\n", _botCommand.DefaultPrefixes);
        await ReplyAsync($"```\n{prefixes}\n```", allowedMentions: AllowedMentions.None);
    }
}