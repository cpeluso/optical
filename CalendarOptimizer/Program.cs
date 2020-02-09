using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;

namespace CalendarOptimizer
{
    public class Program
    {
        private static List<Person> _people = new List<Person>();
        private static List<TimeSlot> _freeCommonIntervals = new List<TimeSlot>();
        private static int _minLength;

        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "OptiCal";

        static void Main(string[] args)
        {
            //LoadCalendars();

            //foreach ( Person person in _people ) person.ComputeFreeIntervals();

            //SearchCommonIntervals();

            //PrintOutput();

            UserCredential credential;

            using (var stream = new FileStream("/Users/carlopeluso/Projects/optical/CalendarOptimizer/credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            Console.WriteLine("Upcoming events:");
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime.ToString();
                    if (String.IsNullOrEmpty(when))
                    {
                        when = eventItem.Start.Date;
                    }
                    Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                }
            }
            else
            {
                Console.WriteLine("No upcoming events found.");
            }
            Console.Read();

        }

        static void SearchCommonIntervals()
        {
            Person referencePerson = _people[0];
            _people.Remove(referencePerson);
            _freeCommonIntervals = referencePerson.FreeIntervals;

            bool intersectionsFound = true;

            while (intersectionsFound) foreach(Person person in _people) intersectionsFound = ComputeIntersections(person);
        }

        static bool ComputeIntersections(Person sndPerson)
        {
            List<TimeSlot> currentCommonIntervals = new List<TimeSlot>();

            foreach(TimeSlot freeInterval in _freeCommonIntervals)
                foreach(TimeSlot personFreeInterval in sndPerson.FreeIntervals)
                    if (CanBeIntersected(freeInterval, personFreeInterval))
                        currentCommonIntervals.Add(GetIntersection(freeInterval, personFreeInterval));

            _freeCommonIntervals = currentCommonIntervals;

            if (_freeCommonIntervals.Count <= 0) return false;
            return true;

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
            Person first = new Person();
            Person second = new Person();
            Person third = new Person();
            Person fourth = new Person();

            first.AvailableFrom = 540;
            first.AvailableTo = 1200;
            first.Meetings.Add(new TimeSlot ( 400, 460 )); 
            first.Meetings.Add(new TimeSlot ( 500, 610 ));
            first.Meetings.Add(new TimeSlot ( 600, 660 ));
            first.Meetings.Add(new TimeSlot ( 630, 680 )); 
            first.Meetings.Add(new TimeSlot ( 750, 870 )); 
            first.Meetings.Add(new TimeSlot ( 900, 930 ));
            first.Meetings.Add(new TimeSlot ( 1080, 1140 ));

            second.AvailableFrom = 540;
            second.AvailableTo = 1200;
            second.Meetings.Add(new TimeSlot ( 540, 630 )); 
            second.Meetings.Add(new TimeSlot ( 720, 750 ));
            second.Meetings.Add(new TimeSlot ( 780, 930 ));
            second.Meetings.Add(new TimeSlot ( 960, 1020 ));
            second.Meetings.Add(new TimeSlot ( 1080, 1110 ));

            third.AvailableFrom = 540;
            third.AvailableTo = 1200;
            third.Meetings.Add(new TimeSlot(540, 750));
            third.Meetings.Add(new TimeSlot(780, 930));
            third.Meetings.Add(new TimeSlot(960, 1080));
            third.Meetings.Add(new TimeSlot(1080, 1110));

            //fourth.AvailableFrom = 540;
            //fourth.AvailableTo = 550;
            //fourth.Meetings.Add(new TimeSlot(540, 750));
            //fourth.Meetings.Add(new TimeSlot(780, 930));
            //fourth.Meetings.Add(new TimeSlot(960, 1080));
            //fourth.Meetings.Add(new TimeSlot(1080, 1110));

            _people.Add(first);
            _people.Add(second);
            _people.Add(third);
            //_people.Add(fourth);

            _minLength = 30;

        }

        static void PrintOutput()
        {
            if (_freeCommonIntervals.Count == 0) Console.WriteLine("Impossibile schedulare un meeting oggi.");
            foreach (TimeSlot timeSlot in _freeCommonIntervals) Console.WriteLine(timeSlot.StartTime + " - " + timeSlot.EndTime + ". Tempo: " + timeSlot.Length);
        }



    }
}