using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class TeachingModule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int ECTS { get; set; }

        public virtual Semester Semester { get; set; }


        public virtual TeachingBuildingBlock TeachingBuildingBlock { get; set; }
    }
}
