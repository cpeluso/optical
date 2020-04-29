using System;
using System.Collections.Generic;
using System.Linq;

namespace CalendarOptimizer
{
    public class Person
    {
        public Person()
        {
        }

        public List<TimeSlot> Meetings = new List<TimeSlot>();
        public List<TimeSlot> FreeIntervals = new List<TimeSlot>();
        public int AvailableFrom { get; set; }
        public int AvailableTo { get; set; }

        public void ComputeFreeIntervals()
        {
            PruneMeetingsOutsideAvailabilityHours();
            MergeMeetings();

            int numberOfMeetings = Meetings.Count;
            int i;

            FreeIntervals.Add(new TimeSlot(AvailableFrom, Meetings[0].StartTime));

            for ( i = 0; i < numberOfMeetings - 1; i++ )
                FreeIntervals.Add(new TimeSlot(Meetings[i].EndTime, Meetings[i + 1].StartTime));

            FreeIntervals.Add(new TimeSlot(Meetings[i].EndTime, AvailableTo));
        }

        private void PruneMeetingsOutsideAvailabilityHours()
        {
            Meetings.RemoveAll(
            meeting =>
                (meeting.StartTime < AvailableFrom && meeting.EndTime < AvailableFrom) ||
                (meeting.EndTime > AvailableTo && meeting.StartTime > AvailableTo)
            );

            foreach (TimeSlot meeting in Meetings)
            {
                if (meeting.StartTime < AvailableFrom && meeting.EndTime < AvailableTo)
                    meeting.StartTime = AvailableFrom;

                if (meeting.EndTime > AvailableTo && meeting.StartTime < AvailableTo)
                    meeting.EndTime = AvailableTo;
            }
        }

        // This method assumes that Meetings is sorted with key StartTime
        private void MergeMeetings()
        {
            List<TimeSlot> mergedMeetings = new List<TimeSlot>();
            bool meetingsMerged = true;
            int counter;

            for (int i = 0; i < Meetings.Count; i++)
            {
                counter = 1;
                while (meetingsMerged && i + counter < Meetings.Count)
                {
                    var meetingMerged = TryToMergeMeetings(Meetings[i], Meetings[i + counter]);

                    if (meetingMerged != Meetings[i])
                    {
                        Meetings[i] = meetingMerged;
                        Meetings[i + counter].ToDelete = true;
                    }
                    else
                        meetingsMerged = false;

                    counter++;
                }

                Meetings.RemoveAll(meeting => meeting.ToDelete);
            }
        }

        private TimeSlot TryToMergeMeetings(TimeSlot fstTimeSlot, TimeSlot sndTimeSlot)
        {
            if ( fstTimeSlot.StartTime < sndTimeSlot.StartTime && fstTimeSlot.EndTime > sndTimeSlot.StartTime && fstTimeSlot.EndTime < sndTimeSlot.EndTime)
                return new TimeSlot(fstTimeSlot.StartTime, sndTimeSlot.EndTime);

            if ( fstTimeSlot.EndTime > sndTimeSlot.EndTime && fstTimeSlot.StartTime > sndTimeSlot.StartTime && fstTimeSlot.StartTime < sndTimeSlot.EndTime)
                return new TimeSlot(sndTimeSlot.StartTime, fstTimeSlot.EndTime);

            if (fstTimeSlot.StartTime < sndTimeSlot.StartTime && fstTimeSlot.EndTime > sndTimeSlot.EndTime)
                return new TimeSlot(fstTimeSlot.StartTime, fstTimeSlot.EndTime);

            return fstTimeSlot;
        }

    }
}
