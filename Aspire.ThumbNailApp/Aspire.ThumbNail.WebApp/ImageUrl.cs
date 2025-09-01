namespace Aspire.ThumbNail.WebApp;

public static class ImageUrl
{
    public static readonly string PathPrefix = "/images";
    public static readonly string RoutePattern = PathPrefix + "/{slug}";

    public static string GetImageUrl(string slug) => $"{PathPrefix}/{slug}";

    public static string GetThumbnailUrl(string slug) => GetImageUrl(slug) + "?thumbnail=true";

    public static string CreateNameSlug(string name)
    {
        return string.Create(name.Length, name, static (slugBuffer, name) =>
        {
            var extension = Path.GetExtension(name);

            for (var i = 0; i < name.Length - extension.Length; i++)
            {
                var c = name[i];
                slugBuffer[i] = char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '-';
            }

            name.AsSpan(^extension.Length..).CopyTo(slugBuffer[^extension.Length..]);
        });
    }

}
