using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;

namespace MyStik.TimeTable.Web.Services
{
    public class OfficeHourService : BaseService
    {
        public OfficeHourService(TimeTableDbContext db) : base(db)
        {
        }

        public OfficeHour GetOfficeHour(OrganiserMember lecturer, Semester sem)
        {
            // zuerst richtg
            var officeHour =
                Db.Activities.OfType<OfficeHour>().FirstOrDefault(x =>
                    x.Semester.Id == sem.Id &&
                    x.Owners.Any(k => k.Member.Id == lecturer.Id)
                );

            return officeHour;
        }

        /// <summary>
        /// Ermittelt die aktuellste Sprechstunde
        /// </summary>
        /// <param name="lecturer"></param>
        /// <returns></returns>
        public OfficeHour GetLatestOfficeHour(OrganiserMember lecturer)
        {
            var officeHour =
                Db.Activities.OfType<OfficeHour>().Where(x =>
                    x.Owners.Any(k => k.Member.Id == lecturer.Id)).OrderByDescending(x => x.Semester.StartCourses).FirstOrDefault();

            return officeHour;
        }

        public OfficeHour CreateOfficeHour(OfficeHourCreateRequest request)
        {
            var db = new TimeTableDbContext();

            var organizer = db.Organisers.SingleOrDefault(o => o.Id == request.OrgId);
            if (organizer == null)
                return null;

            var lecturer = organizer.Members.SingleOrDefault(l => l.Id == request.DozId);
            if (lecturer == null)
                return null;

            var semester = db.Semesters.SingleOrDefault(s => s.Id == request.SemesterId);

            OfficeHour officeHour = null;
                /*
                db.Activities.OfType<OfficeHour>()
                .SingleOrDefault(oh => oh.Semester.Id == semester.Id &&
                    oh.Dates.Any(oc => oc.Hosts.Any(l => l.Id == lecturer.Id)));
                    */

            if (officeHour == null)
            {
                officeHour = new OfficeHour
                {
                    Name = request.Name,
                    ShortName = lecturer.ShortName,
                    ByAgreement = request.ByAgreement,
                    Description = request.Text,
                    Organiser = organizer,
                    Semester = semester,
                    SlotsPerDate = request.SlotsPerDate,
                    FutureSubscriptions = request.FutureSlots,
                    Occurrence = new Occurrence
                    {
                        IsAvailable = true,
                        Capacity = request.SlotDuration > 0 ? -1 : request.Capacity,
                        FromIsRestricted = false,
                        UntilIsRestricted = (request.SubscriptionLimit > 0),
                        UntilTimeSpan =
                            (request.SubscriptionLimit > 0)
                                ? new TimeSpan(request.SubscriptionLimit - 1, 59, 0)
                                : new TimeSpan?(),
                        IsCanceled = false,
                        IsMoved = false,
                    }
                };

                // den Anbieter als Owner eintragen, aber nur wenn er noch nicht drin ist
                if (officeHour.Owners.All(x => x.Member.Id != lecturer.Id))
                {
                    ActivityOwner owner = new ActivityOwner
                    {
                        Activity = officeHour,
                        Member = lecturer,
                        IsLocked = false
                    };

                    officeHour.Owners.Add(owner);
                    db.ActivityOwners.Add(owner);
                }


                db.Activities.Add(officeHour);
                db.SaveChanges();
            }

            if (request.CreateDates)
            {
                var semesterService = new SemesterService();
                var dates = semesterService.GetDays(request.SemesterId, request.DayOfWeek, request.FirstDate, request.LastDate);

                foreach (var dateTime in dates)
                {
                    var ocStart = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, request.StartTime.Hours,
                        request.StartTime.Minutes, request.StartTime.Seconds);
                    var ocEnd = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, request.EndTime.Hours,
                        request.EndTime.Minutes, request.EndTime.Seconds);

                    var occurrence = new ActivityDate
                    {
                        Begin = ocStart,
                        End = ocEnd,
                        Activity = officeHour,
                        Hosts = new HashSet<OrganiserMember>
                        {
                            lecturer,
                        },
                        Occurrence = new Occurrence
                        {
                            IsAvailable = true,
                            Capacity = request.SlotDuration > 0 ? -1 : request.Capacity,
                            FromIsRestricted = false,
                            UntilIsRestricted = (request.SubscriptionLimit > 0),
                            UntilTimeSpan =
                                (request.SubscriptionLimit > 0)
                                    ? new TimeSpan(request.SubscriptionLimit - 1, 59, 0)
                                    : new TimeSpan?(),
                            IsCanceled = false,
                            IsMoved = false,
                        }

                    };

                    // Slots
                    if (request.SlotDuration > 0)
                    {
                        var ohDuration = request.EndTime - request.StartTime;

                        var numSlots = (int)(ohDuration.TotalMinutes / request.SlotDuration + 0.01);


                        for (int i = 1; i <= numSlots; i++)
                        {
                            var slotStart = ocStart.AddMinutes((i - 1) * request.SlotDuration);
                            var slotEnd = ocStart.AddMinutes(i * request.SlotDuration);

                            // i-ter Slot
                            var available = true;
                            if (request.SpareSlots == -99)
                            {
                                available = (i > 1 && i < numSlots);
                            }
                            else
                            {
                                if (request.SpareSlots < 0) // Anzahl vom Ende her
                                {
                                    available = (i <= numSlots + request.SpareSlots);
                                }
                                else if (request.SpareSlots > 0) // Anzahl vom Anfang
                                {
                                    available = (i > request.SpareSlots);
                                }
                            }


                            var slot = new ActivitySlot
                            {
                                Begin = slotStart,
                                End = slotEnd,
                                Occurrence = new Occurrence
                                {
                                    IsAvailable = available,
                                    Capacity = request.Capacity,
                                    FromIsRestricted = false, // Zeitrestriktionen nur auf dem Activity Date
                                    UntilIsRestricted = false,
                                    IsCanceled = false,
                                    IsMoved = false,
                                }
                            };

                            occurrence.Slots.Add(slot);
                        }
                    }

                    officeHour.Dates.Add(occurrence);
                }

                db.SaveChanges();
            }

            return officeHour;
        }


    }
}