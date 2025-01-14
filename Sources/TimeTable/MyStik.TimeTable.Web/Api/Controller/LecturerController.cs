using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class LecturerSearchRequest
    {
        public string Organiser { get; set; }

        public string Committee { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Web.Http.RoutePrefix("api/v2/lecturer")]
    public class LecturerController : ApiBaseController
    {
        [System.Web.Http.Route("search")]
        [System.Web.Http.HttpPost]
        public IQueryable<LecturerDto> Search([FromBody] LecturerSearchRequest request)
        {
            var lecturer = Db.Members.Where(x => x.Organiser.ShortName.Equals(request.Organiser)).ToList();

            var response = new List<LecturerDto>();

            foreach (var member in lecturer)
            {
                var dto = new LecturerDto
                {
                    Faculty = member.Organiser.ShortName,
                    FirstName = member.FirstName,
                    LastName = member.Name,
                    ShortName = member.ShortName
                };

                response.Add(dto);
            }

            return response.AsQueryable();
        }



        //Dozenteninfo APIs
        /// <summary>
        /// Abfrage aller Dozenten
        /// </summary>
        /// <returns>Liste aller verfügbaren Dozenten</returns>
        [System.Web.Http.Route("")]
        public IQueryable<LecturerContractExtended> GetAllLecture()
        {
            var userService = new UserInfoService();

            var lecturerUserList = Db.Members.Where(x => x.IsAssociated && !string.IsNullOrEmpty(x.UserId)).Select(x => x.UserId)
                .Distinct().ToList();

            var LecturerList = new List<LecturerContractExtended>();

            var semester = new SemesterService().GetSemester(DateTime.Today);

            foreach (var userId in lecturerUserList)
            {
                var members = Db.Members.Where(x => !string.IsNullOrEmpty(x.UserId) && x.UserId.Equals(userId)).ToList();
                var mainMember = members.FirstOrDefault();

                var lecModel = new LecturerContractExtended();

                lecModel.Title = mainMember.Role;
                lecModel.Room = "";

                // Details zum Benutzerkonto
                var user = userService.GetUser(mainMember.UserId);

                if (user != null)
                {
                    lecModel.FirstName = user.FirstName;
                    lecModel.LastName = user.LastName;
                    lecModel.Email = user.Email;
                }
                else
                {
                    lecModel.LastName = "N. N.";
                }

                // gibt es noch nicht
                lecModel.Functions = new List<string>();

                lecModel.Courses = new List<CourseDto>();
                lecModel.OfficeHours = new List<CourseDto>();
                lecModel.Modules = new List<ModuleDtoVersion2>();
                foreach (var member in members)
                {
                    var courses = Db.Activities.OfType<Course>()
                            .Where(x =>
                                x.Semester != null && x.Semester.Id == semester.Id &&
                                x.Dates.Any(d => d.Hosts.Any(h => h.Id == member.Id)))
                            .ToList();

                    foreach (var course in courses)
                    {
                        var courseDto = new CourseDto
                        {
                            Name = course.Name,
                            ShortName = course.ShortName,
                            Id = course.Id
                        };
                        lecModel.Courses.Add(courseDto);
                    }


                    var officeHours = Db.Activities.OfType<OfficeHour>()
                        .Where(x => x.Owners.Any(o => o.Member.Id == member.Id) && x.Semester.Id == semester.Id).ToList();

                    foreach (var course in officeHours)
                    {
                        var courseDto = new CourseDto
                        {
                            Name = course.Name,
                            ShortName = course.ShortName,
                            Id = course.Id
                        };
                        lecModel.Courses.Add(courseDto);
                    }

                    var modules = Db.CurriculumModules.Where(x => x.ModuleResponsibilities.Any(r => r.Member.Id == member.Id)).ToList();

                    foreach (var module in modules)
                    {
                        var moduleDto = new ModuleDtoVersion2
                        {
                            CatalogId = module.FullTag,
                            Title = module.Name
                        };
                        lecModel.Modules.Add(moduleDto);
                    }

                }

                // Averfügbar Slots
                //lecModel.AvailableSlots = lecturerService.GetAvailabeSlots(lecturer, semester);

                LecturerList.Add(lecModel);
            }

            return LecturerList.AsQueryable();
        }

        /// <summary>
        /// Abfrage aller Dozenten einer Fakultät
        /// </summary>
        /// <param name="FacId">Id des Organisationsveranstalters, z.B. von FK09 oder andere</param>
        /// <returns>Liste aller Dozenten einer Fakultät</returns>
        [System.Web.Http.Route("org")]
        public LecturersResponse GetFacLecturer(string FacId)
        {
            var lecturerService = new LecturerInfoService();

            var lecturerList = lecturerService.GetFacLecturers(FacId);

            var response = new LecturersResponse
            {
                Lecturers = lecturerList,
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller Dozenten, deren Namen mit "StartsWith"- string beginnt
        /// </summary>
        /// <param name="StartsWith">Beliebiger String mit dem der Name anfangen soll</param>
        /// <returns>Liste aller Dozenten die mit dem übergebenen String anfangen</returns>
        [System.Web.Http.Route("search")]
        public LecturersResponse GetLecturersStartswith(string StartsWith)
        {
            var lecturerService = new LecturerInfoService();

            var orgName = "FK 09";

            var lecturerList = lecturerService.GetLecturersStartwith(StartsWith, orgName);

            var response = new LecturersResponse
            {
                Lecturers = lecturerList,
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller Kurse eines Dozenten
        /// </summary>
        /// <param name="LecturerId">Id des Dozenten</param>
        /// <returns>Liste aller Kurse, die ein Dozent hält</returns>
        [System.Web.Http.Route("courses")]

        public LecturerCoursesResponse GetLectureCourses(string LecturerId)
        {
            var lecturerService = new LecturerInfoService();

            var lecturerList = lecturerService.GetLecturerCourses(LecturerId);

            var response = new LecturerCoursesResponse
            {
                LecturerCourses = lecturerList,
            };

            return response;
        }

        //evtl TODO:
        //Abfrage der nächsten Termine eines Dozenten

        //Sprechstunden APIs
        /// <summary>
        /// Abfrage aller Sprechstunden aller Dozenten bis zum gewählten Zeitpunkt
        /// </summary>
        /// <param name="Until">Datum bis zu dem alle Sprechstunden aufgelistet werden sollen; Datum im Format dd.MM.yyyy </param>
        /// <returns>Liste aller verfügbaren Sprechstunden bis zum Datum</returns>
        /*
        public LecturersOfficeHourResponse GetAllWeekOfficeHours(string Until)
        {
            var until =DateTime.Parse(Until);

            var officeHourService = new LecturerInfoService();

            var officeHourList = officeHourService.GetAllOfficehours(until);

            var response = new LecturersOfficeHourResponse
            {
                OfficeHours = officeHourList,
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller noch stattfinden Sprechstunden eines Profs im Semester
        /// </summary>
        /// <param name="LecturerId">Id des Dozenten</param>
        /// <param name="Until">Datum bis zu dem alle Sprechstundentermine aufgelistet werden sollen; Datum im Format dd.MM.yyyy</param>
        /// <returns>Liste aller noch stattfindenen Spechstunden eines Dozenten</returns>
        public LecturerOfficeHoursResponse GetOfficeHour (string LecturerId, string Until)
        {
            var until = DateTime.Parse(Until);
            var officeHourService = new LecturerInfoService();

            var officeHours = officeHourService.GetLecturerOfficehours(LecturerId, until);

            var response = new LecturerOfficeHoursResponse
            {
                OfficeHours = officeHours,
            };

            return response;
        }

        /// <summary>
        /// Buchung einer Sprechstunde als Student
        /// </summary>
        /// <param name="OfficehourId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Action BookOfficeHourAsStudent (string OfficehourId, string userId)
        {
            return null;
        }

        /// <summary>
        /// Absage einer Buchung als Student
        /// </summary>
        /// <param name="OfficehourId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Action DeletOfficeHourBookingAsStudent(string OfficehourId, string userId)
        {
            return null;
        }

        /// <summary>
        /// Absage einer Sprechstunde als Dozent
        /// </summary>
        /// <param name="OfficehourId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Action DeletOfficeHourBookingAsLecturer(string OfficehourId, string userId)
        {
            return null;
        }
        */
    }
}
