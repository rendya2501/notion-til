using System.Text;


// Notion APIの設定
var notionToken = Environment.GetEnvironmentVariable("NOTION_TOKEN");
var notionDatabaseId = Environment.GetEnvironmentVariable("NOTION_DATABASE_ID");
var notionUrl = $"https://api.notion.com/v1/databases/{notionDatabaseId}/query";

// GitHubの設定
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
var githubRepo = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
var githubApiUrl = $"https://api.github.com/repos/{githubRepo}/contents/";

using (var httpClient = new HttpClient())
{
    // Notion APIにリクエストを送信
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {notionToken}");
    httpClient.DefaultRequestHeaders.Add("Notion-Version", "2021-05-13");
    var notionResponse = await httpClient.PostAsync(notionUrl, new StringContent("{}", Encoding.UTF8, "application/json"));

    if (notionResponse.IsSuccessStatusCode)
    {
        var content = await notionResponse.Content.ReadAsStringAsync();
        // ここでNotionから取得したデータを処理

        // GitHub APIにコンテンツをプッシュ
        httpClient.DefaultRequestHeaders.Add("Authorization", $"token {githubToken}");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHub-Action");

        var githubResponse = await httpClient.PutAsync(githubApiUrl, new StringContent(content, Encoding.UTF8, "application/json"));

        if (!githubResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("GitHubにプッシュする際にエラーが発生しました。");
        }
    }
    else
    {
        Console.WriteLine("Notion APIからの応答が失敗しました。");
    }
}