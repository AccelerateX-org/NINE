using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.Globalization;

namespace MyStik.TimeTable.Web.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkIcon"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="hosts"></param>
        /// <param name="showLinks"></param>
        /// <returns></returns>
        public static MvcHtmlString LecturerList(this HtmlHelper htmlHelper, ICollection<OrganiserMember> hosts, bool showLinks=true)
        {
            var sb = new StringBuilder();
            foreach (var host in hosts)
            {
                var lecName = string.IsNullOrEmpty(host.Name) ? "N.N." : host.Name;
                if (showLinks)
                {
                    sb.Append(htmlHelper.ActionLink(lecName, "Member", "Organiser",
                        new { id = host.Id}, null));
                }
                else
                {
                    sb.Append(lecName);
                }

                if (host != hosts.Last())
                {
                    sb.Append(htmlHelper.Raw(", "));
                }
            }

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="rooms"></param>
        /// <param name="showLinks"></param>
        /// <param name="showCapacity"></param>
        /// <returns></returns>
        public static MvcHtmlString RoomList(this HtmlHelper htmlHelper, ICollection<Room> rooms, bool showLinks=true, bool showCapacity=false)
        {
            var sb = new StringBuilder();
            foreach (var room in rooms)
            {
                if (showLinks)
                {
                    if (string.IsNullOrEmpty(room.Number))
                    {
                        sb.Append(htmlHelper.ActionLink("N.N.", "Calendar", "Room", new { id = room.Id }, null));
                    }
                    else
                    {
                        sb.Append(htmlHelper.ActionLink(room.FullName, "Calendar", "Room", new { id = room.Id }, null));
                    }
                }
                else
                {
                    sb.Append(room.Number);
                }

                if (showCapacity)
                {
                    sb.AppendFormat(" ({0})", room.Capacity);
                }

                if (room != rooms.Last())
                {
                    sb.Append(htmlHelper.Raw(", "));
                }
            }

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="groups"></param>
        /// <param name="showAvailability"></param>
        /// <returns></returns>
        public static MvcHtmlString GroupList(this HtmlHelper htmlHelper, ICollection<SemesterGroup> groups, bool showAvailability=false)
        {
            var sb = new StringBuilder();
            foreach (var group in groups)
            {
                var linkName = group.FullName;
                if (showAvailability && !group.IsAvailable)
                {
                    linkName += "(!)";
                }
                sb.Append(htmlHelper.ActionLink(linkName, "Group", "Semester", new { id = group.Id }, null));
                if (group != groups.Last())
                {
                    sb.Append(htmlHelper.Raw(", "));
                }
            }

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        public static MvcHtmlString GroupList(this HtmlHelper htmlHelper, ICollection<CurriculumGroup> groups)
        {
            var sb = new StringBuilder();
            foreach (var group in groups)
            {
                sb.Append(group.Name);
                if (group != groups.Last())
                {
                    sb.Append(htmlHelper.Raw(", "));
                }
            }

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="date"></param>
        /// <returns></returns>

        public static MvcHtmlString TimeSpan(this HtmlHelper htmlHelper, ActivityDate date)
        {
            return new MvcHtmlString(string.Format("{0:t} - {1:t}", date.Begin, @date.End));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static MvcHtmlString TimeSpan(this HtmlHelper htmlHelper, ActivitySlot date)
        {
            return new MvcHtmlString(string.Format("{0:t} - {1:t}", date.Begin, @date.End));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static MvcHtmlString TimeSpan(this HtmlHelper htmlHelper, SemesterDate date)
        {
            return date.From.Date == date.To.Date ? 
                new MvcHtmlString(string.Format("{0:d}", date.From)) : 
                new MvcHtmlString(string.Format("{0:d} - {1:d}", date.From, @date.To));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public static MvcHtmlString TimeSpan(this HtmlHelper htmlHelper, DateTime from, DateTime until)
        {
            return new MvcHtmlString(string.Format("{0:t} - {1:t}", @from, until));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static MvcHtmlString TimeSpanWithDate(this HtmlHelper htmlHelper, ActivityDate date)
        {
            return new MvcHtmlString(string.Format("{0:d} {1:t} - {2:t}", date.Begin, date.Begin, @date.End));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static MvcHtmlString TimeSpanWithDate(this HtmlHelper htmlHelper, ActivitySlot date)
        {
            return new MvcHtmlString(string.Format("{0:d} {1:t} - {2:t}", date.Begin, date.Begin, @date.End));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public static MvcHtmlString TimeSpanWithDate(this HtmlHelper htmlHelper, DateTime from, DateTime until)
        {
            return new MvcHtmlString(string.Format("{0:d} {1:t} - {2:t}", @from, @from, until));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
        public static MvcHtmlString DateList(this HtmlHelper htmlHelper, ICollection<CourseDateModel> dates)
        {
            var sb = new StringBuilder();

            foreach (var courseDate in dates)
            {
                sb.Append("<div>");

                sb.AppendFormat("{0} [{1:hh\\:mm} - {2:hh\\:mm}]",
                    courseDate.DefaultDate.ToString("dddd", new CultureInfo("de-DE")), courseDate.StartTime, courseDate.EndTime);

                sb.Append("</div>");

            }

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
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