using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class CoursePlan
    {
        public CoursePlan()
        {
            ModuleMappings = new HashSet<ModuleMapping>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set;}

        public string Description { get; set; }

        public string UserId { get; set; }

        public virtual ICollection<ModuleMapping> ModuleMappings { get; set; }
    }
}
