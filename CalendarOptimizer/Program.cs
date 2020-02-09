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

            foreach(Person person in _people)
            {

                //... Do stuff
                //Intersection Time Slot



            }

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