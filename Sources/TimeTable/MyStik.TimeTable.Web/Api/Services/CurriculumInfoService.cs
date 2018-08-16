using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();

        /// <summary>
        /// Abfrage aller Fakultäten
        /// </summary>
        public IEnumerable<FacultiesContracts> GetAllFaculties()
        {
            //var faculties = Db.Organisers.Where(ao => ao.IsStudent == false).ToList();
            var faculties = Db.Organisers.ToList();
            var facultiesList = new List<FacultiesContracts>();

            foreach(var fac in faculties)
            {
                if(fac!=null)
                {
                    facultiesList.Add(new FacultiesContracts
                    {
                        FacultyId=fac.Id.ToString(),
                        FucultyName=fac.Name,
                        FucultyShortname=fac.ShortName,

                    });
                }
            }
            return facultiesList.OrderBy(f=>f.FucultyName);
        }
        /// <summary>
        /// abfrage aller verfügbaren Studienprogramme
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CurriculumStudyprogramContract> GetAllStudyprograms()
        {

            var programs = Db.Curricula.OrderBy(c => c.Name).ToList();

            var StudyprogramList = new List<CurriculumStudyprogramContract>();

            foreach(var program in programs)
            {
                if(program!=null)
                {
                    StudyprogramList.Add(new CurriculumStudyprogramContract
                    {
                        StudyprogramId=program.Id.ToString(),
                        StudyprogramName=program.Name,
                        StudyprogramShortname= program.ShortName,
                    });
                }
            }
            return StudyprogramList.OrderBy(s => s.StudyprogramShortname);
        }

        /// <summary>
        /// Abfrage aller Studiengruppen eines Studienprograms
        /// </summary>
        public IEnumerable<CurriculumStudygroupsContract> GetAllStudygroups(string StudyprogramId)
        {
            //alle gruppen des Programms
            var groups = Db.CurriculumGroups.Where(cg => cg.Curriculum.Id.ToString().Equals(StudyprogramId)).OrderBy(cg => cg.Name).ToList();
            var StudygroupList = new List<CurriculumStudygroupsContract>();

            foreach (var group in groups)
            {
                if (group != null)
                {
                    StudygroupList.Add(new CurriculumStudygroupsContract
                    {
                        StudygroupId=group.Id.ToString(),
                        StudygroupName=group.Name,
                    });
                }
            }
            return StudygroupList.OrderBy(sg => sg.StudygroupName);
        }

        /// <summary>
        /// Abfrage aller Kurse einer Studiengruppe
        /// </summary>
        /// <param name="StudygroupId"></param>
        /// <returns></returns>
        public IEnumerable<CurriculumCourseContract> GetAllStudygroupCourses(string StudygroupId)
        {
            //Semestergruppe zur dazugehörigem Studiengruppe finden
            var SemesterGroupId = Db.SemesterGroups.Where(sg => sg.CapacityGroup.CurriculumGroup.Id.ToString() == StudygroupId).First();
            //Alle Kurse einer Studiengruppe
            var courses = Db.Activities.OfType<Course>().Where(c => c.SemesterGroups.Any(d => d.Id == SemesterGroupId.Id)).ToList();

            var courselist = new List<CurriculumCourseContract>();

            foreach(var course in courses)
            {
                if(course!=null)
                {
                    courselist.Add(new CurriculumCourseContract
                        {
                            LectureId=course.Id.ToString(),
                            Title=course.Name,
                        });
                }
            }
            return courselist.OrderBy(c=>c.Title);
        }

        /// <summary>
        /// Abfrage aller Termine eines Kurses
        /// </summary>
        /// <param name="LectureId"></param>
        /// <returns></returns>
        public CurriculumDateContract GetCourseDates(string LectureId)
        {
            //Liste aller ActivityDates des Vorlesung
            var DateList = Db.ActivityDates.Where(a => a.Activity.Id.ToString() == LectureId).ToList();

            var DateContract = new CurriculumDateContract();

            //Grundinfos zur Vorlesung
            DateContract.LectureId=Db.Activities.Where(a=> a.Id.ToString()==LectureId).FirstOrDefault().Id.ToString();
            DateContract.LectureName = Db.Activities.Where(a => a.Id.ToString() == LectureId).FirstOrDefault().Name;

            var Dates = new List<CurriculumDate>();

            //Jeden einzelnen Termin anlegen
            foreach(var date in DateList)
            {

                if(date!=null)
                {
                    var RoomList = new List<CurriculumRoom>();
                    var LecturerList = new List<CurriculumLecturer>();

                    //Dozenten
                    foreach(var host in date.Hosts)
                    {
                        LecturerList.Add(new CurriculumLecturer
                            {
                                LecturerId=host.Id.ToString() != null ? host.Id.ToString() : "N.N.",
                                LecturerName = host.Name != null ? host.Name : "N.N.",
                            });
                    }
                    //Räume
                    foreach(var room in date.Rooms)
                    {
                        RoomList.Add(new CurriculumRoom
                            {
                                RoomId = room.Id.ToString() != null ? room.Id.ToString() : "N.N.",
                                RoomNumber = room.Number != null ? room.Number : "N.N.",
                            });
                    }
                    //Date speichern
                    Dates.Add(new CurriculumDate
                    {
                        Rooms=RoomList.Any() ? RoomList : null,
                        Lecturers = LecturerList.Any() ? LecturerList : null,
                        isCanceled= date.Occurrence?.IsCanceled ?? false,
                        Date = date.Begin.Date.ToString("dd.MM.yyyy"),
                        Begin = date.Begin.TimeOfDay.ToString("hh\\:mm"),
                        End=date.End.TimeOfDay.ToString("hh\\:mm"),

                    });
                }
            }

            DateContract.Dates = Dates;

            return DateContract;
        }

    }
}