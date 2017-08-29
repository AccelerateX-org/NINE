using System;
using System.Collections.Generic;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class LecturerController : ApiBaseController
    {
        
        //Dozenteninfo APIs
        /// <summary>
        /// Abfrage aller Dozenten
        /// </summary>
        /// <returns>Liste aller verfügbaren Dozenten</returns>
        public LecturersResponseExtended GetAllLecture()
        {
            var lecturerService = new LecturerInfoService();

            // Fakultät
            var orgService = new OrganizerService();
            var faculty = orgService.GetOrganiser("FK 09");
                
    
            // Lehrveranstaltungen im aktuellen Semester
            var semesterService = new SemesterService();
            var semester = semesterService.GetCurrentSemester();

            // Dozenten, die Lehrveranstaltungen im aktuellen Semester anbieten
            var activeLecturers = orgService.GetLecturers(faculty, semester);

            // Benutzerdaten
            var userService = new UserInfoService();
            

            // für jeden Dozenten
            var LecturerList = new List<LecturerContractExtended>();

            foreach (var lecturer in activeLecturers)
            {
                var lecModel = new LecturerContractExtended();

                lecModel.MemberId = lecturer.Id;
                lecModel.Title = lecturer.Role;
                lecModel.Room = "";

                // Details zum Benutzerkonto
                var user = userService.GetUser(lecturer.UserId);

                if (user != null)
                {
                    lecModel.FirstName = user.FirstName;
                    lecModel.LastName = user.LastName;
                    lecModel.Email = user.Email;
                }
                else
                {
                    lecModel.LastName = lecturer.Name;
                }

                // gibt es noch nicht
                lecModel.Functions = new List<string>();

                // Averfügbar Slots
                lecModel.AvailableSlots = lecturerService.GetAvailabeSlots(lecturer, semester);
                
                LecturerList.Add(lecModel);
            }



            var response = new LecturersResponseExtended()
            {
                Lecturers = LecturerList
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller Dozenten einer Fakultät
        /// </summary>
        /// <param name="FacId">Id des Organisationsveranstalters, z.B. von FK09 oder andere</param>
        /// <returns>Liste aller Dozenten einer Fakultät</returns>
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
    }
}
