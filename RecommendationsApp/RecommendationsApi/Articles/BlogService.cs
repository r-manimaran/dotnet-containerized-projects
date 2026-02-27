namespace RecommendationsApi.Articles;

using HtmlAgilityPack;

public class BlogService(HttpClient httpClient) { 
    public async Task<(string Title, string Content)> GetTitleAndContentAsync(string url)
    {
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var htmlContent = await response.Content.ReadAsStringAsync();
        return ExtractArticle(htmlContent);
    }

    private static (string Title, string Content) ExtractArticle(string htmlContent)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var articleNode = htmlDoc.DocumentNode.SelectSingleNode("//article");
        var title = htmlDoc.DocumentNode.SelectSingleNode("//title");

        var sponsorshipDev = articleNode.SelectSingleNode(".//div[contains(@class,'sponsorship-content')]");
        if(sponsorshipDev is not null)
        {
            sponsorshipDev.Remove();
        }

        return (title.InnerHtml.Trim(), articleNode.InnerText.Trim());
    }
}
