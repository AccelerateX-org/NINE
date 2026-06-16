using System;
using System.Collections.Generic;
using System.Linq;

namespace MyStik.TimeTable.DataServices.IO.Contracts
{
    public class OrganiserContextApiContract
    {
        public string Institution { get; set; }
        public string Organiser { get; set; }
    }

    public class OrganiserEntityApiContract
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Short Name
        /// </summary>
        public string Key { get; set; }

        public OrganiserContextApiContract Context { get; set; }

        public string Title { get; set; }

    }

    public class OrganiserLecturerApiContract
    {
        /// <summary>
        /// Short Name
        /// </summary>
        public string MemberKey { get; set; }
        public string Name { get; set; }
    }

    public class OrganiserRoomApiContract
    {
        /// <summary>
        /// Short Name
        /// </summary>
        public string RoomKey { get; set; }

        public bool IsOwner { get; set; }
    }


    public class OrganiserCurriculumApiContract
    {
        /// <summary>
        /// Short Name
        /// </summary>
        public string CurriculumKey { get; set; }
        public string Name { get; set; }

        public string Alias { get; set; }
    }


    public class OrganiserModuleApiContract
    {
        /// <summary>
        /// Short Name
        /// </summary>
        public string ModuleKey { get; set; }
        public string Title { get; set; }
    }

    public class OrganiserCourseApiContract
    {
        public string CourseKey { get; set; }
        public string Title { get; set; }
    }


}