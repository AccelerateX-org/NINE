﻿using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public class NewsletterSubscriptionTicket
    {


        public bool IsValid { get; set; }
        public bool OnWaitingList { get; set; }

        public string Message { get; set; }

        public Course Course { get; set; }

    }
}
