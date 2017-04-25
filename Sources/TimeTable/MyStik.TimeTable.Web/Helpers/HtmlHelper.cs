using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using Microsoft.Ajax.Utilities;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.Globalization;

namespace MyStik.TimeTable.Web.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ActionButton(this HtmlHelper htmlHelper, string linkIcon, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            var repId = Guid.NewGuid().ToString();
            var lnk = htmlHelper.ActionLink(repId, actionName, controllerName, routeValues, htmlAttributes);

            var linkBuilder = new TagBuilder("i");
            linkBuilder.AddCssClass(linkIcon);
            linkBuilder.AddCssClass("fa");

            var sb = new StringBuilder();
            sb.Append(linkBuilder.ToString());
            sb.Append(" " + linkText);

            return MvcHtmlString.Create(lnk.ToString().Replace(repId, sb.ToString()));
        }

        public static MvcHtmlString LecturerList(this HtmlHelper htmlHelper, ICollection<OrganiserMember> hosts)
        {
            var sb = new StringBuilder();
            foreach (var host in hosts)
            {
                var lecName = host.Name;
                sb.Append(htmlHelper.ActionLink(lecName, "Member", "Organiser",
                    new {orgId = host.Organiser.ShortName, shortName = host.ShortName}, null));
                if (host != hosts.Last())
                {
                    sb.Append(htmlHelper.Raw(", "));
                }
            }

            return new MvcHtmlString(sb.ToString());
        }


        public static MvcHtmlString RoomList(this HtmlHelper htmlHelper, ICollection<Room> rooms)
        {
            var sb = new StringBuilder();
            foreach (var room in rooms)
            {
                sb.Append(htmlHelper.ActionLink(room.Number, "Calendar", "Room", new { id = room.Id }, null));
                sb.AppendFormat(" ({0})", room.Capacity);
                if (room != rooms.Last())
                {
                    sb.Append(htmlHelper.Raw(", "));
                }
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString GroupList(this HtmlHelper htmlHelper, ICollection<SemesterGroup> groups)
        {
            var sb = new StringBuilder();
            foreach (var group in groups)
            {
                sb.Append(htmlHelper.ActionLink(group.FullName, "Group", "Semester", new { id = group.Id }, null));
                if (group != groups.Last())
                {
                    sb.Append(htmlHelper.Raw(", "));
                }
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString GroupListExtended(this HtmlHelper htmlHelper, ICollection<SemesterGroup> groups)
        {
            var sb = new StringBuilder();
            foreach (var group in groups)
            {
                sb.Append("<div>");

                sb.AppendFormat("{0} {1}",
                    group.Semester.Name,
                    group.FullName);

                sb.Append("</div>");
            }

            return new MvcHtmlString(sb.ToString());
        }


        public static MvcHtmlString TimeSpan(this HtmlHelper htmlHelper, ActivityDate date)
        {
            return new MvcHtmlString(string.Format("{0} - {1}", date.Begin.ToString("t"), @date.End.ToString("t")));
        }

        public static MvcHtmlString TimeSpan(this HtmlHelper htmlHelper, ActivitySlot date)
        {
            return new MvcHtmlString(string.Format("{0} - {1}", date.Begin.ToString("t"), @date.End.ToString("t")));
        }

        public static MvcHtmlString TimeSpan(this HtmlHelper htmlHelper, SemesterDate date)
        {
            return date.From.Date == date.To.Date ? 
                new MvcHtmlString(string.Format("{0}", date.From.ToString("d"))) : 
                new MvcHtmlString(string.Format("{0} - {1}", date.From.ToString("d"), @date.To.ToString("d")));
        }

        public static MvcHtmlString TimeSpan(this HtmlHelper htmlHelper, DateTime from, DateTime until)
        {
            return new MvcHtmlString(string.Format("{0} - {1}", from.ToString("t"), until.ToString("t")));
        }



        public static MvcHtmlString TimeSpanWithDate(this HtmlHelper htmlHelper, ActivityDate date)
        {
            return new MvcHtmlString(string.Format("{0} {1} - {2}", date.Begin.ToString("d"), date.Begin.ToString("t"), @date.End.ToString("t")));
        }

        public static MvcHtmlString TimeSpanWithDate(this HtmlHelper htmlHelper, ActivitySlot date)
        {
            return new MvcHtmlString(string.Format("{0} {1} - {2}", date.Begin.ToString("d"), date.Begin.ToString("t"), @date.End.ToString("t")));
        }


        public static MvcHtmlString TimeSpanWithDate(this HtmlHelper htmlHelper, DateTime from, DateTime until)
        {
            return new MvcHtmlString(string.Format("{0} {1} - {2}", from.ToString("d"), from.ToString("t"), until.ToString("t")));
        }


        public static MvcHtmlString DateList(this HtmlHelper htmlHelper, ICollection<CourseDateModel> dates)
        {
            var sb = new StringBuilder();

            foreach (var courseDate in dates)
            {
                sb.Append("<div>");

                sb.AppendFormat("{0} [{1} - {2}]",
                    courseDate.DefaultDate.ToString("dddd", new CultureInfo("de-DE")),
                    courseDate.StartTime.ToString(@"hh\:mm"),
                    courseDate.EndTime.ToString(@"hh\:mm"));

                sb.Append("</div>");

            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString DateList(this HtmlHelper htmlHelper, ICollection<ActivityDate> dates)
        {
            var sb = new StringBuilder();

            foreach (var courseDate in dates)
            {
                sb.Append("<div>");

                sb.AppendFormat("{0} [{1} - {2}]",
                    courseDate.Begin.ToString("dd. MMMM yyyy", new CultureInfo("de-DE")),
                    courseDate.Begin.ToString(@"hh\:mm"),
                    courseDate.End.ToString(@"hh\:mm"));

                sb.Append("</div>");

            }

            return new MvcHtmlString(sb.ToString());
        }
    }
}