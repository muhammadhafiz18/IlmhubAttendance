using System.Text.Json.Serialization;

namespace Attendance.Models;

public class From
{
    public long Id { get; set; }
    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
    public string UserName { get; set; }
    [JsonPropertyName("language_code")]
    public string LanguageCode { get; set; }
}