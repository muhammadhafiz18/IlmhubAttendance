namespace Attendance.Models;

public class Student
{
    public long ChatId { get; set; }
    public string? FullName { get; set; }
    public string UserName { get; set; }
    public double LastLatitude { get; set; }
    public double LastLongitude { get; set; }
    public bool IsPresent { get; set; }
}
