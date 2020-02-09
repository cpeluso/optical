using System;
using System.Collections.Generic;

namespace CalendarOptimizer
{
    public class Person
    {
        public Person()
        {
        }

        public List<TimeSlot> Meetings { get; set; }
        public List<TimeSlot> FreeIntervals { get; set; }
        public int ReperibleFrom { get; set; }
        public int ReperibleTo { get; set; }

    }
}
