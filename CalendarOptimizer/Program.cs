using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CalendarOptimizer
{
    public class Program
    {
        private static List<Person> _people = new List<Person>();
        private static List<TimeSlot> _freeCommonIntervals = new List<TimeSlot>();
        private static int _minLength = 30;

        static void Main(string[] args)
        {
            LoadCalendars();

            foreach ( Person person in _people ) person.ComputeFreeIntervals();

            SearchCommonIntervals();


        }

        static void SearchCommonIntervals()
        {
            Person referencePerson = _people[0];
            _people.Remove(referencePerson);
            _freeCommonIntervals = referencePerson.FreeIntervals;

            foreach(Person person in _people)
                ComputeIntersections(person);


        }

        static void ComputeIntersections(Person sndPerson)
        {
            List<TimeSlot> currentCommonIntervals = new List<TimeSlot>();

            foreach(TimeSlot freeInterval in _freeCommonIntervals)
                foreach(TimeSlot personFreeInterval in sndPerson.FreeIntervals)
                    if (CanBeIntersected(freeInterval, personFreeInterval))
                        currentCommonIntervals.Add(GetIntersection(freeInterval, personFreeInterval));

            _freeCommonIntervals = currentCommonIntervals;

        }

        static bool CanBeIntersected(TimeSlot fstTimeSlot, TimeSlot sndTimeSlot)
        {
            if (fstTimeSlot.EndTime < sndTimeSlot.StartTime || sndTimeSlot.EndTime < fstTimeSlot.StartTime) return false;
            if (GetIntersection(fstTimeSlot, sndTimeSlot) == null) return false;
            return true;
        }

        static TimeSlot GetIntersection(TimeSlot fstTimeSlot, TimeSlot sndTimeSlot)
        {
            TimeSlot timeSlot = new TimeSlot();

            if (fstTimeSlot.StartTime > sndTimeSlot.StartTime) timeSlot.StartTime = fstTimeSlot.StartTime;
            else timeSlot.StartTime = sndTimeSlot.StartTime;

            if (fstTimeSlot.EndTime < sndTimeSlot.EndTime) timeSlot.EndTime = fstTimeSlot.EndTime;
            else timeSlot.EndTime = sndTimeSlot.EndTime;

            timeSlot.Length = timeSlot.EndTime - timeSlot.StartTime;

            if (timeSlot.Length < _minLength) return null;

            return timeSlot;
        }

        //Just for having an input...
        static void LoadCalendars()
        {
            Person fst = new Person();
            Person snd = new Person();

            fst.AvailableFrom = 540;
            fst.AvailableTo = 1200;
            fst.Meetings.Add(new TimeSlot ( 400, 460 )); 
            fst.Meetings.Add(new TimeSlot ( 500, 610 ));
            fst.Meetings.Add(new TimeSlot ( 600, 660 ));
            fst.Meetings.Add(new TimeSlot ( 630, 680 )); 
            fst.Meetings.Add(new TimeSlot ( 750, 870 )); 
            fst.Meetings.Add(new TimeSlot ( 900, 930 ));
            fst.Meetings.Add(new TimeSlot ( 1080, 1140 ));

            snd.AvailableFrom = 540;
            snd.AvailableTo = 1200;
            snd.Meetings.Add(new TimeSlot ( 540, 630 )); 
            snd.Meetings.Add(new TimeSlot ( 720, 750 ));
            snd.Meetings.Add(new TimeSlot ( 780, 930 ));
            snd.Meetings.Add(new TimeSlot ( 960, 1020 ));
            snd.Meetings.Add(new TimeSlot ( 1080, 1110 ));

            _people.Add(fst);
            _people.Add(snd);

        }



    }
}