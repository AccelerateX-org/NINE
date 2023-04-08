using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class ConsultationRequest
    {
        public string Lecturer { get; set; }

        public string Organiser { get; set; }
    }

    public class ConsultationResponse
    {
        public DateTime Begin { get; set; }

        public DateTime End { get; set; }
    }


    [System.Web.Http.RoutePrefix("api/v2/consultations")]

    public class ConsultationsController : ApiBaseController
    {


        [System.Web.Http.Route("available")]
        [HttpPost]
        public IQueryable<ConsultationResponse> AvailableDates([FromBody] ConsultationRequest request)
        {
            // Den Lehrenden nach org und shortname suchen
            var lecturer = Db.Members.FirstOrDefault(x => x.ShortName.Equals(request.Lecturer) && x.Organiser.ShortName.Equals(request.Organiser));

            if (lecturer == null)
                return null;

            var today = DateTime.Today;
            var semester = new SemesterService().GetSemester(today);

            var officeHours = Db.Activities.OfType<OfficeHour>().Where(x =>
                    x.Semester != null &&
                    x.Semester.Id == semester.Id && x.Owners.Any(y => y.Member != null && y.Member.Id == lecturer.Id))
                .ToList();


            // Db.Activities.OfType<OfficeHour>().Where(x => x.)
            // dann die Termine der nächsten x Tage (horizon)

            var now = DateTime.Now;

            var response = new List<ConsultationResponse>();

            foreach (var officeHour in officeHours)
            {
                foreach (var officeHourDate in officeHour.Dates.Where(x => x.Begin > now))
                {
                    if (officeHourDate.Slots.Any())
                    {
                        // nur leere Slots
                        foreach (var activitySlot in officeHourDate.Slots.Where(x => !x.Occurrence.Subscriptions.Any()))
                        {
                            response.Add(new ConsultationResponse
                            {
                                Begin = activitySlot.Begin,
                                End = activitySlot.End
                            });
                        }

                    }
                    else
                    {
                        response.Add(new ConsultationResponse
                        {
                            Begin = officeHourDate.Begin,
                            End = officeHourDate.End
                        });
                    }
                }
            }


            return response.AsQueryable();
        }


    }
}
