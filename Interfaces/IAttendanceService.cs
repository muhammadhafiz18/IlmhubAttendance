using Attendance.Models;

namespace Attendance.Interfaces;

public interface IAttendanceService
{
    string MarkAttendance(Student student);
}