using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Services
{
    public class LecturerInfoService
    {
        //Alle zukünftigen Sprechstunden bis Zeitpunkt
        public IEnumerable<LecturerOfficeHourContract>GetAllOfficehours(DateTime until)
        {
            var db = new TimeTableDbContext();

            //Alle Sprechstunden
            var officeHours = db.Activities.OfType<OfficeHour>().ToList();

            var officeHourContract = new List<LecturerOfficeHourContract>();

            foreach(var officeHour in officeHours)
            {
                //Nur buchbaren zukünftige Termine bis gewälten Zeitpunkt
                var nextDate = officeHour.Dates.Where(oc => oc.Begin >= GlobalSettings.Now && oc.Begin <= until && oc.Occurrence.IsAvailable).OrderBy(oc => oc.Begin).ToList();

                if(nextDate!=null)
                {
                    foreach(var date in nextDate)
                    {
                        var officehourDate = new LecturerOfficeHourContract();

                        officehourDate.LecturerId = date.Hosts.FirstOrDefault() != null ? date.Hosts.First().Id.ToString() : "N.N.";
                        officehourDate.LecturerName = date.Hosts.FirstOrDefault() != null ? date.Hosts.First().Name : "N.N.";

                        //Steht der prof beim Raum als Owner drin?
                        officehourDate.LecturerRoomId = date.Rooms.Where(r => r.Owner == officehourDate.LecturerName).FirstOrDefault() != null ? date.Hosts.First().Id.ToString() : "N.N.";
                        officehourDate.LecturerRoomNumber = date.Rooms.Where(r => r.Owner == officehourDate.LecturerName).FirstOrDefault() != null ? date.Hosts.First().Name : "N.N.";

                        var officehourSlots = new List<LecturerOfficeHourDateSlot>();

                        foreach(var slot in date.Slots)
                        {
                            officehourSlots.Add(new LecturerOfficeHourDateSlot
                            {
                                OfficeHourSlotId=slot.Id.ToString(),
                                from=slot.Begin,
                                until=slot.End,
                                NumberOfPossibleSubscribers=slot.Occurrence.Capacity,
                                CurrentNumberOfSubscribers=slot.Occurrence.Subscriptions.Count(c=> c.Id.ToString() != null),
                                isBookablefrom=slot.Occurrence.FromDateTime,
                                isBookableuntil=slot.Occurrence.UntilDateTime,
                            });
                        }
                        officehourDate.OfficeHours = officehourSlots;
                        officeHourContract.Add(officehourDate);
                    }
                }
            }

            return officeHourContract.OrderBy(oh => oh.LecturerName);
        }

        //alle zukünftigen Sprechstunden eines Profs
        public LecturerOfficeHourContract GetLecturerOfficehours(string lecturerId, DateTime until)
        {
            var db = new TimeTableDbContext();

            //nur Sprechstunden des Profs
            var officeHours = db.Activities.OfType<OfficeHour>().Where(oh => oh.Organiser.Id.ToString().Equals(lecturerId)).FirstOrDefault();

            var officehoursContract = new LecturerOfficeHourContract();

            if(officeHours!=null)
            {
                var nextDate = officeHours.Dates.Where(oc => oc.Begin >= GlobalSettings.Now && oc.Begin <= until && oc.Occurrence.IsAvailable).OrderBy(oc => oc.Begin).ToList();
                
                foreach (var date in nextDate)
                {
                    var officehourContract = new LecturerOfficeHourContract();
                    
                    officehourContract.LecturerId = date.Hosts.FirstOrDefault() != null ? date.Hosts.First().Id.ToString() : "N.N.";
                    officehourContract.LecturerName = date.Hosts.FirstOrDefault() != null ? date.Hosts.First().Name : "N.N.";

                    //Steht der prof beim Raum als Owner drin?
                    officehourContract.LecturerRoomId = date.Rooms.Where(r => r.Owner == officehourContract.LecturerName).FirstOrDefault() != null ? date.Hosts.First().Id.ToString() : "N.N.";
                    officehourContract.LecturerRoomNumber = date.Rooms.Where(r => r.Owner == officehourContract.LecturerName).FirstOrDefault() != null ? date.Hosts.First().Name : "N.N.";

                    var officehourSlots = new List<LecturerOfficeHourDateSlot>();

                    foreach (var slot in date.Slots)
                    {
                        officehourSlots.Add(new LecturerOfficeHourDateSlot
                        {
                            OfficeHourSlotId = slot.Id.ToString(),
                            from = slot.Begin,
                            until = slot.End,
                            NumberOfPossibleSubscribers = slot.Occurrence.Capacity,
                            CurrentNumberOfSubscribers = slot.Occurrence.Subscriptions.Count(c => c.Id.ToString() != null),
                            isBookablefrom = slot.Occurrence.FromDateTime,
                            isBookableuntil = slot.Occurrence.UntilDateTime,
                        });
                    }
                    officehourContract.OfficeHours = officehourSlots;
                    officehoursContract = officehourContract;
                }
                
            }
            return officehoursContract;
        }

        //Alle Dozenten abfragen
        public IEnumerable<LecturerContract> GetAllLecturers()
        {
            var db = new TimeTableDbContext();
            var lecturers = db.Members.Where(l => l.Organiser!= null);

            var lecturerContract = new List<LecturerContract>();

            foreach (var lecturer in lecturers)
            {
                if (lecturer != null)
                {
                    lecturerContract.Add(new LecturerContract
                    {
                        LecturerId = lecturer.Id.ToString(),
                        LecturerName = lecturer.Name,
                        LecturerShortname = lecturer.ShortName,
                    });
                }
            }
            return lecturerContract.OrderBy(l => l.LecturerName);

        }

        //Alle Dozenten einer Fakultät
        public IEnumerable<LecturerContract> GetFacLecturers (string FacId)
        {
            var db = new TimeTableDbContext();

            //Member 
            var lecturers = db.Members.Where(l => l.Organiser.Id.ToString().Equals(FacId));

            var lecturerContract = new List<LecturerContract>();
            foreach (var lecturer in lecturers)
            {
                if (lecturer != null)
                {
                    lecturerContract.Add(new LecturerContract
                    {
                        LecturerId = lecturer.Id.ToString(),
                        LecturerName = lecturer.Name,
                        LecturerShortname = lecturer.ShortName,
                    });
                }
            }
            return lecturerContract.OrderBy(l => l.LecturerName);
        }
        //TODO FacId
        //Dozenten deren Name/vorname mit x startet einer Fakultät
        public IEnumerable<LecturerContract> GetLecturersStartwith (string Startswith)
        {
            var db = new TimeTableDbContext();

            //Member sind alle Organisationsmitglieder?
            var lecturers = db.Members.Where(l => l.ShortName.StartsWith(Startswith) ||
                                              l.Name.StartsWith(Startswith));//&&
                                              
                                              //l.Organiser.ShortName.Equals("FK 09"));

            var lecturerContract = new List<LecturerContract>();
            foreach(var lecturer in lecturers)
            {
                if(lecturer!=null)
                {
                    lecturerContract.Add(new LecturerContract
                        {
                            LecturerId=lecturer.Id.ToString(),
                            LecturerName=lecturer.Name,
                            LecturerShortname=lecturer.ShortName,
                        });
                }
            }
            return lecturerContract.OrderBy(l=>l.LecturerName);
        }
        //Alle Vorlesungen eines Dozenten
        public LecturerCoursesContract GetLecturerCourses(string LecturerId)
        {
            var db = new TimeTableDbContext();

            var lecturer = db.Members.Where(l => l.Id.ToString().Equals(LecturerId)).FirstOrDefault();

            var lecturerCourseContract = new LecturerCoursesContract();
            
            if(lecturer!=null)
            {
                //Grundinfos
                lecturerCourseContract.LecturerId = LecturerId;
                lecturerCourseContract.LecturerName = lecturer.Name;
                lecturerCourseContract.LecturerShortname = lecturer.ShortName;

                var courseList = new List<LectureCourses>();

                //Abfrage aller Kurse, bei dem der Dozent verwickelt ist
                var courselist = db.Activities.OfType<Course>().Where(c =>
                        c.Dates.Any(oc => oc.Hosts.Any(l => l.Id.ToString() == LecturerId)))
                        .ToList();

                foreach(var course in courselist)
                {
                    courseList.Add(new LectureCourses
                        {
                            LectureId=course.Id.ToString(),
                            Title=course.Name,
                        });
                    
                }
                lecturerCourseContract.LectureCourses = courseList.OrderBy(c=> c.Title);
            }
            
            return lecturerCourseContract;
        }
        //evtl später TODO:
        //Alle kommenden Termine eines Dozenten
    }
}