using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CourseConverter
    {
        private TimeTableDbContext _db;

        /// <summary>
        /// 
        /// </summary>
        public CourseConverter(TimeTableDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 
        /// </summary>
        public CourseDto Convert(Guid id)
        {
            var course = _db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == id);

            return Convert(course);
        }

        /// <summary>
        /// 
        /// </summary>
        public CourseDto Convert(Course course)
        {
            var dto = new CourseDto();


            dto.Id = course.Id;
            dto.Name = course.Name;
            dto.ShortName = course.ShortName;
            dto.Description = course.Description;
            dto.IsCoterie = course.Occurrence.IsCoterie;
            dto.HasHomeBias = course.Occurrence.HasHomeBias;

            foreach (var semesterGroup in course.SemesterGroups)
            {
                var corr = new CorrelationDto();
                corr.Curriculum = semesterGroup.CapacityGroup.CurriculumGroup.Curriculum.ShortName;
                corr.Organiser = semesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser.ShortName;

                if (dto.Correlations == null)
                {
                    dto.Correlations = new List<CorrelationDto>();
                }

                dto.Correlations.Add(corr);
            }

            foreach (var activityDate in course.Dates)
            {
                var courseDate = ConvertDate(activityDate);

                if (dto.Dates == null)
                {
                    dto.Dates = new List<CourseDateDto>();
                }

                dto.Dates.Add(courseDate);
            }


            return dto;

        }

        internal CourseDateDto ConvertDate(ActivityDate activityDate)
        {
            var courseDate = new CourseDateDto
            {
                Id = activityDate.Id,
                Begin = activityDate.Begin.ToUniversalTime().ToString(@"yyyyMMdd\THHmmss\Z"),
                End = activityDate.End.ToUniversalTime().ToString(@"yyyyMMdd\THHmmss\Z"),
                IsCanceled = activityDate.Occurrence.IsCanceled,
                Title = activityDate.Title
            };


            foreach (var host in activityDate.Hosts)
            {
                var lecturer = new LecturerDto
                {
                    FirstName = host.FirstName,
                    LastName = host.Name,
                    Title = host.Title
                };

                if (!string.IsNullOrEmpty(host.UrlProfile))
                {
                    lecturer.AddAction("Profile", host.UrlProfile);
                }

                if (courseDate.Lecturer == null)
                {
                    courseDate.Lecturer = new List<LecturerDto>();
                }

                courseDate.Lecturer.Add(lecturer);
            }

            foreach (var room in activityDate.Rooms)
            {
                var courseRoom = new RoomDto
                {
                    Number = room.Number,
                    Building = room.Number.Substring(0, 1)
                };


                if (courseRoom.Number.StartsWith("K") || courseRoom.Number.StartsWith("L"))
                {
                    courseRoom.Campus = "Pasing";
                }
                else if (courseRoom.Number.StartsWith("F"))
                {
                    courseRoom.Campus = "Karlstrasse";
                }
                else
                {
                    courseRoom.Campus = "Lothstrasse";
                }

                if (courseDate.Rooms == null)
                {
                    courseDate.Rooms = new List<RoomDto>();
                }

                courseDate.Rooms.Add(courseRoom);
            }

            return courseDate;
        }

        internal void ConvertDates(CourseSummaryDto summary, Course course)
        {
            foreach (var activityDate in course.Dates)
            {
                var courseDate = ConvertDate(activityDate);

                if (summary.Dates == null)
                {
                    summary.Dates = new List<CourseDateDto>();
                }

                summary.Dates.Add(courseDate);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public CourseSummaryDto ConvertSummary(Course course)
        {
            var dto = new CourseSummaryDto();

            var courseService = new CourseService(_db);

            var summary = courseService.GetCourseSummary(course);

            dto.Id = course.Id;
            dto.Name = course.Name;
            dto.ShortName = course.ShortName;
            dto.Description = course.Description;
            dto.IsCoterie = course.Occurrence.IsCoterie;
            dto.HasHomeBias = course.Occurrence.HasHomeBias;

            foreach (var host in summary.Lecturers)
            {
                var lecturer = new LecturerDto();
                lecturer.FirstName = host.FirstName;
                lecturer.LastName = host.Name;
                lecturer.Title = host.Title;

                if (!string.IsNullOrEmpty(host.UrlProfile))
                {
                    lecturer.AddAction("Profile", host.UrlProfile);
                }

                if (dto.Lecturer == null)
                {
                    dto.Lecturer = new List<LecturerDto>();
                }

                dto.Lecturer.Add(lecturer);
            }

            foreach (var room in summary.Rooms)
            {
                var courseRoom = new RoomDto();

                courseRoom.Number = room.Number;
                courseRoom.Building = room.Number.Substring(0, 1);

                if (courseRoom.Number.StartsWith("K") || courseRoom.Number.StartsWith("L"))
                {
                    courseRoom.Campus = "Pasing";
                }
                else if (courseRoom.Number.StartsWith("F"))
                {
                    courseRoom.Campus = "Karlstrasse";
                }
                else
                {
                    courseRoom.Campus = "Lothstrasse";
                }

                if (dto.Locations == null)
                {
                    dto.Locations = new List<RoomDto>();
                }

                dto.Locations.Add(courseRoom);

            }

            foreach (var activityDate in summary.Dates)
            {
                var courseDate = new AppointmentDto();

                courseDate.DayOfWeekName = activityDate.DayOfWeek.ToString();
                courseDate.TimeBegin = activityDate.StartTime.ToString();
                courseDate.TimeEnd = activityDate.EndTime.ToString();

                if (dto.Appointments == null)
                {
                    dto.Appointments = new List<AppointmentDto>();
                }

                dto.Appointments.Add(courseDate);
            }


            foreach (var nexus in course.Nexus)
            {
                var module = new ModuleDto();

                var curr = new CurriculumDto();

                curr.Name = nexus.Requirement.Option.Package.Curriculum.Name;
                curr.ShortName = nexus.Requirement.Option.Package.Curriculum.ShortName;

                curr.Organiser = new OrganiserDto();
                curr.Organiser.Name = nexus.Requirement.Option.Package.Curriculum.Organiser.Name;
                curr.Organiser.ShortName = nexus.Requirement.Option.Package.Curriculum.Organiser.ShortName;
                curr.Organiser.Color = nexus.Requirement.Option.Package.Curriculum.Organiser.HtmlColor;

                module.Id = nexus.Requirement.Id;
                module.Curriculum = curr;
                module.Ects = nexus.Requirement.ECTS;
                module.UsCredits = nexus.Requirement.USCredits;
                module.Sws = nexus.Requirement.SWS;


                if (dto.Modules == null)
                {
                    dto.Modules = new List<ModuleDto>();
                }

                dto.Modules.Add(module);
            }



            return dto;

        }

    }
}