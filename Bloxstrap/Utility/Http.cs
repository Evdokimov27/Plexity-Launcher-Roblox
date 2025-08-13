using System.Net.Http.Headers;

namespace Plexity.Utility
{
    internal static class Http
    {
        /// <summary>
        /// Gets and deserializes a JSON API response to the specified object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="JsonException"></exception>
        public static async Task<T> GetJson<T>(string url)
        {
			using var client = new HttpClient();

			// Обязательный заголовок для GitHub API
			client.DefaultRequestHeaders.UserAgent.Add(
				new ProductInfoHeaderValue("PlexityApp", "1.0"));
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

			// Если нужен токен (приватный репо или больше лимит)
			var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
			if (!string.IsNullOrWhiteSpace(token))
			{
				client.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", token);
			}

			var resp = await client.GetAsync(url);
			resp.EnsureSuccessStatusCode();

			var json = await resp.Content.ReadAsStringAsync();
			return JsonSerializer.Deserialize<T>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
		}
    }
}
