using Attendance.Interfaces;
using Attendance.Models;

namespace Attendance.Services;

public class AttendanceService : IAttendanceService
{
    private const double EduCenterLatitude = 41.351092246107875;
    private const double EduCenterLongitude = 69.35306481507374;
    private const double MaxDistanceMetres = 50;

    public string MarkAttendance(Student student)
    {
        student.IsPresent = IsWithin50Meters(EduCenterLatitude, EduCenterLongitude, student.LastLatitude, student.LastLongitude);
        if (student.IsPresent)
        {
            // TODO:
            // Google sheet'ga saqlidigan logika bo'lishi kerak
            return $"✅ {student.FullName} you are marked as present";
        }
        else 
        {
            return $"❌ {student.FullName} you are not in edu. center yet, please try again later";
        }
    }

    private static bool IsWithin50Meters(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371.0;

        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distanceKm = EarthRadiusKm * c;
        double distanceMeters = distanceKm * 1000;

        return distanceMeters <= MaxDistanceMetres;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
