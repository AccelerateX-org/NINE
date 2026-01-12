using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using NodaTime;

namespace MyStik.TimeTable.Web.Controllers
{
    public class AutonomyController : BaseController
    {
        // GET: Autonomy
        public ActionResult Index(Guid id)
        {
            /*
            var orgs = Db.Organisers.ToList();
            foreach (var org in orgs)
            {
                if (org != null && org.Autonomy != null)
                {
                    foreach (var c in org.Autonomy.Committees.ToList())
                    {
                        if (c.Curriculum != null)
                        {
                            var cur = Db.Curricula.SingleOrDefault(x => x.Id == c.Curriculum.Id);
                            var aut = cur.Autonomy;
                            if (aut == null)
                            {
                                aut = new MyStik.TimeTable.Data.Autonomy
                                {
                                    Committees = new List<MyStik.TimeTable.Data.Committee>()
                                };
                                cur.Autonomy = aut;
                                Db.Autonomy.Add(aut);
                            }
                            if (!aut.Committees.Any(x => x.Id == c.Id))
                            {
                                aut.Committees.Add(c);
                            }
                            c.Curriculum = null;
                            org.Autonomy.Committees.Remove(c);
                        }
                    }
                }
            }
            Db.SaveChanges();
            */
            var org2 = GetOrganiser(id);
            var model = new OrgAutonomyModel
            {
                Organiser = org2,
            };


            ViewBag.UserRight = GetUserRight(org2);

            return View(model);
        }

        public ActionResult CreateCommittee(Guid id)
        {
            var org = GetOrganiser(id);

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

            /*
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
            */

            return RedirectToAction("Index");
        }


        public ActionResult Committee(Guid id)
        {
            var user = GetCurrentUser();

            var committee = Db.Committees.SingleOrDefault(x => x.Id == id);

            var member = committee.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.Member.UserId) && x.Member.UserId.Equals(user.Id));

            ViewBag.UserRight = GetUserRight();
            ViewBag.Member = member;


            return View(committee);
        }


        public ActionResult EditCommittee(Guid id)
        {
            var committee = Db.Committees.SingleOrDefault(x => x.Id == id);
            var org = Db.Organisers.FirstOrDefault(x => x.Autonomy.Committees.Any(y => y.Id == committee.Id));
            

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

            var model = new CommitteeCreateModel();
            model.CommitteeId = committee.Id;
            //model.CurriculumId = committee.Curriculum?.Id ?? Guid.Empty;
            model.Description = committee.Description;
            model.Name = committee.Name;


            return View(model);
        }

        [HttpPost]
        public ActionResult EditCommittee(CommitteeCreateModel model)
        {
            var committee = Db.Committees.SingleOrDefault(x => x.Id == model.CommitteeId);
            var org = Db.Organisers.FirstOrDefault(x => x.Autonomy.Committees.Any(y => y.Id == committee.Id));


            // keine Zwei Gremien mit dem selben Namen
            committee.Name = model.Name;
            committee.Description = model.Description;
            //committee.Curriculum = Db.Curricula.SingleOrDefault(x => x.Id == model.CurriculumId);

            Db.SaveChanges();
           

            return RedirectToAction("Index", new {id = org.Id});
        }

        public ActionResult DeleteCommittee(Guid id)
        {
            var committee = Db.Committees.SingleOrDefault(x => x.Id == id);
            var org = Db.Organisers.FirstOrDefault(x => x.Autonomy.Committees.Any(y => y.Id == committee.Id));

            foreach (var member in committee.Members.ToList())
            {
                Db.CommitteeMember.Remove(member);
            }

            Db.Committees.Remove(committee);
            Db.SaveChanges();

            return RedirectToAction("Index", new { id = org.Id });
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