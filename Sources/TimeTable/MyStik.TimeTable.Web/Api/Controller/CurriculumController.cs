using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class CurriculumController : ApiBaseController
    {

        //Fakultäts und Vorlesungs APIs
        /// <summary>
        /// Abfrage aller verfügbaren Fakultäten
        /// </summary>
        /// <returns>Liste aller verfügbaren Fakultäten</returns>
        public FacultiesResponse GetAllFaculties()
        {
            var curriculumService = new CurriculumInfoService();

            var facList = curriculumService.GetAllFaculties();

            var response = new FacultiesResponse
            {
                Faculties = facList,
            };
            return response;

        }


        /// <summary>
        /// Abfrage aller Studienprogramme 
        /// </summary>
        /// <returns>Liste aller Studienprogramme</returns>
        public CurriculumStudyprogramsResponse GetAllStudyprograms()
        {
            var curriculumService = new CurriculumInfoService();

            var programList = curriculumService.GetAllStudyprograms();

            var response = new CurriculumStudyprogramsResponse
            {
                Studyprograms = programList,
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller Studiengruppen eines Studienprogramms
        /// </summary>
        /// <param name="StudyprogramId"> Id des Studienprograms</param>
        /// <returns>Liste aller Studiengruppen eines Studienprogramms</returns>
        public CurriculumStudygroupsResponse GetAllStudygroups(string StudyprogramId)
        {
            var curriculumService = new CurriculumInfoService();

            var groupList = curriculumService.GetAllStudygroups(StudyprogramId);

            var response = new CurriculumStudygroupsResponse
            {
                Studygroups = groupList,
            };

            return response;
        }
        //alle Kurse einer Studiengruppe /Module
        /// <summary>
        /// Abfrage aller Kurse einer Studiengruppe
        /// </summary>
        /// <param name="StudygroupId">Id der Studiengruppe</param>
        /// <returns>Liste aller Kurse einer Studiengruppe</returns>
        public CurriculumStudygroupCoursesResponse GetStudygroupCourses(string StudygroupId)
        {
            var curriculumService = new CurriculumInfoService();

            var courseList = curriculumService.GetAllStudygroupCourses(StudygroupId);

            var response = new CurriculumStudygroupCoursesResponse
            {
                StudygroupLectures = courseList,
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller Termine eines Kurses
        /// </summary>
        /// <param name="LectureId">Id der Vorlesung</param>
        /// <returns>Alle Termine einer Vorlesung</returns>
        public CurriculumCourseDateResponse GetLectureDates(string LectureId)
        {
            var curriculumService = new CurriculumInfoService();

            var courseList = curriculumService.GetCourseDates(LectureId);

            var response = new CurriculumCourseDateResponse
            {
                LectureDates = courseList,
            };

            return response;
        }


    }
}
