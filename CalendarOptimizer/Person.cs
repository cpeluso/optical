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


        //This method assumes that there are only meetings separated in time
        public void ComputeFreeIntervals()
        {
            IncorporateMeetings();
            int numberOfMeetings = Meetings.Count;
            int i;

            FreeIntervals.Add(new TimeSlot(ReperibleFrom, Meetings[0].StartTime));

            for ( i = 0; i < numberOfMeetings - 1; i++ ) FreeIntervals.Add(new TimeSlot(Meetings[i].EndTime, Meetings[i + 1].StartTime));

            FreeIntervals.Add(new TimeSlot(Meetings[i].EndTime, ReperibleTo));

        }

        private void IncorporateMeetings()
        {
            List<TimeSlot> incorporatedMeetings = new List<TimeSlot>();
            TimeSlot incorporatedMeeting;

            for (int i = 0; i < Meetings.Count; i++)
            {
                for (int j = i; j < Meetings.Count; j++)
                {
                    TimeSlot timeSlot = Meetings[i];
                    TimeSlot innerTimeSlot = Meetings[j];

                    incorporatedMeeting = TryToIncorporateMeetings(timeSlot, innerTimeSlot);

                    if (incorporatedMeeting != null)
                    {
                        Meetings[i] = incorporatedMeeting;
                        Meetings.Remove(Meetings[j]);
                    }

                }
            }

        }

        private TimeSlot TryToIncorporateMeetings(TimeSlot fstTimeSlot, TimeSlot sndTimeSlot)
        {
            if ( fstTimeSlot.StartTime < sndTimeSlot.StartTime)
            {
                if (fstTimeSlot.EndTime > sndTimeSlot.StartTime && fstTimeSlot.EndTime < sndTimeSlot.EndTime)
                {
                    return new TimeSlot(fstTimeSlot.StartTime, sndTimeSlot.EndTime);
                }
            }

            if ( fstTimeSlot.EndTime > sndTimeSlot.EndTime)
            {
                if (fstTimeSlot.StartTime > sndTimeSlot.StartTime && fstTimeSlot.StartTime < sndTimeSlot.EndTime)
                {
                    return new TimeSlot(sndTimeSlot.StartTime, fstTimeSlot.EndTime);
                }
            }

            if (fstTimeSlot.StartTime < sndTimeSlot.StartTime && fstTimeSlot.EndTime > sndTimeSlot.EndTime)
            {
                return new TimeSlot(fstTimeSlot.StartTime, fstTimeSlot.EndTime);
            }

            return null;
        }

    }
}
