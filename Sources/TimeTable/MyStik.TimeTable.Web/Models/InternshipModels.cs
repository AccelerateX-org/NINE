using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class InternshipStateModel
    {
        /// <summary>
        /// User des Studenten
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Student (Verfasser)
        /// </summary>
        public Student Student { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Internship Internship { get; set; }

        public string PlanBeginn { get; set; }
        public string PlanEnd { get; set; }

    }

    public class InternshipAdvisorViewModel
    {
        public Internship Internship { get; set; }

        public string CorporateName { get; set; }

        public string PersonFirstName { get; set; }

        public string PersonLastName { get; set; }

        /// <summary>
        /// Tätigkeitsgebiet / Abteilung
        /// </summary>
        public string PersonAction { get; set; }


        public string PersonEMail { get; set; }

        public string PersonPhone { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class InternshipDetailModel
    {
        public ApplicationUser User { get; set; }

        public Student Student { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Internship Internship { get; set; }
    }



}