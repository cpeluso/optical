using System;
using System.Collections.Generic;

namespace CalendarOptimizer
{
    public class Person
    {
        public Person()
        {
        }

        public List<TimeSlot> Meetings = new List<TimeSlot>();
        public List<TimeSlot> FreeIntervals = new List<TimeSlot>();
        public int ReperibleFrom { get; set; }
        public int ReperibleTo { get; set; }

        public void ComputeFreeIntervals()
        {
            var numberOfMeetings = Meetings.Count;
            int i;

            FreeIntervals.Add(new TimeSlot(ReperibleFrom, Meetings[0].StartTime));

            for ( i = 0; i < numberOfMeetings - 1; i++ ) FreeIntervals.Add(new TimeSlot(Meetings[i].EndTime, Meetings[i + 1].StartTime));

            FreeIntervals.Add(new TimeSlot(Meetings[i].EndTime, ReperibleTo));

        }

    }
}
