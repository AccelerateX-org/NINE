using MyStik.TimeTable.Data;
using System.Collections.Generic;

namespace MyStik.TimeTable.Contracts
{
    public interface ITimeTableInfoService
    {
        /// <summary>
        /// Alle Kurse, die nicht zugewiesen sind
        /// </summary>
        /// <returns></returns>
        ICollection<Course> GetAllUnassignedCourses();

        /// <summary>
        /// Alle Kurse im Semester
        /// </summary>
        /// <param name="semester"></param>
        /// <returns></returns>
        ICollection<Course> GetCourses(string semester);

        ICollection<Course> GetCourses(string semester, string curriculumShortName);

        ICollection<Course> GetCourses(string semester, string curriculumShortName, int number);

        ICollection<Course> GetCourses(string semester, string curriculumShortName, int number, string group);

        ICollection<Course> GetCourses(string semester, string curriculumShortName, string type);


    }
}
