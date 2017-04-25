using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Lottery
{
    public class LotteryDrawingReportModel
    {
        public LotteryDrawingReportModel()
        {
            Courses = new List<CourseDrawingReportModel>();
        }
        public Data.Lottery Lottery { get; set; }

        public List<CourseDrawingReportModel> Courses { get; private set; }


        public string CreateHtml()
        {
            var sb = new StringBuilder();

            foreach (var course in Courses)
            {
            sb.AppendLine("<div class=\"row\">");
                sb.AppendLine("<div class=\"col-md-12\">");
                    sb.AppendLine("<div class=\"portlet light bordered\">");
                        sb.AppendLine("<div class=\"portlet-title\">");
                            sb.AppendLine("<div class=\"caption\">");
                                sb.AppendLine("<i class=\"fa fa-random\"></i>");
                                sb.AppendFormat("<span class=\"caption-subject bold uppercase\"> {0}</span>", course.Course.Name);
                            sb.AppendLine("</div>");
                            sb.AppendLine("<div class=\"portlet-body\">");
                            
                                sb.AppendLine("<table class=\"table table-condensed\">");
                                sb.AppendLine("<thead>");
                                sb.AppendLine("<tr>");
                                sb.AppendLine("<th>Name</th>");
                                sb.AppendLine("<th>Gruppe</th>");
                                sb.AppendLine("<th>Vorher</th>");
                                sb.AppendLine("<th>Nachher</th>");
                                sb.AppendLine("<th>Bemerkung</th>");
                                sb.AppendLine("</tr>");
                                sb.AppendLine("</thead>");
                                sb.AppendLine("<tbody>");

                foreach (var subscription in course.Subscriptions)
                {
                    sb.AppendLine("<tr>");
                    sb.AppendFormat("<td>{0}</td>", subscription.Subscription.Id);
                    sb.AppendFormat("<td>{0}</td>", subscription.Subscription.TimeStamp);
                    sb.AppendFormat("<td>{0}</td>", subscription.StateBeforeDrawing);
                    sb.AppendFormat("<td>{0}</td>", subscription.StateAfterDrawing);
                    sb.AppendFormat("<td>{0}</td>", subscription.Remark);
                    sb.AppendLine("</tr>");
                }


                                sb.AppendLine("</tbody>");
                                sb.AppendLine("</table>");
                            sb.AppendLine("</div>");
                        sb.AppendLine("</div>");
                    sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            sb.AppendLine("</div>");
              


            }

            return sb.ToString();
        }


    }

    public class CourseDrawingReportModel
    {
        public CourseDrawingReportModel()
        {
            Subscriptions = new List<SuscriptionDrawingReportModel>();
        }

        public Course Course { get; set; }

        public List<SuscriptionDrawingReportModel> Subscriptions { get; private set; }
    }

    public class SuscriptionDrawingReportModel
    {
        public OccurrenceSubscription Subscription { get; set; }

        public string StateBeforeDrawing { get; set; }

        public int LapCountBeforeDrawing { get; set; }


        public string StateAfterDrawing { get; set; }

        public int LapCountAfterDrawing { get; set; }

        public string Remark { get; set; }

    }
}
