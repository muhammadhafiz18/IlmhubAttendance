using System.Text.Json.Serialization;

namespace Attendance.Models;

public class Update
{
    [JsonPropertyName("update_id")]
    public long UpdateId { get; set; }
    public Message Message { get; set; }
}
