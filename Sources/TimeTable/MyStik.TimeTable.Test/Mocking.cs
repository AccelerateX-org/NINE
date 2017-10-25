using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace MyStik.TimeTable.Test
{
    public interface IPersonService
    {
        Person GetPersonById(int id);
    }

    public class Person
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }

 
}
