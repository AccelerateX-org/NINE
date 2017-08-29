using System.IO;
using System.Linq;
using System.Web.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.GpUntis;
using MyStik.TimeTable.DataServices.Test;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    public class TestDataHub : Hub
    {
        /// <summary>
        /// 
        /// </summary>
        public void StartImport()
        {
            var db = new TimeTableDbContext();
            var service = new TestDataService(db);
            
            var msg = "Initialisiere Semester";

            Clients.Caller.updateProgress(msg, 25, 0);
            service.InitSemester();
            Clients.Caller.updateProgress(msg, 25, 100);

            msg = "Initialisiere Veranstalter";
            Clients.Caller.updateProgress(msg, 50, 0);

            const int nFKs = 14;
            for (var i = 1; i <= 14; i++)
            {
                var shortName = $"FK {i:00}";
                var longName = $"Fakultät {i:00}";

                var fk = service.InitOrganiser(shortName, longName);
                InitTestUser(db, fk);

                msg = $"Veranstalter {shortName} angelegt";
                Clients.Caller.updateProgress(msg, 50, (int)(i * 100)/nFKs);
            }
            Clients.Caller.updateProgress(msg, 50, 100);


            // Stundenpläne nach Vorhandensein von Untisdaten
            // finde den Pfad


            var dataPath = HostingEnvironment.MapPath(@"~\Assets\data\gpu");
            var semService = new SemesterService();
            var prevSemester = semService.GetPreviousSemester();
            var currentSemester = semService.GetCurrentSemester();

            var orgs = db.Organisers.ToList();
            foreach (var org in orgs)
            {
                var name = org.ShortName.Replace(" ", "").ToLower();

                var fkDir = Path.Combine(dataPath, name);
                if (Directory.Exists(fkDir))
                {
                    // da gibt es was
                    // jetzt die semester durchgehen
                    // vorheriges
                    // aktuelles
                    // nächstes nicht, weil "in Planung"

                    // hier der Untis Import
                    ImportSemester(prevSemester, org, fkDir);
                    ImportSemester(currentSemester, org, fkDir);

                    Clients.Caller.updateProgress(msg, 75, 100);
                }
            }


            msg = "Alle Daten importiert";
            Clients.Caller.updateProgress(msg, 100, 100);
        }


        private void ImportSemester(Semester sem, ActivityOrganiser org, string fkDir)
        {
            var semTerm = sem.Name.Substring(0, 2).ToLower();

            var msgBase = $"Importiere Stundenpläne {org.ShortName} für {sem.Name}: ";

            var semDir = Path.Combine(fkDir, semTerm);
            if (Directory.Exists(semDir))
            {
                FileReader reader = new FileReader();
                reader.ReadFiles(semDir);

                var importer = new SemesterImport(reader.Context, sem.Id, org.Id);

                var nC = reader.Context.Kurse.Count;
                var i = 0;

                foreach (var k in reader.Context.Kurse)
                {
                    var msgCourse = importer.ImportCourse(k);
                    i++;

                    var msg = msgBase + msgCourse;
                    Clients.Caller.updateProgress(msg, 75, (i * 100) / nC);
                }
            }

        }


        private void InitTestUser(TimeTableDbContext db, ActivityOrganiser org)
        {
            var userDb = new ApplicationDbContext();
            var userManager = new IdentifyConfig.ApplicationUserManager(new UserStore<ApplicationUser>(userDb));



            // Student
            // nur Name und Status
            var studUser = FindUser(userManager,org.ShortName, "stud");
            if (studUser == null)
            {
                studUser = CreateUser(userManager, GetUserName(org.ShortName, "stud"), "Student", org.ShortName, MemberState.Student);
            }

            var dozUser = FindUser(userManager, org.ShortName, "doz");
            if (dozUser == null)
            {
                dozUser = CreateUser(userManager, GetUserName(org.ShortName, "doz"), "Dozent", org.ShortName,
                    MemberState.Staff);
            }
            var dozMember = GetMemberByName(db, org, string.Format("{0}{1}", org.ShortName, "DOZ"));
            if (dozMember != null && dozUser != null)
            {
                dozMember.UserId = dozUser.Id;
                db.SaveChanges();
            }

            var adminUser = FindUser(userManager, org.ShortName, "admin");
            if (adminUser == null)
            {
                adminUser = CreateUser(userManager, GetUserName(org.ShortName, "admin"), "Admin", org.ShortName,
                    MemberState.Staff);
            }

            var adminMember = GetMemberByName(db, org, string.Format("{0}{1}", org.ShortName, "ADMIN"));
            if (adminMember != null && adminUser != null)
            {
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
                db.SaveChanges();
            }
        }

        private ApplicationUser FindUser(IdentifyConfig.ApplicationUserManager UserManager, string orgName, string roleName)
        {
            var name = GetUserName(orgName, roleName);
            var usr = UserManager.FindByName(name);
            return usr;
        }

        private string GetUserName(string orgName, string roleName)
        {
            var x = orgName.ToLower().Replace(" ", "");
            return string.Format("{0}-{1}", x, roleName);
        }


        private ApplicationUser CreateUser(IdentifyConfig.ApplicationUserManager UserManager, string userName, string firstName, string lastName, MemberState state)
        {
            var user = UserManager.FindByName(userName);
            if (user != null)
                return user;

            
            user = new ApplicationUser
            {
                Email = $"test.{userName}@fillter.org",
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                MemberState = state
            };

            var result = UserManager.Create(user, "Pas1234?");

            user = UserManager.FindByName(userName);

            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="org"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public OrganiserMember GetMemberByName(TimeTableDbContext db, ActivityOrganiser org, string shortName)
        {
            var member = org.Members.SingleOrDefault(m => m.ShortName.Equals(shortName));

            if (member == null)
            {
                member = new OrganiserMember
                {
                    ShortName = shortName,
                    Name = shortName,
                };

                db.Members.Add(member);
                org.Members.Add(member);

                db.SaveChanges();
            }

            return member;

        }


    }
}