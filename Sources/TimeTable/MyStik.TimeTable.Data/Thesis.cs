using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Thesis
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set;  }


        /// <summary>
        /// Betreuer
        /// </summary>
        public OrganiserMember Lecturer { get; set; }

        /// <summary>
        /// Student
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }

        public DateTime Registration { get; set; }

        public bool? IsAccepted { get; set; }

        public DateTime? Acception { get; set; }

        public bool? IsDelivered { get; set; }

        public DateTime? Delivery { get; set; }

        public bool? IsGraded { get; set; }

        public DateTime? Grade { get; set; }

        public int? Mark { get; set; }


    }
}
