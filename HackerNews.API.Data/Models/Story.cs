using System.Text.Json.Serialization;

namespace HackerNews.API.Data.Models
{
    /// <summary>
    /// Model class used for story data
    /// We have skipped lots of fields and kept only
    /// required field as per requirements
    /// </summary>
    public class Story
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
