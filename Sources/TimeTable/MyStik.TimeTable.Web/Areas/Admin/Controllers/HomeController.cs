using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Areas.Admin.Models;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Roles = "SysAdmin")]
    public class HomeController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="users"></param>
        /// <param name="orgName"></param>
        /// <param name="roleName"></param>
        private void AddUser(ICollection<ApplicationUser> users, string orgName, string roleName)
        {
            var user = FindUser(orgName, roleName);
            if (user != null)
            {
                users.Add(user);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        private ApplicationUser FindUser(string orgName, string roleName)
        {
            var name = GetUserName(orgName, roleName);
            var usr = UserManager.FindByName(name);
            return usr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        private string GetUserName(string orgName, string roleName)
        {
            var x = orgName.ToLower().Replace(" ", "");
            return string.Format("{0}-{1}", x, roleName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateTestUser(Guid id)
        {
            var org = GetOrganiser(id);

            // Student
            // nur Name und Status
            var studUser = FindUser(org.ShortName, "stud");
            if (studUser == null)
            {
                studUser = CreateUser(GetUserName(org.ShortName, "stud"), "Student", org.ShortName, MemberState.Student);
            }

            var dozUser = FindUser(org.ShortName, "doz");
            if (dozUser == null)
            {
                dozUser = CreateUser(GetUserName(org.ShortName, "doz"), "Dozent", org.ShortName, MemberState.Staff);
                var dozMember = GetMemberByName(org, string.Format("{0}{1}", org.ShortName, "DOZ"));
                dozMember.UserId = dozUser.Id;
                Db.SaveChanges();
            }

            var adminUser = FindUser(org.ShortName, "admin");
            if (adminUser == null)
            {
                adminUser = CreateUser(GetUserName(org.ShortName, "admin"), "Admin", org.ShortName, MemberState.Staff);
                var adminMember = GetMemberByName(org, string.Format("{0}{1}", org.ShortName, "ADMIN"));
                adminMember.UserId = adminUser.Id;
                adminMember.IsAdmin = true;
                adminMember.IsCourseAdmin = true;
                adminMember.IsCurriculumAdmin = true;
                adminMember.IsEventAdmin = true;
                adminMember.IsNewsAdmin = true;
                adminMember.IsAlumniAdmin = true;
                adminMember.IsMemberAdmin = true;
                adminMember.IsRoomAdmin = true;
                adminMember.IsSemesterAdmin = true;
                adminMember.IsStudentAdmin = true;
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult InitWISemesterGroups()
        {
            var semester = Db.Semesters.FirstOrDefault();

            var isWS = semester.Name.StartsWith("WS");


            var currs = Db.Curricula.ToList();

            foreach (var curriculum in currs)
            {
                foreach (var curriculumGroup in curriculum.CurriculumGroups.ToList())
                {
                    foreach (var capacityGroup in curriculumGroup.CapacityGroups.ToList())
                    {
                        if ((capacityGroup.InWS && isWS) || (capacityGroup.InSS && !isWS))
                        {
                            var exist = semester.Groups.Any(g => g.CapacityGroup.Id == capacityGroup.Id);

                            if (!exist)
                            {
                                var semGroup = new SemesterGroup
                                {
                                    CapacityGroup = capacityGroup,
                                    Semester = semester
                                };

                                semester.Groups.Add(semGroup);
                                Db.SemesterGroups.Add(semGroup);
                            }
                        }
                    }
                }
            }


            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult InitWISchedule()
        {
            var dataPath = Server.MapPath(@"~\Assets\data\gpu");

            // Dateien umkopieren
            var files = Directory.EnumerateFiles(dataPath, "GPU*.txt");

            // dann einen Hub im View integrieren
            var tempDir = Path.GetTempPath();

            var semester = Db.Semesters.FirstOrDefault();
            var org = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));

            if (semester != null && org != null)
            {
                tempDir = Path.Combine(tempDir, semester.Name);
                tempDir = Path.Combine(tempDir, org.ShortName);

                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
            }

            foreach (var sfile in files)
            {
                string fn = Path.GetFileName(sfile);
                string tfile = Path.Combine(tempDir, fn);
                
                System.IO.File.Copy(sfile, tfile);
            }
            
            return RedirectToAction("Index");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public OrganiserMember GetMemberByName(ActivityOrganiser org, string shortName)
        {
            var member = org.Members.SingleOrDefault(m => m.ShortName.Equals(shortName));

            if (member == null)
            {
                member = new OrganiserMember
                {
                    ShortName = shortName,
                    Name = shortName,
                };

                Db.Members.Add(member);
                org.Members.Add(member);

                Db.SaveChanges();
            }

            return member;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private ApplicationUser CreateUser(string userName, string firstName, string lastName, MemberState state)
        {
            var user = UserManager.FindByName(userName);
            if (user != null)
                return user;

            user = new ApplicationUser
            {
                Email = string.Format("test.{0}@fillter.org", userName),
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                MemberState = state
            };

            var result = UserManager.Create(user, "Pas1234?");
            if (result == null) throw new ArgumentNullException("result");

            user = UserManager.FindByName(userName);

            return user;
        }

    }
}