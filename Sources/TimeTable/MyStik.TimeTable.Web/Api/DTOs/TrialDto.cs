using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    public class TrialDto
    {
        public string Title { get; set; }

        public List<TrialColDto> Columns { get; set; }
    }

    public class TrialColDto
    {
        public int ColNumber { get; set; }


        public string ColContent { get; set; }
    }
}