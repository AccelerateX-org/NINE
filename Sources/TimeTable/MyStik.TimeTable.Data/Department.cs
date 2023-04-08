using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Department
    {
        public Department()
        {
            Contacts = new HashSet<Contact>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }


    }
}
