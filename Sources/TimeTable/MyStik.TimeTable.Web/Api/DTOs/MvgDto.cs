using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    public class MvgScheduleDto
    {
        public string Number { get; set; }

        public string Destination { get; set; }

        public int Departure { get; set; }
    }
}