using Discord;
using Discord.Commands;

using JetBrains.Annotations;

using ShitchenKink.Core.Extensions;
using ShitchenKink.Core.Services;

namespace ShitchenKink.Commands.Modules;

[UsedImplicitly]
public class BasicModule : ModuleBase<SocketCommandContext>
{
    private readonly BotCommandService _botCommand;

    public BasicModule(BotCommandService botCommand)
    {
        _botCommand = botCommand;
    }

    [Command("help")]
    [UsedImplicitly]
    public async Task HelpAsync() =>
        await ReplyAsync("Help yourself.");

    [Command("echo")]
    [UsedImplicitly]
    public async Task EchoAsync() =>
        await ReplyAsync("`@echo off`");

    [Command("echo")]
    [UsedImplicitly]
    public async Task EchoAsync([Remainder] string text) =>
        await ReplyAsync($"*some dummy says:*\n{text}", allowedMentions: AllowedMentions.None);

    [Command("prefix")]
    [UsedImplicitly]
    public async Task PrefixAsync()
    {
        var prefixes = _botCommand.DefaultPrefixes.JoinString("\n");
        await ReplyAsync($"Active prefixes: ```\n{prefixes}\n```", allowedMentions: AllowedMentions.None);
    }
}