using Discord;
using Discord.Commands;

using JetBrains.Annotations;

using ShitchenKink.Core.Extensions;
using ShitchenKink.Core.Services;

namespace ShitchenKink.Commands.Modules;

[Group("customprefix")]
[UsedImplicitly]
public class CustomPrefixModule : ModuleBase<SocketCommandContext>
{
    private const int MaxCustomPrefixes = 5;

    private readonly BotCommandService _botCommand;

    public CustomPrefixModule(BotCommandService botCommand)
    {
        _botCommand = botCommand;
    }

    [Command]
    [Priority(0)]
    [UsedImplicitly]
    public async Task GetAsync()
    {
        var prefixes = _botCommand.GetUserPrefixes(Context.User)
            .Select((prefix, idx) => $"{idx + 1}. {prefix}")
            .JoinString("\n");

        if (String.IsNullOrEmpty(prefixes))
        {
            await ReplyAsync("You do not have any custom prefixes.");
            return;
        }

        await ReplyAsync($"Your custom prefixes: ```\n{prefixes}\n```", allowedMentions: AllowedMentions.None);
    }

    [Command("add")]
    [Priority(1)]
    [UsedImplicitly]
    public async Task AddAsync([Remainder] string prefix)
    {
        var prefixes = _botCommand.GetUserPrefixes(Context.User).ToArray();

        if (prefixes.Length >= MaxCustomPrefixes)
        {
            await ReplyAsync($"You can only have {MaxCustomPrefixes} custom prefixes!");
            return;
        }

        if (prefixes.Contains(prefix))
        {
            await ReplyAsync($"Prefix ``{prefix}`` already exists.", allowedMentions: AllowedMentions.None);
            return;
        }

        await _botCommand.SetUserPrefixesAsync(Context.User, prefixes.Append(prefix));
        await ReplyAsync($"Prefix ``{prefix}`` added.", allowedMentions: AllowedMentions.None);
    }

    [Command("remove")]
    [Priority(1)]
    [UsedImplicitly]
    public async Task RemoveAsync([Remainder] string prefixPart)
    {
        var prefixes = _botCommand.GetUserPrefixes(Context.User).ToList();

        string removedPrefix;

        if (Int32.TryParse(prefixPart, out var prefixNum) && prefixNum > 0 && prefixNum <= prefixes.Count)
        {
            removedPrefix = prefixes[prefixNum - 1];
            prefixes.RemoveAt(prefixNum - 1);
        }
        else
        {
            var prefixIdx = prefixes.FindLastIndex(prefix => prefix.Contains(prefixPart));

            if (prefixIdx < 0)
            {
                await ReplyAsync("Did not find a prefix to remove.");
                return;
            }

            removedPrefix = prefixes[prefixIdx];
            prefixes.RemoveAt(prefixIdx);
        }

        await _botCommand.SetUserPrefixesAsync(Context.User, prefixes);
        await ReplyAsync($"Prefix ``{removedPrefix}`` removed.", allowedMentions: AllowedMentions.None);
    }
}