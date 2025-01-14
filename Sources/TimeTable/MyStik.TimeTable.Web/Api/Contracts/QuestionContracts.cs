using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    public class GymCreateApiQuestionModel
    {
        public string userId { get; set; }

        public string title { get; set; }

        public string module { get; set; }

        public ICollection<GymCreateApiAnswerModel> answers { get; set; }
    }

    public class GymCreateApiAnswerModel
    {
        public string text { get; set; }

        public bool isCorrect { get; set; }
    }
}