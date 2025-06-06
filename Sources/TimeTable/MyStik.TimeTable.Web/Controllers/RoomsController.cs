﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Web;
using MyStik.TimeTable.Web.Areas.Gym.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class RoomsController : BaseController
    {
        // GET: Rooms
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var model = Db.Organisers.Where(o => o.RoomAssignments.Any()).OrderBy(s => s.Name).ToList();

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult Organiser(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

             model.Rooms = Db.Rooms.Where(x => x.Assignments.Any(a => a.Organiser.Id == id)).ToList();

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult Statistics()
        {
            return View();
        }
        public ActionResult Schedule(Guid? id)
        {
            var org = id == null ? GetMyOrganisation() : GetOrganiser(id.Value);
            ViewBag.UserRight = GetUserRight(org);
            return View();
        }

        public ActionResult Reports(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };


            ViewBag.UserRight = GetUserRight(org);
            return View(model);
        }


        public ActionResult Allocation()
        {

            return View();
        }

        public ActionResult Groups(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

            model.Rooms = Db.Rooms.Where(x => x.Assignments.Any(a => a.Organiser.Id == id)).ToList();

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult Offices(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

            model.Rooms = Db.Rooms.Where(x => x.Assignments.Any(a => a.Organiser.Id == id)).ToList();

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }


        [AllowAnonymous]
        public FileResult GetFamosState(string searchPattern = "")
        {
            var now = DateTime.Now;

            var semester = new SemesterService(Db).GetSemester(now);

            var rooms = string.IsNullOrEmpty(searchPattern) ?
                Db.Rooms.ToList() :
                Db.Rooms.Where(x => x.Number.ToLower().StartsWith(searchPattern.ToLower())).ToList();

            var roomsWithDate = rooms.Where(x => x.Dates.Any(d => d.Begin <= now && now <= d.End)).ToList();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "ID;Semester;Vorlesungstitel;Langtitel;Bemerkung;Dozent;Fakultaet;Start;Ende;Serientyp;Intervall;Serienendtyp;Wiederholung;Teilnehmer;Raum");


            writer.Write(Environment.NewLine);

            foreach (var room in roomsWithDate)
            {
                var dates = room.Dates.Where(d => d.Begin <= now && now <= d.End).ToList();

                foreach (var date in dates)
                {
                    var id = room.Id.ToString();
                    var sem = semester.Name;
                    var comment = string.Empty;
                    var org = date.Activity.Organiser != null ? date.Activity.Organiser.ShortName : string.Empty;

                    var lec = date.Hosts.FirstOrDefault() != null ? date.Hosts.FirstOrDefault()?.Name : string.Empty;

                    var dow = ((int)date.Begin.DayOfWeek == 0) ? 7 : (int)date.Begin.DayOfWeek;

                    var type = 0;
                    var interval = 1;
                    var typeEnd = 1;
                    var frequency = Math.Pow(2, dow);
                    var part = date.Activity.Occurrence != null ? date.Activity.Occurrence.Subscriptions.Count : 1;



                    writer.Write("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}",
                        id, sem,
                        date.Activity.Name, date.Activity.Name, comment, lec, org,
                        date.Begin.ToString("dd.MM.yyyy HH:mm"), date.End.ToString("dd.MM.yyyy HH:mm"),
                        type, interval, typeEnd, frequency, part,
                        room.Number
                        );
                    writer.Write(Environment.NewLine);
                }

            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Raumbelegung");
            sb.Append("_");
            sb.Append(now.ToString("yyyyMMdd"));
            sb.Append(".csv");



            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }


        [HttpPost]
        public ActionResult Upload(OrganiserViewModel model, HttpPostedFileBase attachmentRooms)
        {
            var temp = Db.RoomAssignments.Where(x => x.Organiser == null).ToList();
            foreach (var roomAssignment in temp)
            {
                Db.RoomAssignments.Remove(roomAssignment);
            }

            Db.SaveChanges();


            var org = Db.Organisers.Include(activityOrganiser => activityOrganiser.Members).SingleOrDefault(x => x.Id == model.Organiser.Id);

            var tempFile = Path.GetTempFileName();
            attachmentRooms.SaveAs(tempFile);

            var lines = System.IO.File.ReadAllLines(tempFile, Encoding.Default);

            var i = 0;

            foreach (var line in lines)
            {
                i++;
                if (i == 1) continue;

                var words = line.Split(';');
                var name = words[0].Trim();
                var shortName = words[1].Trim();
                var roomNumber = words[2].Trim();

                if (string.IsNullOrEmpty(shortName) || string.IsNullOrEmpty(roomNumber)) continue;
                var member = org.Members.FirstOrDefault(x => x.ShortName.Equals(shortName));
                if (member == null) continue;

                var room = Db.Rooms.Include(room1 =>
                    room1.Assignments.Select(roomAssignment => roomAssignment.Organiser)).Include(room2 =>
                    room2.RoomAccesses.Select(roomAccess => roomAccess.Member)).FirstOrDefault(x => x.Number.Equals(roomNumber));

                if (room == null)
                {
                    room = new Room
                    {
                        Number = roomNumber,
                        Name = "Büro",
                        IsBookable = false,
                        IsForLearning = false,
                        Capacity = 0
                    };

                    Db.Rooms.Add(room);
                }

                var rAssign = room.Assignments.FirstOrDefault(x => x.Organiser != null && x.Organiser.Id == org.Id);

                if (rAssign == null)
                {
                    rAssign = new RoomAssignment
                    {
                        Room = room,
                        Organiser = org,
                        IsOwner = true,
                    };

                    Db.RoomAssignments.Add(rAssign);
                }

                var rAccess = room.RoomAccesses.FirstOrDefault(x => x.Member.Id == member.Id);

                if (rAccess == null)
                {
                    rAccess = new RoomAccess
                    {
                        Member = member,
                        IsOwner = true,
                        MayBook = true,
                        Room = room
                    };

                    Db.RoomAccesses.Add(rAccess);
                }

                Db.SaveChanges();
            }


            return RedirectToAction("Organiser", new { id = org.Id });
        }

    }
}