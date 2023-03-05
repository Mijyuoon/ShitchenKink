using System.Collections.Immutable;

using Discord;

namespace ShitchenKink.Core.Extensions;

public static class MessageExtensions
{
    private static readonly ImmutableHashSet<string> ImageMimeTypes
        = ImmutableHashSet.CreateRange(new[]
        {
            "image/png", "image/jpeg", "image/webp", "image/gif",
        });

    public static bool IsImage(this IAttachment attachment)
        => ImageMimeTypes.Contains(attachment.ContentType);

    public static IEnumerable<string> ExtractImageUrls(this IMessage message)
    {
        var attachments = message.Attachments
            .Where(at => at.IsImage())
            .Select(at => at.Url);

        var embeds = message.Embeds
            .Where(em => em.Image is not null || em.Type == EmbedType.Image)
            .Select(em => em.Image?.Url ?? em.Url);

        return attachments.Concat(embeds);
    }
}