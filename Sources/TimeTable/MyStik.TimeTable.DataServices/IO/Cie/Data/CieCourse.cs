﻿namespace MyStik.TimeTable.DataServices.IO.Cie.Data
{
    public class CieCourse
    {
        public string name { get; set; }
        public string id { get; set; }
        public string level { get; set; }
        public int availableSlots { get; set; }
        public string description { get; set; }

        public string lecturer { get; set; }

        public double ects { get; set; }

        public double usCredits { get; set; }

        public double semesterWeekHours { get; set; }

        public string courseStatus { get; set; }

        public string department { get; set; }


    }
}
