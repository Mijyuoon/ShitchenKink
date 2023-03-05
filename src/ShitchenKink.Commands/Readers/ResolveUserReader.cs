using System.Globalization;

using Discord;
using Discord.Commands;

namespace ShitchenKink.Commands.Readers;

public class ResolveUserReader<T> : UserTypeReader<T> where T : class, IUser
{
    public override async Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (MentionUtils.TryParseUser(input, out var userId)
            || UInt64.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out userId))
        {
            var user = context.Guild is not null
                    ? await context.Guild.GetUserAsync(userId) as T
                    : await context.Channel.GetUserAsync(userId) as T;

            if (user is not null)
            {
                return TypeReaderResult.FromSuccess(user);
            }
        }

        return await base.ReadAsync(context, input, services);
    }
}