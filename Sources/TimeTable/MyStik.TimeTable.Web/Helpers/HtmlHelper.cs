using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.Globalization;
using MyStik.TimeTable.Web.Api.DTOs;

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
        public static MvcHtmlString LecturerList(this HtmlHelper htmlHelper, ICollection<OrganiserMember> hosts, bool showLinks=true, bool asInline=false)
        {
            var sb = new StringBuilder();

            foreach (var host in hosts)
            {
                var lecName = string.IsNullOrEmpty(host.Name) ? "N.N." : host.Name;
                if (showLinks)
                {
                    sb.Append(htmlHelper.ActionLink(lecName, "Private", "Person",
                        new { memberId = host.Id}, null));
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
        public static MvcHtmlString RoomList(this HtmlHelper htmlHelper, ICollection<Room> rooms, bool showLinks=true, bool showCapacity=false, bool asInline = false)
        {
            if (!rooms.Any())
                return new MvcHtmlString(string.Empty);

            var sb = new StringBuilder();
            /*
            if (asInline)
            {
                sb.Append("<i class=\"fas fa-li fa-building\"></i> ");
            }
            else
            {
                sb.Append("<i class=\"fa fa-building\"></i> ");
            }
            */
            foreach (var room in rooms)
            {
                if (showLinks)
                {
                    if (string.IsNullOrEmpty(room.Number))
                    {
                        sb.Append(htmlHelper.ActionLink("N.N.", "Details", "Room", new { id = room.Id }, null));
                    }
                    else
                    {
                        sb.Append(htmlHelper.ActionLink(room.Number, "Details", "Room", new { id = room.Id }, null));
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
        /// <param name="rooms"></param>
        /// <param name="showLinks"></param>
        /// <param name="showCapacity"></param>
        /// <returns></returns>
        public static MvcHtmlString RoomList(this HtmlHelper htmlHelper, ICollection<VirtualRoom> rooms, bool showLinks = true, bool showCapacity = false, bool asInline = false)
        {
            if (!rooms.Any())
                return new MvcHtmlString(string.Empty);

            var sb = new StringBuilder();

            /*
            if (asInline)
            {
                sb.Append("<i class=\"fas fa-li fa-tv\"></i>");
            }
            else
            {
                sb.Append("<i class=\"fa fa-tv\"></i> ");
            }
            */
            foreach (var room in rooms)
            {
                if (showLinks)
                {
                    if (string.IsNullOrEmpty(room.Name))
                    {
                        sb.Append(htmlHelper.ActionLink("N.N.", "Details", "VirtualRoom", new { id = room.Id }, null));
                    }
                    else
                    {
                        sb.Append(htmlHelper.ActionLink(room.Name, "Details", "VirtualRoom", new { id = room.Id }, null));
                    }
                }
                else
                {
                    sb.Append(room.Name);
                }

                /*
                if (showCapacity)
                {
                    sb.AppendFormat(" ({0})", room.Capacity);
                }
                */

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
        /// <param name="rooms"></param>
        /// <param name="showLinks"></param>
        /// <param name="showCapacity"></param>
        /// <returns></returns>
        public static MvcHtmlString RoomList(this HtmlHelper htmlHelper, ICollection<VirtualRoomAccess> rooms, bool showLinks = true, bool showCapacity = false, bool asInline = false)
        {
            if (!rooms.Any())
                return new MvcHtmlString(string.Empty);

            var sb = new StringBuilder();
            /*
            if (asInline)
            {
                sb.Append("<i class=\"fas fa-li fa-tv\"></i>");
            }
            else
            {
                sb.Append("<i class=\"fa fa-tv\"></i> ");
            }
            */
            foreach (var room in rooms)
            {
                if (showLinks)
                {
                    if (string.IsNullOrEmpty(room.Room.Name))
                    {
                        sb.Append(htmlHelper.ActionLink("N.N.", "Details", "VirtualRoom", new { id = room.Room.Id }, null));
                    }
                    else
                    {
                        sb.Append(htmlHelper.ActionLink(room.Room.Name, "Details", "VirtualRoom", new { id = room.Room.Id }, null));
                    }
                }
                else
                {
                    sb.Append(room.Room.Name);
                }

                /*
                if (showCapacity)
                {
                    sb.AppendFormat(" ({0})", room.Capacity);
                }
                */

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
        /// <param name="showLink"></param>
        /// <returns></returns>
        public static MvcHtmlString GroupList(this HtmlHelper htmlHelper, ICollection<SemesterGroup> groups, bool showAvailability=false, bool showLink=true)
        {
            var sb = new StringBuilder();
            foreach (var group in groups)
            {
                var linkName = group.FullName;
                if (showLink)
                {
                    sb.Append(htmlHelper.ActionLink(linkName, "Group", "Dictionary", new {semId = group.Semester.Id, groupId = group.CapacityGroup.Id}, null));
                }
                else
                {
                    sb.Append(linkName);
                }
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
        /// <param name="groups"></param>
        /// <param name="showAvailability"></param>
        /// <param name="showLink"></param>
        /// <returns></returns>
        public static MvcHtmlString GroupList(this HtmlHelper htmlHelper, Occurrence occ, bool showLinks=true)
        {
            var sb = new StringBuilder();
            foreach (var group in occ.Groups)
            {
                var firstGroup = group.SemesterGroups.FirstOrDefault();
                if (firstGroup == null)
                {
                    sb.Append("N.N.");
                }
                else
                {
                    sb.AppendFormat("{0} (", firstGroup.CapacityGroup.CurriculumGroup.Curriculum.ShortName);

                    foreach (var semesterGroup in group.SemesterGroups)
                    {
                        if (showLinks)
                        {
                            var linkName = semesterGroup.GroupName;

                            sb.Append(htmlHelper.ActionLink(linkName, "Group", "Dictionary",
                                new {semId = semesterGroup.Semester.Id, groupId = semesterGroup.CapacityGroup.Id},
                                null));
                        }
                        else
                        {
                            sb.Append(semesterGroup.GroupName);
                        }

                        if (semesterGroup != group.SemesterGroups.Last())
                        {
                            sb.Append(htmlHelper.Raw(", "));
                        }
                    }
                    sb.Append(")");
                }
                if (group != occ.Groups.Last())
                {
                    sb.Append(htmlHelper.Raw(", "));
                }
            }

            return new MvcHtmlString(sb.ToString());
        }


        public static MvcHtmlString TopicList(this HtmlHelper htmlHelper, ICollection<SemesterTopic> topics)
        {
            var sb = new StringBuilder();
            foreach (var group in topics)
            {
                var linkName = group.TopicName;
                sb.Append(linkName);

                if (group != topics.Last())
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
            if (from.Date == until.Date)
                return new MvcHtmlString(string.Format("{0:d} {1:t} - {2:t}", @from, @from, until));

            return new MvcHtmlString(string.Format("{0:d} {1:t} - {2:d} {3:t}", @from, @from, until, until));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
        public static MvcHtmlString Date(this HtmlHelper htmlHelper, CourseDateModel date, bool withTime=true)
        {
            var sb = new StringBuilder();

            if (withTime)
            {
                sb.AppendFormat("{0} [{1:hh\\:mm} - {2:hh\\:mm}]",
                    date.DefaultDate.ToString("dddd", new CultureInfo("de-DE")), date.StartTime, date.EndTime);
            }
            else
            {
                sb.AppendFormat("{0}",
                    date.DefaultDate.ToString("dddd", new CultureInfo("de-DE")));
            }


            return new MvcHtmlString(sb.ToString());
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

                sb.AppendFormat("{0} [{1:HH\\:mm} - {2:HH\\:mm}]",
                    courseDate.Begin.ToString("dd. MMMM yyyy", new CultureInfo("de-DE")),
                    courseDate.Begin,
                    courseDate.End);

                sb.Append("</div>");

            }

            return new MvcHtmlString(sb.ToString());
        }


        public static MvcHtmlString LabelList(this HtmlHelper htmlHelper, Course course)
        {
            if (course == null || course.LabelSet == null)
                return new MvcHtmlString(string.Empty);

            var sb = new StringBuilder();
            var db = new TimeTableDbContext();


            foreach (var label in course.LabelSet.ItemLabels)
            {
                sb.Append("<span class=\"badge bg-secondary\">");

                var curr = db.Curricula.FirstOrDefault(x => x.LabelSet.ItemLabels.Any(l => l.Id == label.Id));

                if (curr != null)
                {
                    sb.AppendFormat("{0}:{1}", curr.ShortName, label.Name);
                }

                var inst = db.Institutions.FirstOrDefault(x => x.LabelSet.ItemLabels.Any(l => l.Id == label.Id));

                if (inst != null)
                {
                    sb.AppendFormat("{0}:{1}", inst.Tag, label.Name);
                }

                sb.Append("</span>");
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString LabelLinkList(this HtmlHelper htmlHelper, Course course)
        {
            if (course == null || course.LabelSet == null)
                return new MvcHtmlString(string.Empty);

            var sb = new StringBuilder();
            var db = new TimeTableDbContext();

            var sem = course.Semester;

            foreach (var label in course.LabelSet.ItemLabels)
            {
                sb.Append("<div>");

                var curr = db.Curricula.FirstOrDefault(x => x.LabelSet.ItemLabels.Any(l => l.Id == label.Id));



                if (curr != null)
                {
                    var linkName = $"{curr.ShortName}:{label.Name}";
                    sb.Append(htmlHelper.ActionLink(linkName, "Label", "Dictionary", new { semId = sem.Id, orgId = curr.Organiser.Id, currId = curr.Id, labelId = label.Id}, null));
                }

                var inst = db.Institutions.FirstOrDefault(x => x.LabelSet.ItemLabels.Any(l => l.Id == label.Id));

                if (inst != null)
                {
                    //sb.AppendFormat("{0}:{1}", inst.Tag, label.Name);
                    //sb.Append(htmlHelper.ActionLink(room.Number, "Details", "Room", new { id = room.Id }, null));
                }

                sb.Append("</div>");
            }

            return new MvcHtmlString(sb.ToString());
        }


        public static MvcHtmlString FacultyList(this HtmlHelper htmlHelper, ICollection<OrganiserMember> members)
        {
            var sb = new StringBuilder();
            foreach (var member in members)
            {
                sb.Append(member.Organiser.ShortName);
                if (member != members.Last())
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
        /// <param name="dates"></param>
        /// <returns></returns>
        public static MvcHtmlString DateListWithRooms(this HtmlHelper htmlHelper, ICollection<ActivityDate> dates)
        {
            var sb = new StringBuilder();

            foreach (var courseDate in dates)
            {
                var sbRoom = new StringBuilder();
                foreach (var room in courseDate.Rooms)
                {
                    sbRoom.Append(room.Number);
                    if (room != courseDate.Rooms.Last())
                    {
                        sbRoom.Append(htmlHelper.Raw(", "));
                    }
                }

                sb.Append("<div>");

                if (courseDate.Occurrence.IsCanceled)
                {
                    sb.Append("<del>");
                }

                sb.AppendFormat("{0} [{1:HH\\:mm} - {2:HH\\:mm}] {3}",
                    courseDate.Begin.ToString("dd.MM.yyyy", new CultureInfo("de-DE")),
                    courseDate.Begin,
                    courseDate.End,
                    sbRoom.ToString());

                if (courseDate.Occurrence.IsCanceled)
                {
                    sb.Append("</del> abgesagt");
                }
                sb.Append("</div>");

            }

            return new MvcHtmlString(sb.ToString());
        }


        public static MvcHtmlString Date(this HtmlHelper htmlHelper, ActivityDate courseDate)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0} [{1:HH\\:mm} - {2:HH\\:mm}]",
                courseDate.Begin.ToString("dd. MMMM yyyy", new CultureInfo("de-DE")), courseDate.Begin, courseDate.End);

            return new MvcHtmlString(sb.ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="org"></param>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static MvcHtmlString FacultyLabel(this HtmlHelper htmlHelper, ActivityOrganiser org, string icon=null)
        {
            var sb = new StringBuilder();

            var color = string.IsNullOrEmpty(org.HtmlColor) ? "#ddd" : org.HtmlColor;

            if (string.IsNullOrEmpty(icon))
            {
                sb.AppendFormat(
                    "<span class=\"label\" style=\"background-color: {0}; color: white\">{1}</span>",
                    color, org.ShortName);
            }
            else
            {
                sb.AppendFormat(
                    "<span class=\"label\" style=\"background-color: {0}; color: white\"><i class=\"fa {1}\"></i> {2}</span>",
                    color, icon, org.ShortName);
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString SubscriptionLabel(this HtmlHelper htmlHelper, OccurrenceSubscription subscription)
        {
            var sb = new StringBuilder();

            var iconName = "";
            if (subscription.OnWaitingList)
            {
                iconName = "fa-hourglass";
            }
            else
            {
                if (subscription.IsConfirmed)
                {
                    iconName = "fa-group";
                }
                else
                {
                    iconName = "fa-ticket";
                }
            }

            sb.AppendFormat(
                "<i class=\"fa {0}\"></i>", iconName);

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString AssemblyVersion(this HtmlHelper helper)
        {
            //var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var version = @System.Diagnostics.FileVersionInfo
                .GetVersionInfo(typeof(MyStik.TimeTable.Web.Startup).Assembly.Location).ProductVersion;

            return MvcHtmlString.Create(version);
        }
    }
}