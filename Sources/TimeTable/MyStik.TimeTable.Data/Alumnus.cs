using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Alumnus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// public access
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CodeExpiryDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// required
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Gender { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        public string Faculty { get; set; }

        public string Program { get; set; }

        /// <summary>
        /// Bachelor, Master, Promotion
        /// </summary>
        public string Degree { get; set; }

        /// <summary>
        /// Bezeichnung des Semesters
        /// </summary>
        public string FinishingSemester { get; set; }
       


        public DateTime? Created { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public virtual Student Student { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// Studiengang des Absolventen
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }

        /// <summary>
        /// Abschlusssemester
        /// </summary>
        public virtual Semester Semester { get; set; }

        /// <summary>
        /// UserId - kann auch leer sein oder ins "nichts" zeigen
        /// </summary>
        public string UserId { get; set; }
    }
}
