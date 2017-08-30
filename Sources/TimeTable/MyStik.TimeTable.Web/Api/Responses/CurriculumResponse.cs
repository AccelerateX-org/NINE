using System.Collections.Generic;
using MyStik.TimeTable.Web.Api.Contracts;

namespace MyStik.TimeTable.Web.Api.Responses
{
    /// <summary>
    /// Response zu Abfragen der Fakultäten
    /// </summary>
    public class FacultiesResponse
    {
        /// <summary>
        /// Liste der Fakultäten, siehe FacultiesContracts
        /// </summary>
        public IEnumerable<FacultiesContracts> Faculties { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage der Studienprogramme
    /// </summary>
    public class CurriculumStudyprogramsResponse
    {
        /// <summary>
        /// Liste der Studienprogramme einer Fakultät, siehe CurriculumStudyprogramConract
        /// </summary>
        public IEnumerable<CurriculumStudyprogramContract> Studyprograms { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage der Studiengruppen
    /// </summary>
    public class CurriculumStudygroupsResponse
    {
        /// <summary>
        /// Liste der Studiengruppen eines Studienprograms, siehe CurriculumStudygroupsContract
        /// </summary>
        public IEnumerable<CurriculumStudygroupsContract> Studygroups { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage der Kurse einer Studiengruppe
    /// </summary>
    public class CurriculumStudygroupCoursesResponse
    {
        /// <summary>
        /// List der Kurse einer Studiengruppe, siehe CurriculumCourseContract
        /// </summary>
        public IEnumerable<CurriculumCourseContract> StudygroupLectures { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage der Termine eines Kurses
    /// </summary>
    public class CurriculumCourseDateResponse
    {
        /// <summary>
        /// Alle Termine die eine Kurs hat, siehe CurriculumDateContract
        /// </summary>
        public CurriculumDateContract LectureDates { get; set; }
    }
}