using System;
using System.Collections.Generic;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class ScriptCreatetModel
    {
        public string Title { get; set; }

        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HttpPostedFileBase ScriptDoc { get; set; }


        public string Module { get; set; }

        public bool AllCourses { get; set; }

        public Semester Semester { get; set; }

        public Guid SemesterId { get; set; }

        public Guid DocId { get; set; }

    }



    public class OrderPeriodCreatetModel
    {
        public Semester Semester { get; set; }

        public Guid SemesterId { get; set; }


        public string Title { get; set; }

        public string Description { get; set; }
        public string Begin { get; set; }
        public string End { get; set; }

    }


    public class ScriptShopPeriodModel
    {
        public ScriptShopPeriodModel()
        {
            Courses = new List<ScriptShopCourseModel>();
        }

        public ApplicationUser User { get; set; }

        public OrderPeriod Period { get; set; }

        public List<ScriptShopCourseModel> Courses { get; set; }

    }


    public class ScriptShopCourseModel
    {
        public ScriptShopCourseModel()
        {
            Documents = new List<ScriptShopDocumentModel>();
        }

        public CourseSummaryModel CourseSummary { get; set; }


        public List<ScriptShopDocumentModel> Documents { get; set; }

    }


    public class ScriptShopDocumentModel
    {
        public ScriptPublishing Publishing { get; set; }

        public List<ScriptOrder> Orders { get; set; }
    }


    public class ScriptOrderDetailsModel
    {
        public ScriptOrderDetailsModel()
        {
            Persons = new List<ScriptOrderPersonModel>();
            Documents = new List<ScriptOrderDocumentModel>();
        }


        public OrderPeriod Period { get; set; }


        public List<ScriptOrderPersonModel> Persons { get; set; }

        public List<ScriptOrderDocumentModel> Documents { get; set; }

    }


    public class ScriptOrderPersonModel
    {
        public ApplicationUser User { get; set; }

        public Student Student { get; set; }

        public OrderBasket Basket { get; set; }
    }


    public class ScriptOrderDocumentModel
    {
        public ScriptOrderDocumentModel()
        {
            Orderers = new List<ScriptOrderPersonModel>();
        }


        public ScriptDocument Document { get; set; }

        public List<ScriptOrderPersonModel> Orderers { get; set; }


    }


    public class ScriptOrderHistoryModel
    {
        public ScriptOrderHistoryModel()
        {
            Baskets = new List<OrderBasket>();
        }

        public OrderPeriod Period { get; set; }

        public List<OrderBasket> Baskets { get; set; }
    }

}