using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class MemberService
    {
        protected IdentifyConfig.ApplicationUserManager UserManager;
        protected TimeTableDbContext db;


        public MemberService(TimeTableDbContext db, IdentifyConfig.ApplicationUserManager userManager)
        {
           UserManager = userManager;
            this.db = db;
        }


        public bool IsUserMemberOf(string userName, string orgName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(orgName))
                return false;


            var organiser = db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(orgName.ToUpper()));
            if (organiser == null)
                return false;

            var user = UserManager.FindByName(userName);
            if (user == null)
                return false;

            var member = organiser.Members.SingleOrDefault(m => m.UserId == user.Id);
            if (member == null)
                return false;

            return true;
        }

        public bool IsUserOrgAdmin(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            var db = new TimeTableDbContext();

            var user = UserManager.FindByName(userName);
            if (user == null)
                return false;

            var member = db.Organisers.FirstOrDefault(org => org.Members.Any(m => m.UserId == user.Id && m.IsAdmin));
            return member != null;
        }

        public bool IsUserOrgAdmin(string userName, Guid orgId)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            var db = new TimeTableDbContext();

            var user = UserManager.FindByName(userName);
            if (user == null)
                return false;

            var member = db.Organisers.FirstOrDefault(org => org.Id == orgId && org.Members.Any(m => m.UserId == user.Id && m.IsAdmin));
            return member != null;
        }

        public bool IsUserAdminOf(string userName, string orgName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(orgName))
                return false;

            var organiser = db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(orgName.ToUpper()));
            if (organiser == null)
                return false;

            var user = UserManager.FindByName(userName);
            if (user == null)
                return false;

            var member = organiser.Members.SingleOrDefault(m => m.UserId == user.Id);
            if (member == null)
                return false;

            return member.IsAdmin;
        }

        public bool HasRole(string userName, string orgName, string role)
        {
            var organiser = db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(orgName.ToUpper()));
            if (organiser == null)
                return false;

            var user = UserManager.FindByName(userName);
            if (user == null)
                return false;

            var member = organiser.Members.SingleOrDefault(m => m.UserId == user.Id);
            if (member == null)
                return false;

            return member.Role.ToUpper().Contains(role.ToUpper());
        }

        public OrganiserMember GetMember(string userName, string orgName)
        {
            var organiser = db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(orgName.ToUpper()));
            if (organiser == null)
                return null;

            var user = UserManager.FindByName(userName);
            if (user == null)
                return null;

            var member = organiser.Members.SingleOrDefault(m => m.UserId == user.Id);

            return member;
        }

        public OrganiserMember GetMember(string userName)
        {
            var user = UserManager.FindByName(userName);
            if (user == null)
                return null;

            var member = db.Members.FirstOrDefault(m => m.UserId == user.Id);

            return member;
        }


        public string GetOrganisationName(Semester semester, string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            var user = UserManager.FindByName(userName);
            if (user == null)
                return string.Empty;

            var organiser = db.Organisers.FirstOrDefault(org => org.Members.Any(m => m.UserId.Equals(user.Id)));
            if (organiser != null)
                return organiser.ShortName;

            // es könnte auch ein Student sein
            // Suche nach der Semstererinschreibung
            // dort findet sich der Studiengang und darüber dann der Veranstalter
            var semesterService = new SemesterService();
            var subscription = semesterService.GetSubscription(semester, user.Id);
            if (subscription == null)
                return string.Empty;

            return subscription.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser.ShortName;
        }

        public ActivityOrganiser GetOrganisation(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            var user = UserManager.FindByName(userName);
            if (user == null)
                return null;

            // Ist der Benutzer in einer Organisation?
            var memberOrg = db.Organisers.FirstOrDefault(org => org.Members.Any(m => m.UserId.Equals(user.Id)));
            if (memberOrg != null)
                return memberOrg;

            // wenn nicht: existiert eine Semestereintragung? kann auch in der Vargangenheit liegen
            var subscription = db.Subscriptions.OfType<SemesterSubscription>().FirstOrDefault(s => s.UserId.Equals(user.Id));
            if (subscription != null && subscription.SemesterGroup.CapacityGroup.CurriculumGroup != null)
            {
                return subscription.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser;
            }

            // default: FK 09
            return db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));
        }


        public string IsOrgMember(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;

            var user = UserManager.FindByName(userName);
            if (user == null)
                return string.Empty;

            var organiser = db.Organisers.FirstOrDefault(org => org.Members.Any(m => m.UserId.Equals(user.Id)));
            if (organiser != null)
                return organiser.ShortName;

            return string.Empty;
        }


        public bool IsStudent(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return true;

            var user = UserManager.FindByName(userName);
            if (user == null)
                return true;

            // Student
            // Mitglied in keiner Organisation
            // oder
            // Mitglied in in mindestens einer studentischen Organisation

            var organisers = db.Organisers.Where(org => org.Members.Any(m => m.UserId.Equals(user.Id)));
            if (!organisers.Any())
                return true;

            // ist der Benutzer Mitglied in einer studentischen Oragnisation?
            var orgCount = organisers.Count(org => org.IsStudent);
            if (orgCount > 0)
                return true;

            return false;

        }

        /// <summary>
        /// Überprüft, ob der Benutzer im angegebenen Semester Dozent ist
        /// d.h. es gibt mindestens eine LV mit mindestens einem Termin,
        /// bei der der User als Host eingetragen ist
        /// </summary>
        /// <param name="userId">Id des Benutzers</param>
        /// <param name="semester">Semester</param>
        /// <returns></returns>
        public bool IsLecturer(string userId, Semester semester)
        {
            return
                db.Activities.OfType<Course>()
                    .Any(c => 
                        c.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                        c.Dates.Any(d => d.Hosts.Any(h => h.UserId != null && h.UserId.Equals(userId))));
        }

        internal ICollection<OrganiserMember> GetMemberships(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return new List<OrganiserMember>();

            var user = UserManager.FindByName(userName);
            if (user == null)
                return new List<OrganiserMember>();

            return db.Members.Where(m => m.UserId.Equals(user.Id)).ToList();
        }
    }
}