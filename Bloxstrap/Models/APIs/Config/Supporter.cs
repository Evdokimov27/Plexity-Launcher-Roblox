namespace Plexity.Models.APIs.Config
{
    public class Supporter
    {

		[JsonPropertyName("name")]
		public string Name { get; set; } = null!;

		[JsonPropertyName("link")]
		public string Link { get; set; } = null!;

		[JsonPropertyName("img")]
		public string Image { get; set; } = null!;

		[JsonPropertyName("linkName")]
		public string? LinkText { get; set; } = null!;

	}
}
