using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Contracts;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public class OfficeHourService
    {
        public OfficeHour CreateOfficeHour(OfficeHourCreateRequest request)
        {
            var db = new TimeTableDbContext();

            var lecturer = db.Members.SingleOrDefault(l => l.ShortName.Equals(request.DozId));
            var organizer = db.Organisers.SingleOrDefault(o => o.ShortName.Equals(request.OrgId));
            if (lecturer == null || organizer == null)
                return null;

            var semester = db.Semesters.SingleOrDefault(s => (s.Name.ToUpper().Equals(request.SemesterId.ToUpper())));

            var officeHour = db.Activities.OfType<OfficeHour>()
                .SingleOrDefault(oh => oh.Semester.Id == semester.Id &&
                    oh.Dates.Any(oc => oc.Hosts.Any(l => l.Id == lecturer.Id)));

            if (officeHour == null)
            {
                officeHour = new OfficeHour
                {
                    Name = "Sprechstunde",
                    ShortName = lecturer.ShortName,
                    ByAgreement = request.ByAgreement,
                    Organiser = organizer,
                    Semester = semester,
                    Occurrence = new Occurrence
                    {
                        IsAvailable = true,
                        Capacity = request.SlotDuration > 0 ? -1 : request.Capacity,
                        FromIsRestricted = false,
                        UntilIsRestricted = false,
                        IsCanceled = false,
                        IsMoved = false,
                    }
                };
                db.Activities.Add(officeHour);
                db.SaveChanges();
            }

            if (request.CreateDates)
            {
                var semesterService = new SemesterService();
                var dates = semesterService.GetDays(request.SemesterId, request.DayOfWeek);

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
                                    ? new TimeSpan(request.SubscriptionLimit-1, 59, 0)
                                    : new TimeSpan?(),
                            IsCanceled = false,
                            IsMoved = false,
                        }

                    };

                    // Slots
                    if (request.SlotDuration > 0)
                    {
                        var ohDuration = request.EndTime - request.StartTime;

                        var numSlots = (int) (ohDuration.TotalMinutes/request.SlotDuration + 0.01);


                        for (int i = 1; i <= numSlots; i++)
                        {
                            var slotStart = ocStart.AddMinutes((i - 1)*request.SlotDuration);
                            var slotEnd = ocStart.AddMinutes(i*request.SlotDuration);

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

        public OfficeHour GetOfficeHour(string dozId, string semesterName)
        {
            var db = new TimeTableDbContext();
            var semester = db.Semesters.SingleOrDefault(s => s.Name.ToUpper().Equals(semesterName));
            return db.Activities.OfType<OfficeHour>().SingleOrDefault(a =>
                                (a.Semester == null || a.Semester.Id == semester.Id) &&
                                a.Dates.Any(d => d.Hosts.Any(m => m.ShortName.Equals(dozId))));            
        }
    }
}
