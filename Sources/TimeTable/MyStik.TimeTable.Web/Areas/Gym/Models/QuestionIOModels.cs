using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Areas.Gym.Models
{
    public class QuestionIOModel
    {
        public string id { get; set; }

        public string title { get; set; }

        public string text { get; set; }

        public List<AnswerIOModel> answers { get; set; }
    }

    public class AnswerIOModel
    {
        public string text { get; set; }

        public bool iscorrect { get; set; }
    }
}