using Discord;
using Discord.Commands;

using JetBrains.Annotations;

using ShitchenKink.Commands.Services;
using ShitchenKink.Core.Extensions;

namespace ShitchenKink.Commands.Modules;

[Group("ifunny")]
[UsedImplicitly]
public class IfunnyModule : ModuleBase<SocketCommandContext>
{
    private const int AvatarSize = 512;

    private const string UploadFilename = "ifunny.jpg";
    private const int UploadQuality = 65;

    private readonly IfunnyService _ifunny;

    public IfunnyModule(IfunnyService ifunny)
    {
        _ifunny = ifunny;
    }

    [Command]
    [UsedImplicitly]
    public async Task LinkedAsync(string message)
    {
        try
        {
            await CreateAndSendAsync(message);
        }
        catch (UriFormatException)
        {
            // Ignore if message is not a URL
        }
    }

    [Command]
    [UsedImplicitly]
    public async Task AvatarAsync(IUser user)
    {
        var avatarUrl = user is IGuildUser guildUser
            ? guildUser.GetDisplayAvatarUrl(ImageFormat.Png, AvatarSize)
            : user.GetAvatarUrl(ImageFormat.Png, AvatarSize);

        await CreateAndSendAsync(avatarUrl ?? user.GetDefaultAvatarUrl());
    }

    [Command]
    [UsedImplicitly]
    public async Task MessageAsync()
    {
        // Images in current message take priority
        var thisImage = Context.Message.ExtractImageUrls().FirstOrDefault();
        if (thisImage is not null)
        {
            await CreateAndSendAsync(thisImage);
            return;
        }

        // Check the referenced (reply) message next
        var replyImage = Context.Message.ReferencedMessage?.ExtractImageUrls().FirstOrDefault();
        if (replyImage is not null)
        {
            await CreateAndSendAsync(replyImage);
            return;
        }

        var messages = await Context.Channel
            .GetMessagesAsync(Context.Message, Direction.Before, limit: 100)
            .FlattenAsync();

        // Search for images in previous messages
        foreach (var message in messages)
        {
            var image = message.ExtractImageUrls().FirstOrDefault();
            if (image is null) continue;

            await CreateAndSendAsync(image);
            return;
        }
    }

    private async Task CreateAndSendAsync(string url)
    {
        await using var imageStream = await _ifunny.FromUrlAsync(url, UploadQuality);
        await Context.Channel.SendFileAsync(imageStream, UploadFilename);
    }
}