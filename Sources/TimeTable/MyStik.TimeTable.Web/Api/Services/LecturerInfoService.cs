using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class LecturerInfoService
    {

        /// <summary>
        /// Alle zukünftigen Sprechstunden bis Zeitpunkt
        /// </summary>
        /// <param name="until"></param>
        /// <returns></returns>
        public IEnumerable<LecturerOfficeHourContract> GetAllOfficehours(DateTime until)
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



        /// <summary>
        /// alle zukünftigen Sprechstunden eines Profs
        /// </summary>
        /// <param name="lecturerId"></param>
        /// <param name="until"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Alle Dozenten abfragen
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Alle Dozenten einer Fakultät
        /// </summary>
        /// <param name="FacId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Dozenten deren Name/vorname mit x startet einer Fakultät
        /// </summary>
        /// <param name="Startswith"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public IEnumerable<LecturerContract> GetLecturersStartwith (string Startswith, string orgName)
        {
            var db = new TimeTableDbContext();

            //Member sind alle Organisationsmitglieder?
            var lecturers = db.Members.Where(l => l.ShortName.StartsWith(Startswith) ||
                                              l.Name.StartsWith(Startswith) &&
                                              l.Organiser.ShortName.Equals(orgName));

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

        /// <summary>
        /// Alle Vorlesungen eines Dozenten
        /// </summary>
        /// <param name="LecturerId"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Persönlichen Token in der DB hinterlegen
        /// </summary>
        /// <param name="lecturer"></param>
        /// <param name="semester"></param>
        /// <returns></returns>
        public ICollection<AvailableSlotModel> GetAvailabeSlots(OrganiserMember lecturer, Semester semester)
        {
            var db = new TimeTableDbContext();

            var list = new List<AvailableSlotModel>();

            var officeHour =
                db.Activities.OfType<OfficeHour>().SingleOrDefault(a =>
                    a.Semester.Id == semester.Id &&
                    a.Dates.Any(oc => oc.Hosts.Any(l => l.Id == lecturer.Id)));

            if (officeHour == null)
                return list;

            var futureDates = officeHour.Dates.Where(x => x.Begin > DateTime.Now).OrderBy(x => x.Begin).ToList();

            foreach (var date in futureDates)
            {
                if (date.Slots.Any())
                {
                    // bin ich in in einem Slot eingetragen?
                    // ja => dazufügen (Status kommt später)
                    // nein => ist noch ein Platz frei
                    foreach (var slot in date.Slots)
                    {
                        var isSubscribed = false;
                        if (isSubscribed)
                        {
                            var ohSlot = new AvailableSlotModel
                            {
                                Begin = slot.Begin,
                                End = slot.End,
                                OccurrenceId = slot.Occurrence.Id,
                                IsSubscribed = true
                            };
                            list.Add(ohSlot);
                        }
                        else
                        {
                            if (slot.Occurrence.Capacity < 0)
                            {
                                // keine Platzbeschränkung
                                var ohSlot = new AvailableSlotModel
                                {
                                    Begin = slot.Begin,
                                    End = slot.End,
                                    OccurrenceId = slot.Occurrence.Id
                                };
                                list.Add(ohSlot);
                            }
                            else if (slot.Occurrence.Subscriptions.Count < slot.Occurrence.Capacity)
                            {
                                // Platzbeschränkung mit noch freien Plätzen
                                var n = slot.Occurrence.Capacity - slot.Occurrence.Subscriptions.Count;

                                var ohSlot = new AvailableSlotModel
                                {
                                    Begin = slot.Begin,
                                    End = slot.End,
                                    OccurrenceId = slot.Occurrence.Id,
                                    Remark = string.Format("Noch {0} Plätze verfügbar", n)
                                };
                                list.Add(ohSlot);
                            }
                        }
                    }
                }
                else
                {
                    var isSubscribed = false;
                    if (isSubscribed)
                    {
                        var ohSlot = new AvailableSlotModel
                        {
                            Begin = date.Begin,
                            End = date.End,
                            OccurrenceId = date.Occurrence.Id,
                            IsSubscribed = true
                        };
                        list.Add(ohSlot);
                    }
                    else
                    {

                        if (date.Occurrence.Capacity < 0)
                        {
                            var ohSlot = new AvailableSlotModel
                            {
                                Begin = date.Begin,
                                End = date.End,
                                OccurrenceId = date.Occurrence.Id
                            };
                            list.Add(ohSlot);
                        }
                        else if (date.Occurrence.Subscriptions.Count < date.Occurrence.Capacity)
                        {
                            var n = date.Occurrence.Capacity - date.Occurrence.Subscriptions.Count;

                            var ohSlot = new AvailableSlotModel
                            {
                                Begin = date.Begin,
                                End = date.End,
                                OccurrenceId = date.Occurrence.Id,
                                Remark = string.Format("Noch {0} Plätze verfügbar", n)
                            };
                            list.Add(ohSlot);
                        }
                    }
                }
            }



            return list;
        }
    
    }


}