using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Web.Utils;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Data;
using System.Web.Mvc;
using ImageMagick.ImageOptimizers;
using Microsoft.Ajax.Utilities;

namespace MyStik.TimeTable.Web.Jobs
{
    public class StudyPlanPrintJobDescription
    {
        public Guid CurriculumId { get; set; }

        public Guid SemesterId { get; set; }

        public Guid MemberId { get; set; }

        public string Remark { get; set; }
    }

    public class StudyPlanPrintJob
    {
        public void Print(StudyPlanPrintJobDescription jobDescription)
        {
            return;
        }
    }
}