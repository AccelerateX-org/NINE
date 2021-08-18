using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    public class GroupScheduleDto
    {
        public string Name { get; set; }

        public string ShortName { get; set; }

    }


    public class GroupScheduleBasketModel
    {
        public string Curriculum { get; set; }

        public string Group { get; set; }


    }
}