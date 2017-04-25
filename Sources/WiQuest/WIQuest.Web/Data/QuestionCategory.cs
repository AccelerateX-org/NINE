using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class QuestionCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string ShortName { get; set; }

        public string Name { get; set; }

        public int Reihenfolge { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}