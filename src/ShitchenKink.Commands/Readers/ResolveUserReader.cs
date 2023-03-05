using Discord;
using Discord.Commands;

namespace ShitchenKink.Commands.Readers;

public class ResolveUserReader : TypeReader
{
    public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider _)
    {
        if (!MentionUtils.TryParseUser(input, out var userId))
        {
            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "User not found.");
        }

        var user = await context.Client.GetUserAsync(userId);
        return TypeReaderResult.FromSuccess(user);
    }
}