using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Booking;

namespace MyStik.TimeTable.Web.Models
{
    public class CourseSelectModel
    {
        public ApplicationUser User { get; set; }
        public Student Student { get; set; }
        public CourseSummaryModel Summary { get; set; }
        public OccurrenceSubscription Subscription { get; set; }

        public BookingState BookingState { get; set; }

        public SemesterGroup SemesterGroup { get; set; }
    }


}