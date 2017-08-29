using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Examiner
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual OrganiserMember Member { get; set; }


        public virtual StudentExam Exam { get; set; }

        /// <summary>
        /// Erstprüfer, zweitprüfer
        /// auch: Aufsicht???
        /// </summary>
        public string Role { get; set; }

    }
}
