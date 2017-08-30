using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Data.DefaultData;
using MyStik.TimeTable.DataServices.Curriculum;
using MyStik.TimeTable.Web.Areas.Admin.Models;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new DataAdminModel();

            var allOrgs = Db.Organisers.ToList();

            foreach (var organiser in allOrgs)
            {
                var orgModel = new OrgState
                {
                    Organiser = organiser,
                };

                AddUser(orgModel.Users, organiser.ShortName, "stud");
                AddUser(orgModel.Users, organiser.ShortName, "doz");
                AddUser(orgModel.Users, organiser.ShortName, "admin");


                model.Organisers.Add(orgModel);
            }

            var mc = new ModuleCatalogService();
            var allCurr = Db.Curricula.ToList();
            foreach (var curriculum in allCurr)
            {
                model.Curricula.Add(new CurriculumState
                {
                    Curriculum = curriculum,
                    ModuleCatalog = mc.GetCatalog(curriculum.Organiser.ShortName, curriculum.ShortName)
                });

            }





            return View(model);
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
        public ActionResult InitFK09()
        {
            var data = new InfrastructureData();

            // Alle Fakultäen einrichten
            data.InitOrganisationFK01();
            data.InitOrganisationFK02();
            data.InitOrganisationFK03();
            data.InitOrganisationFK04();
            data.InitOrganisationFK05();
            data.InitOrganisationFK06();
            data.InitOrganisationFK07();
            data.InitOrganisationFK08();
            data.InitOrganisationFK09();
            data.InitOrganisationFK10();
            data.InitOrganisationFK11();
            data.InitOrganisationFK12();
            data.InitOrganisationFK13();
            data.InitOrganisationFK14();

            data.InitMemberFK01();
            data.InitMemberFK02();
            data.InitMemberFK03();
            data.InitMemberFK04();
            data.InitMemberFK05();
            data.InitMemberFK06();
            data.InitMemberFK07();
            data.InitMemberFK08();
            data.InitMemberFK09();
            data.InitMemberFK10();
            data.InitMemberFK11();
            data.InitMemberFK12();
            data.InitMemberFK13();
            data.InitMemberFK14();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult InitWI()
        {
            var data = new InfrastructureData();

            data.InitCurriculaFK01();
            data.InitCurriculaFK02();
            data.InitCurriculaFK03();
            data.InitCurriculaFK04(); 
            data.InitCurriculaFK05();
            data.InitCurriculaFK06();
            data.InitCurriculaFK07();
            data.InitCurriculaFK08();
            data.InitCurriculaFK09();
            data.InitCurriculaFK10();       
            data.InitCurriculaFK11();
            data.InitCurriculaFK12();
            data.InitCurriculaFK13();
            data.InitCurriculaFK14();
            
            return RedirectToAction("Index");
        
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult InitWIModules()
        {
            var data = new InfrastructureData();

            data.InitModulesFK01();
            data.InitModulesFK02();
            data.InitModulesFK03();     
            data.InitModulesFK04();     
            data.InitModulesFK05();
            data.InitModulesFK06();
            data.InitModulesFK07();
            data.InitModulesFK08();
            data.InitModulesFK09();     // nur WI
            data.InitModulesFK10();     // leer
            data.InitModulesFK11();
            data.InitModulesFK12();
            data.InitModulesFK13();
            data.InitModulesFK14();

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