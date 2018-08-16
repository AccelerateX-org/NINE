using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Roles="SysAdmin")]
    public class SysAdminController : BaseController
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
        /// <returns></returns>
        public ActionResult RepairMemberState()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RepairCurricula()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RenameGroups()
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RepairGroups()
        {
            var sem =  Db.Semesters.SingleOrDefault(s => s.Name.Equals("WS15"));

            if (sem != null)
            {
                var group = sem.Groups.SingleOrDefault(g => !string.IsNullOrEmpty(g.Name) && g.Name.Equals("C-IMT"));

                if (group != null)
                {
                    group.Name = "C";
                    Db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertIMT()
        {

            var wi = Db.Curricula.SingleOrDefault(x => x.ShortName.Equals("WI"));
            var wi1 = wi.CurriculumGroups.SingleOrDefault(x => x.Name.Equals("1"));
            var wi1C = wi1.CapacityGroups.SingleOrDefault(x => x.Name.Equals("C"));

            var imt = new GroupAlias
            {
                CapacityGroup = wi1C,
                Name = "1C-IMT"
            };

            Db.GroupAliases.Add(imt);
            wi1C.Aliases.Add(imt);

            Db.SaveChanges();


            return RedirectToAction("Index");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RepairSubscriptions()
        {
            var userDB = new ApplicationDbContext();

            var sem = SemesterService.GetSemester(DateTime.Today);

            var model = new List<SubTestModel>();

            foreach (var user in userDB.Users)
            {
                var subscriptions =
                    Db.Subscriptions.OfType<SemesterSubscription>()
                        .Where(s => s.UserId.Equals(user.Id) && s.SemesterGroup.Semester.Id == sem.Id).ToList();

                if (subscriptions.Count > 1)
                {
                    foreach (var sub in subscriptions)
                    {
                        if (sub != subscriptions.First())
                        {
                            model.Add(new SubTestModel()
                            {
                                User = user,
                                Subscription = sub
                            });
                        }
                    }
                }
            }


            foreach (var sub in model)
            {
                Db.Subscriptions.Remove(sub.Subscription);
            }
            Db.SaveChanges();


            return View(model);
        }
    }
}