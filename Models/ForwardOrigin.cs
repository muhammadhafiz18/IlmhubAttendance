using System.Text.Json.Serialization;

namespace Attendance.Models;

public class ForwardOrigin
{
    public string Type { get; set; }

    [JsonPropertyName("sender_user")]
    public SenderUser SenderUser { get; set; }
    public long Date { get; set; }
}
