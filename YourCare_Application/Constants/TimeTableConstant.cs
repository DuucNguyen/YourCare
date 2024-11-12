using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YourCare_Application.Constants
{
    public class TimeSlotConstant
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
    public class TimeTableConstant
    {
        public static readonly List<TimeSlotConstant> MorningTimeTable = new List<TimeSlotConstant>
        {
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("9:00"), EndTime = TimeOnly.Parse("9:30") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("9:30"), EndTime = TimeOnly.Parse("10:00") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("10:00"), EndTime = TimeOnly.Parse("10:30") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("10:30"), EndTime = TimeOnly.Parse("11:00") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("11:00"), EndTime = TimeOnly.Parse("11:30") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("11:30"), EndTime = TimeOnly.Parse("12:00") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("12:00"), EndTime = TimeOnly.Parse("12:30") }
        };


        public static readonly List<TimeSlotConstant> AfternoonTimeTable = new List<TimeSlotConstant>
        {
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("14:00"), EndTime = TimeOnly.Parse("14:30") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("14:30"), EndTime = TimeOnly.Parse("15:00") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("15:00"), EndTime = TimeOnly.Parse("15:30") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("15:30"), EndTime = TimeOnly.Parse("16:00") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("16:00"), EndTime = TimeOnly.Parse("16:30") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("16:30"), EndTime = TimeOnly.Parse("17:00") },
            new TimeSlotConstant{ StartTime = TimeOnly.Parse("17:00"), EndTime = TimeOnly.Parse("17:30") }
        };

    }
}
