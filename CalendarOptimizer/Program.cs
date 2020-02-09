﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CalendarOptimizer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var (fstPerson, sndPerson) = DefinePeople();
            //ReadFile("/Users/carlopeluso/Desktop/file.txt");
            ComputeFreeInterval(fstPerson);



                    
                



        }

        static void ComputeFreeInterval(Person person)
        {
            var numberOfMeetings = person.Meetings.Count;
            if (person.FreeIntervals.Count == 0) //Currently no free intervals retrieved
            {
                TimeSlot timeSlot = new TimeSlot();
                timeSlot.StartTime = person.ReperibleFrom;
            }
        }

        static (Person, Person) DefinePeople()
        {
            Person fst = new Person();
            Person snd = new Person();

            fst.ReperibleFrom = 540;
            fst.ReperibleTo = 1200;
            fst.Meetings.Add(new TimeSlot ( 600, 660 ));
            fst.Meetings.Add(new TimeSlot ( 750, 870 ));
            fst.Meetings.Add(new TimeSlot ( 900, 930 ));
            fst.Meetings.Add(new TimeSlot ( 1080, 1140 ));

            snd.ReperibleFrom = 540;
            snd.ReperibleTo = 1200;
            snd.Meetings.Add(new TimeSlot ( 540, 630 ));
            snd.Meetings.Add(new TimeSlot ( 720, 750 ));
            snd.Meetings.Add(new TimeSlot ( 780, 930 ));
            snd.Meetings.Add(new TimeSlot ( 960, 1020 ));
            snd.Meetings.Add(new TimeSlot ( 1080, 1110 ));

            return (fst, snd);
        }



    }
}