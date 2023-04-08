using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class MemberService
    {
        /// <summary>
        /// 
        /// </summary>
        protected TimeTableDbContext db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userManager"></param>
        public MemberService(TimeTableDbContext db)
        {
            this.db = db;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public OrganiserMember GetMember(string userId, Guid orgId)
        {
            var organiser = db.Organisers.SingleOrDefault(org => org.Id == orgId);
            if (organiser == null)
                return null;

            var member = organiser.Members.SingleOrDefault(m => !string.IsNullOrEmpty(m.UserId) &&  m.UserId.Equals(userId));

            return member;
        }


        public OrganiserMember GetMember(Guid id)
        {
            var member = db.Members.SingleOrDefault(m => m.Id == id);
            return member;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="semester"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        /*
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
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ActivityOrganiser GetOrganisation(string userId)
        {
            // Bei Studenten nach einer Eintragung suchen
            // das aktuelle Studium
            var student = db.Students.Where(x => !string.IsNullOrEmpty(x.UserId) && x.UserId.Equals(userId)).OrderByDescending(x => x.Created).FirstOrDefault();


            // Ist der Benutzer in einer Organisation?
            var memberOrg = GetFacultyMemberships(userId);

            // nur Student, kein Member
            if (student != null && !memberOrg.Any())
                return student.Curriculum.Organiser;

            // Member schlägt Studium
            if (memberOrg.Any())
                return memberOrg.First().Organiser;

            // default: nimm die erste
            return db.Organisers.FirstOrDefault();
        }



        internal List<OrganiserMember> GetMemberships(string userId)
        {
            return db.Members.Where(m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(userId)).ToList();
        }

        internal List<OrganiserMember> GetFacultyMemberships(string userId)
        {
            return db.Members.Where(m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(userId) && m.Organiser.IsFaculty && !m.Organiser.IsStudent).ToList();
        }

        internal OrganiserMember GetOrganiserMember(Guid orgid, string userId)
        {
            return db.Members.FirstOrDefault(m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(userId) && m.Organiser.Id == orgid);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public OrganiserMember GetMemberFromShortName(string orgName, string shortName)
        {
            var organiser = db.Organisers.SingleOrDefault(org => org.ShortName.ToUpper().Equals(orgName.ToUpper()));
            if (organiser == null)
                return null;

            var member = organiser.Members.SingleOrDefault(m => m.ShortName.Equals(shortName));

            return member;
        }

    }
}