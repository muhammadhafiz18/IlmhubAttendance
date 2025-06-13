using System.Text.Json.Serialization;

namespace Attendance.Models;

public class SenderUser
{
    public long Id { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
    public string? Username { get; set; }
    
    [JsonPropertyName("language_code")]
    public string LanguageCode { get; set; }
}