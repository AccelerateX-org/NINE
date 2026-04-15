using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.IO.CSV2
{
    internal class DateCone
    {
        internal DateTime Begin { get; set; }
        internal DateTime End { get; set; }
    }

    public class SemesterImport
    {
        private readonly ImportContext _import;

        private readonly TimeTableDbContext _db = new TimeTableDbContext();


        private readonly ILog _Logger = LogManager.GetLogger("Import");

        private readonly StringBuilder _report = new StringBuilder();


        public SemesterImport(ImportContext import)
        {
            _import = import;
        }

        public void ImportCourse(ImportCourseId courseId, List<ImportCourseDataSet> dataSet)
        {
            // Implementation for importing a course
        }

        public void CheckCourse(ImportCourseId courseHeader)
        {
            var organiser = _db.Organisers.FirstOrDefault(x => x.ShortName.Equals(courseHeader.Faculty));
            if (organiser == null)
            {
                courseHeader.IsValid = false;
                courseHeader.Errors.Add($"Organiser with short name '{courseHeader.Faculty}' not found.");
                return;
            }

            courseHeader.OrgId = organiser.Id;

            var semester = _db.Semesters.FirstOrDefault(x => x.Name.Equals(courseHeader.Semester));
            if (semester == null)
            {
                courseHeader.IsValid = false;
                courseHeader.Errors.Add($"Semester with name '{courseHeader.Semester}' not found.");
                return;
            }

            courseHeader.SemId = semester.Id;

            var course = _db.Activities.OfType<Course>().FirstOrDefault(x => x.Organiser.Id == organiser.Id &&
                                                                             x.Semester.Id == semester.Id &&
                                                                             x.ShortName.Equals(courseHeader.Id));

            if (course != null)
            {
                courseHeader.CourseId = course.Id;

                courseHeader.Errors.Add($"Course with name '{courseHeader.Id}' already exists.");
            }
        }

        public void CheckLecturer(ImportCourseId courseHeader, List<ImportItem> dataItemsLecturer)
        {
            var organiser = _db.Organisers
                .Include(activityOrganiser => activityOrganiser.Members)
                .FirstOrDefault(x => x.Id == courseHeader.OrgId.Value);

            foreach (var lecturer in dataItemsLecturer)
            {
                var lec = lecturer.Token.Trim();
                if (lec.Contains(" "))
                {
                    var words = lec.Split(' ');
                    var lastName = words[0];
                    var firstName = words[1];

                    var candidates = organiser.Members.Where(x => x.Name.Equals(lastName)).ToList();
                    if (!candidates.Any())
                    {
                        courseHeader.IsValid = false;
                        courseHeader.Errors.Add($"Lecturer with last name '{lastName}' not found.");
                        return;
                    }

                    var candidate = candidates.Where(x => x.FirstName.Equals(firstName)).ToList();
                    switch (candidate.Count)
                    {
                        case 0:
                            courseHeader.IsValid = false;
                            courseHeader.Errors.Add(
                                $"Lecturer with first name '{firstName}' not found among candidates with last name '{lastName}'.");
                            return;
                        case 1:
                            lecturer.ObjectId = candidate.First().Id;
                            return;
                        default:
                            courseHeader.IsValid = false;
                            courseHeader.Errors.Add(
                                $"Lecturer with first name '{firstName}' not unique among candidates with last name '{lastName}'.");
                            return;
                    }
                }
                else
                {
                    var candidates = organiser.Members.Where(x => x.ShortName.Equals(lec)).ToList();
                    switch (candidates.Count())
                    {
                        case 0:
                            courseHeader.IsValid = false;
                            courseHeader.Errors.Add($"Lecturer with short name '{lec}' not found.");
                            return;
                        case 1:
                            lecturer.ObjectId = candidates.First().Id;
                            return;
                        default:
                            courseHeader.IsValid = false;
                            courseHeader.Errors.Add($"Lecturer with short name '{lec}' not unique.");
                            return;
                    }
                }
            }
        }

        public void CheckRooms(ImportCourseId courseHeader, List<ImportItem> dataItemsRoom)
        {
            foreach (var room in dataItemsRoom)
            {
                var roomNumber = room.Token.Trim();

                var candidates = _db.Rooms.Where(x => x.Number.Equals(roomNumber)).ToList();
                switch (candidates.Count())
                {
                    case 0:
                        courseHeader.IsValid = false;
                        courseHeader.Errors.Add($"Room with number '{roomNumber}' not found.");
                        return;
                    case 1:
                        room.ObjectId = candidates.First().Id;
                        return;
                    default:
                        courseHeader.IsValid = false;
                        courseHeader.Errors.Add($"Room with number '{roomNumber}' not unique.");
                        return;
                }
            }
        }

        public void CheckCohortes(ImportCourseId courseHeader, List<ImportItem> dataItemsCohorte)
        {
            var org = _db.Organisers.Include(activityOrganiser => activityOrganiser.Institution.LabelSet)
                .Include(activityOrganiser1 => activityOrganiser1.LabelSet).SingleOrDefault(x => x.Id == courseHeader.OrgId);
            var inst = org.Institution;


            foreach (var cohorte in dataItemsCohorte)
            {
                var words = cohorte.Token.Split(':');
                if (words.Length != 2)
                {
                    courseHeader.IsValid = false;
                    courseHeader.Errors.Add($"Cohort token '{cohorte.Token}' is not in the expected format 'prefix:suffix'.");
                    return;
                }

                var prefix = words[0].Trim();
                var suffix = words[1].Trim();

                ItemLabelSet labelSet = null;

                if (prefix.Equals(inst.Tag))
                {
                    labelSet = inst.LabelSet;
                }
                else if (prefix.Equals(org.ShortName))
                {
                    labelSet = org.LabelSet;
                }
                else
                {
                    var candidates = org.Curricula.Where(x => x.Alias.Equals(prefix)).ToList();

                }

                if (labelSet == null)
                {
                    courseHeader.IsValid = false;
                    courseHeader.Errors.Add($"Cohort prefix '{prefix}' does not match institution tag '{inst.Tag}' or organiser short name '{org.ShortName}'.");
                    return;
                }


            }
        }
    }
}