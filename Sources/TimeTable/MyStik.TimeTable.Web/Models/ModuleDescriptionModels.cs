﻿using System;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class ExaminationEditModel
    {
        public Guid orgId { get; set; }

        public Guid moduleId { get; set; }

        public Guid examinationId { get; set; }

        public Guid accredidationId { get; set; }

        public Guid semesterId { get; set; }

        public Guid examOptId { get; set; }

        public Guid firstMemberId { get; set; }

        public Guid secondMemberId { get; set;}

        public string Conditions { get; set; }

        public string Utilities { get; set; }

        public Semester Semester { get; set; }

        public CurriculumModule Module { get; set; }

        public ModuleAccreditation Accreditation { get; set; }

        public OrganiserMember FirstMember { get; set; }

        public OrganiserMember SecondMember { get; set; }
    }

    public class ModuleSemesterView
    {
        public CurriculumModule CurriculumModule { get; set; }

        public Semester Semester { get; set; }

        public ModuleDescription ModuleDescription { get; set; }
        
    }
}