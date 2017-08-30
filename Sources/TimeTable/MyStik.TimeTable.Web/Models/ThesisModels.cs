using System;
using System.ComponentModel.DataAnnotations;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Thesis
    {
        /// <summary>
        /// 
        /// </summary>
        public string Titel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Deskription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Matrikelnr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string E_Mail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Studiengang { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Richtung { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Abgabe { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Bekanntgabe { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirmaName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string FirmaAbteilung { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirmaPerson { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirmaKontakt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Thema { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MailStu { get; set; }



    }

    /// <summary>
    /// 
    /// </summary>
    public class ThesisCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Start { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string End { get; set;  }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ThesisExamCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ExamId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StudentUserId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid FirstExaminerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SecondExaminerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string End { get; set; }
    }
}
