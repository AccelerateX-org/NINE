using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices
{
    public class SubscriptionService
    {
        public CourseSubscriptionTicket SubscribeCourse(Guid userId, Guid courseId)
        {

            TimeTableDbContext db = new TimeTableDbContext();

            
            return new CourseSubscriptionTicket();

        }

        public NewsletterSubscriptionTicket SubscribeNewsletter(Guid userId, Guid newsletterId)
        {

            TimeTableDbContext db = new TimeTableDbContext();
                        

            return new NewsletterSubscriptionTicket();
            
        }

        public OfficeHourSubscriptionTicket SubscribeOfficeHour(Guid userId, Guid officehourId)
        {

            TimeTableDbContext db = new TimeTableDbContext();
                        

            return new OfficeHourSubscriptionTicket();
            
        }

        public EventSubscriptionTicket SubscribeEvent(Guid userId, Guid eventId)
        {

            TimeTableDbContext db = new TimeTableDbContext();

            return new EventSubscriptionTicket();


        }

        public EventDateSubscriptionTicket SubscribeEventDate(Guid userId, Guid eventdateId)
        {

            TimeTableDbContext db = new TimeTableDbContext();

            return new EventDateSubscriptionTicket();



        }



    }
}
