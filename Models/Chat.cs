using System.Text.Json.Serialization;

namespace Attendance.Models;

public class Chat
{
    public long Id { get; set; }
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Type { get; set; }
}