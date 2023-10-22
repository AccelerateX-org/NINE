using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class AutonomyController : BaseController
    {
        // GET: Autonomy
        public ActionResult Index(Guid? id)
        {
            var org = id == null ? GetMyOrganisation() : GetOrganiser(id.Value);

            var aut = org.Autonomy;

            // Fehelende Selbstverwaltung automatisch ergänzen
            if (aut == null)
            {
                aut = new Autonomy();
                aut.Committees = new List<Committee>();

                Db.Autonomy.Add(aut);
                org.Autonomy = aut;

                Db.SaveChanges();
            }


            var model = new OrgAutonomyModel
            {
                Organiser = org,
                Autonomy = aut
            };

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult CreateCommittee()
        {
            var org = GetMyOrganisation();


            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Text = "Fakultätsweit",
                Value = Guid.Empty.ToString()
            });



            var currList = Db.Curricula.Where(c => c.Organiser.Id == org.Id).OrderBy(c => c.ShortName).ToList();
            var sList = currList.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            selectList.AddRange(sList);

            
            ViewBag.Curricula = selectList;


            return View();
        }

        [HttpPost]
        public ActionResult CreateCommittee(CommitteeCreateModel model)
        {
            var org = GetMyOrganisation();

            var aut = org.Autonomy;

            Committee committee = null;

            // keine Zwei Gremien mit dem selben Namen
            if (model.CurriculumId == Guid.Empty)
            {
                committee = aut.Committees.FirstOrDefault(x => x.Name.Equals(model.Name) && x.Curriculum == null);
            }
            else
            {
                committee = aut.Committees.FirstOrDefault(x => x.Name.Equals(model.Name) && x.Curriculum != null && x.Curriculum.Id == model.CurriculumId);
            }

            if (committee == null)
            {
                committee = new Committee
                {
                    Name = model.Name,
                    Description = model.Description,
                    Autonomy = aut,
                    Curriculum = Db.Curricula.SingleOrDefault(x => x.Id == model.CurriculumId)
                };

                Db.Committees.Add(committee);
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        public ActionResult Committee(Guid id)
        {
            var user = GetCurrentUser();

            var committee = Db.Committees.SingleOrDefault(x => x.Id == id);

            var member = committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));

            ViewBag.UserRights = GetUserRight();
            ViewBag.Member = member;


            return View(committee);
        }



        public ActionResult AddMember(Guid id)
        {
            var committe = Db.Committees.SingleOrDefault(x => x.Id == id);
            var org = Db.Organisers.SingleOrDefault(x =>
                x.Autonomy != null && x.Autonomy.Committees.Any(c => c.Id == id));

            var model = new AddCommitteeMemberModel()
            {
                Committee = committe,
                OrganiserId2 = org.Id

            };

            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        [HttpPost]
        public ActionResult AddMembers(Guid CommitteeId, ICollection<Guid> DozIds)
        {
            var committee = Db.Committees.SingleOrDefault(x => x.Id == CommitteeId);


            if (DozIds == null || DozIds.Count == 0)
            {
                foreach (var member in committee.Members.ToList())
                {
                    Db.CommitteeMember.Remove(member);
                }

                Db.SaveChanges();

                return PartialView("_SaveSuccess");
            }


            foreach (var member in committee.Members.ToList())
            {
                var inList = DozIds.Contains(member.Member.Id);

                if (inList)
                {
                    DozIds.Remove(member.Member.Id);
                }
                else
                {
                    Db.CommitteeMember.Remove(member);
                }
            }

            // die verbleibenden sind neu

            foreach (var dozId in DozIds)
            {
                var cm = new CommitteeMember
                {
                    Member = Db.Members.SingleOrDefault(x => x.Id == dozId),
                    HasChair = false,
                    Committee = committee
                };

                Db.CommitteeMember.Add(cm);
            }


            Db.SaveChanges();

            return Json(new { result = "Redirect", url = Url.Action("Committee", new { id = committee.Id }) });

        }

        public ActionResult AddChair(Guid asid, Guid cmid)
        {
            var cm = Db.CommitteeMember.SingleOrDefault(x => x.Id == cmid);
            cm.HasChair = true;
            Db.SaveChanges();

            return RedirectToAction("Committee", new { id = asid });
        }

        public ActionResult RemoveChair(Guid asid, Guid cmid)
        {
            var cm = Db.CommitteeMember.SingleOrDefault(x => x.Id == cmid);
            cm.HasChair = false;
            Db.SaveChanges();

            return RedirectToAction("Committee", new { id = asid });
        }


        public ActionResult DeleteMember(Guid asid, Guid cmid)
        {
            var cm = Db.CommitteeMember.SingleOrDefault(x => x.Id == cmid);

            Db.CommitteeMember.Remove(cm);

            Db.SaveChanges();

            return RedirectToAction("Committee", new { id = asid });
        }


    }
}