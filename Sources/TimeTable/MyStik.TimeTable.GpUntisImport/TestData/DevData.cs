using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Contracts;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;

namespace MyStik.TimeTable.GpUntisImport.TestData
{
    public class DevData
    {
        public void InitOfficeHours(string semId)
        {
            var ohService = new OfficeHourService();

            var request = new OfficeHourCreateRequest
            {
                DozId = "HIN",
                DayOfWeek = DayOfWeek.Monday,
                Capacity = 5,
                SemesterId = semId,
                OrgId = "FK 09",
                StartTime = new TimeSpan(17, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                SlotDuration = 0,
                ByAgreement = false,
                CreateDates = true,
            };

            ohService.CreateOfficeHour(request);

            request = new OfficeHourCreateRequest
            {
                DozId = "STU",
                DayOfWeek = DayOfWeek.Thursday,
                Capacity = 1,
                SemesterId = semId,
                OrgId = "FK 09",
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(11, 0, 0),
                SlotDuration = 10,
                SpareSlots = 0,
                ByAgreement = false,
                CreateDates = true,
            };

            ohService.CreateOfficeHour(request);

            request = new OfficeHourCreateRequest
            {
                DozId = "KRA",
                DayOfWeek = DayOfWeek.Tuesday,
                Capacity = 1,
                SpareSlots = -2,                         // 2 am Ende
                SemesterId = semId,
                OrgId = "FK 09",
                StartTime = new TimeSpan(17, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                SlotDuration = 10,
                ByAgreement = false,
                SubscriptionLimit = 24,
                CreateDates = true,
            };

            ohService.CreateOfficeHour(request);


            request = new OfficeHourCreateRequest
            {
                DozId = "PUC",
                DayOfWeek = DayOfWeek.Wednesday,
                Capacity = -1,
                SemesterId = semId,
                OrgId = "FK 09",
                StartTime = new TimeSpan(16, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                SlotDuration = 0,
                ByAgreement = true,
                CreateDates = true,
            };

            ohService.CreateOfficeHour(request);


        }


        public void InitInternationalActivities()
        {
            InitEvent("INT", "EuroMan 2014", "Summer Academy für Bachelor in Plymouth 2014");
            InitEvent("INT", "EuroMaster 2014", "Summer Academy für Master in Plymouth 2014");
        }

        public void InitUnionActivities()
        {
            InitEvent("FS09", "Schools Out XXL", "Sause am Gardasee oder Balaton");
            InitEvent("FS09", "Paintball", "Bringt Farbe ins Studium");
        }

        public void InitUnionNewsletter()
        {
            InitNewsletter("FS09", "Job", "Laufend neue Jobangebote");
            InitNewsletter("FS09", "Fachschaft", "Aktuelle Infos der Studierendenvertreter");
        }

        private void InitEvent(string organiserName, string name, string description)
        {
            var context = new TimeTableDbContext();

            var organiser = context.Organisers.SingleOrDefault(org => org.ShortName.Equals(organiserName));

            if (organiser != null)
            {
                Event ac = new Event
                {
                    Name = name,
                    ShortName = name,
                    Organiser = organiser,
                    Description = description,
                    Occurrence =  CreateDefaultOccurrence(),
                };

                context.Activities.Add(ac);
                context.SaveChanges();

            }
        }

        private void InitNewsletter(string organiserName, string name, string description)
        {
            var context = new TimeTableDbContext();

            var organiser = context.Organisers.SingleOrDefault(org => org.ShortName.Equals(organiserName));

            if (organiser != null)
            {
                Newsletter ac = new Newsletter
                {
                    Name = name,
                    ShortName = name,
                    Organiser = organiser,
                    Description = description,
                    Occurrence = CreateDefaultOccurrence(),
                };

                context.Activities.Add(ac);
                context.SaveChanges();

            }
        }

        private static Occurrence CreateDefaultOccurrence()
        {
            return new Occurrence
            {
                Capacity = -1,
                IsAvailable = true,
                IsCanceled = false,
                IsMoved = false,
                FromIsRestricted = false,
                UntilIsRestricted = false,
            };
        }

    }
}
