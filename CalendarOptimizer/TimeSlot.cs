using System;
namespace CalendarOptimizer
{
    public class TimeSlot
    {
        public TimeSlot() { }

        public TimeSlot(int startTime, int endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
            Length = endTime - startTime;
        }

        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int Length { get; set; }
    }
}
