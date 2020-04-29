using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using System.Linq;

namespace CalendarOptimizer
{
    public class Program
    {
        private static List<Person> _people = new List<Person>();
        private static List<TimeSlot> _freeCommonIntervals = new List<TimeSlot>();
        private static int _minLength;

        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "OptiCal";
        static UserCredential credential;
        static CalendarService _service;
        static string[] accounts = {"EMAIL1", "EMAIL2"};

        const int AVAILABLE_FROM = 600; //9.00
        const int AVAILABLE_TO = 1080; //18.00
        const int MINIMUM_LENGTH_OF_MEETING = 30; //30 minutes

        static void Main(string[] args)
        {
            LoadCredentials();
            IstantiateGoogleCalendar();

            SendRequests(accounts);
            //ExampleCalendar();

            SearchCommonIntervals();
            PrintOutput();
        }

        static void LoadCredentials()
        {
            using (var stream = new FileStream("INSERT YOUR credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }

        static void IstantiateGoogleCalendar()
        {
            // Create Google Calendar API service.
            _service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        static void SendRequests(string[] accounts)
        {
            accounts.ToList().ForEach(account =>
            {
                Person person = new Person();

                FreeBusyResponse results = LoadRequest(account);

                //Default case
                person.AvailableFrom = AVAILABLE_FROM;
                person.AvailableTo = AVAILABLE_TO;

                LoadMeetings(person, results);
            });

            //Default case
            _minLength = MINIMUM_LENGTH_OF_MEETING;
        }

        static void LoadMeetings(Person person, FreeBusyResponse results)
        {
            foreach (var calendar in results.Calendars)
            {
                calendar.Value.Busy.ToList().ForEach(value =>
                {
                    DateTime start = (DateTime)value.Start;
                    DateTime end = (DateTime)value.End;

                    //Function???
                    int startTime = (start.Hour * 60) + start.Minute;
                    int endTime = (end.Hour * 60) + end.Minute;

                    person.Meetings.Add(new TimeSlot(startTime, endTime));
                });
            }

            if (person.Meetings.Count > 0)
                _people.Add(person);
        }

        static FreeBusyResponse LoadRequest(string account)
        {
            FreeBusyRequestItem calendarRequested = new FreeBusyRequestItem();

            var today = DateTime.Now.ToString("yyyy-MM-dd");

            FreeBusyRequest requestBody = new FreeBusyRequest{
                TimeMin = Convert.ToDateTime(today + "T00:00:00.000Z"),
                TimeMax = Convert.ToDateTime(today + "T23:59:59.000Z"),
                Items = new List<FreeBusyRequestItem>(),
                TimeZone = "Europe/Rome"
            };

            calendarRequested.Id = account;
            requestBody.Items.Add(calendarRequested);

            FreebusyResource.QueryRequest request = _service.Freebusy.Query(requestBody);
            return request.Execute();
        }

        static void ExampleCalendar()
        {
            Person first = new Person();
            Person second = new Person();
            Person third = new Person();
            Person fourth = new Person();

            first.AvailableFrom = 540;
            first.AvailableTo = 1200;
            first.Meetings.Add(new TimeSlot(400, 460));
            first.Meetings.Add(new TimeSlot(500, 610));
            first.Meetings.Add(new TimeSlot(600, 660));
            first.Meetings.Add(new TimeSlot(630, 680));
            first.Meetings.Add(new TimeSlot(750, 870));
            first.Meetings.Add(new TimeSlot(900, 930));
            first.Meetings.Add(new TimeSlot(1080, 1140));

            second.AvailableFrom = 540;
            second.AvailableTo = 1200;
            second.Meetings.Add(new TimeSlot(540, 630));
            second.Meetings.Add(new TimeSlot(720, 750));
            second.Meetings.Add(new TimeSlot(780, 930));
            second.Meetings.Add(new TimeSlot(960, 1020));
            second.Meetings.Add(new TimeSlot(1080, 1110));

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

        static void SearchCommonIntervals()
        {
            int peopleCounter = 0;

            _people.ForEach(person => person.ComputeFreeIntervals());

            Person referencePerson = _people[0];
            _people.Remove(referencePerson);

            _freeCommonIntervals = referencePerson.FreeIntervals;

            bool intersectionsFound = true;

            while (intersectionsFound)
                foreach (Person person in _people)
                {
                    intersectionsFound = ComputeIntersections(person);
                    peopleCounter++;

                    if (peopleCounter == _people.Count)
                        intersectionsFound = false;
                }
        }

        static bool ComputeIntersections(Person person)
        {
            List<TimeSlot> currentCommonIntervals = new List<TimeSlot>();

            foreach(TimeSlot freeInterval in _freeCommonIntervals)
                foreach(TimeSlot personFreeInterval in person.FreeIntervals)
                    if (CanBeIntersected(freeInterval, personFreeInterval))
                        currentCommonIntervals.Add(GetIntersection(freeInterval, personFreeInterval));

            _freeCommonIntervals = currentCommonIntervals;

            if (_freeCommonIntervals.Count <= 0)
                return false;

            return true;
        }

        static bool CanBeIntersected(TimeSlot fstTimeSlot, TimeSlot sndTimeSlot)
        {
            if (fstTimeSlot.EndTime < sndTimeSlot.StartTime || sndTimeSlot.EndTime < fstTimeSlot.StartTime)
                return false;

            if (GetIntersection(fstTimeSlot, sndTimeSlot) == null)
                return false;

            return true;
        }

        static TimeSlot GetIntersection(TimeSlot fstTimeSlot, TimeSlot sndTimeSlot)
        {
            TimeSlot timeSlot = new TimeSlot();

            if (fstTimeSlot.StartTime > sndTimeSlot.StartTime)
                timeSlot.StartTime = fstTimeSlot.StartTime;
            else
                timeSlot.StartTime = sndTimeSlot.StartTime;

            if (fstTimeSlot.EndTime < sndTimeSlot.EndTime)
                timeSlot.EndTime = fstTimeSlot.EndTime;
            else
                timeSlot.EndTime = sndTimeSlot.EndTime;

            timeSlot.Length = timeSlot.EndTime - timeSlot.StartTime;

            if (timeSlot.Length < _minLength)
                return null;

            return timeSlot;
        }

        static void PrintOutput()
        {
            if (_freeCommonIntervals.Count == 0)
                Console.WriteLine("Impossible to schedule a meeting today");

            foreach (TimeSlot timeSlot in _freeCommonIntervals)
                Console.WriteLine(timeSlot.StartTime + " - " + timeSlot.EndTime + ". Time: " + timeSlot.Length);
        }

    }
}