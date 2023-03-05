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
}