using System.Text.Json.Serialization;

namespace Attendance.Models;

public class Message
{
    [JsonPropertyName("message_id")]
    public int MessageId { get; set; }
    public From From { get; set; }
    public Chat Chat { get; set; }
    public long Date { get; set; }
    public string Text { get; set; }

    [JsonPropertyName("forward_origin")]
    public ForwardOrigin ForwardOrigin { get; set; }

    [JsonPropertyName("forward_date")]
    public long ForwardDate { get; set; }
    public Location Location { get; set; }

}