using Discord;
using Discord.Commands;

using JetBrains.Annotations;

using ShitchenKink.Commands.Readers;
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
    public async Task AvatarAsync([OverrideTypeReader(typeof(ResolveUserReader))] IUser user)
    {
        var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, AvatarSize);
        await CreateAndSendAsync(avatarUrl!);
    }

    [Command]
    [UsedImplicitly]
    public async Task MessageAsync()
    {
        // Attachment in current message takes priority
        var thisAttachment = Context.Message.Attachments.FirstOrDefault();
        if (thisAttachment is not null)
        {
            await CreateAndSendAsync(thisAttachment.Url);
            return;
        }

        var messages = await Context.Channel
            .GetMessagesAsync(Context.Message, Direction.Before, limit: 100)
            .FlattenAsync();

        // Search for images in previous messages
        foreach (var message in messages)
        {
            var attachment = message.Attachments.FirstOrDefault(at => at.IsImage());
            var embed = message.Embeds.FirstOrDefault(em =>
                em.Image is not null || em.Type == EmbedType.Image);

            var imageUrl = attachment?.Url ?? embed?.Image?.Url ?? embed?.Url;
            if (imageUrl is null) continue;

            await CreateAndSendAsync(imageUrl);
            return;
        }
    }

    private async Task CreateAndSendAsync(string url)
    {
        await using var imageStream = await _ifunny.FromUrlAsync(url, UploadQuality);
        await Context.Channel.SendFileAsync(imageStream, UploadFilename);
    }
}