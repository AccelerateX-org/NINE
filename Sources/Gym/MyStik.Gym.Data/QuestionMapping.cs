using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyStik.Gym.Data
{
    public class QuestionMapping
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Question Question { get; set; }

        public virtual QuestionSet QuestionSet { get; set; }
    }

    public class QuestionSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public virtual ICollection<QuestionMapping> Mappings { get; set; } 

    }
}
