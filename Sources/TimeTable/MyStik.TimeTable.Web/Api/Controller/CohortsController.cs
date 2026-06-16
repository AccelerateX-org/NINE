using MyStik.TimeTable.DataServices.IO.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace MyStik.TimeTable.Web.Api.Controller
{
    [RoutePrefix("api/v2/cohorts")]
    public class CohortsController : ApiBaseController
    {
        [Route("")]
        [ResponseType(typeof(List<CohortEntityApiContract>))]

        public IHttpActionResult GetCohorts(string institution, string organiser = "", string program = "",
            string labels = "")
        {
            var list = new List<CohortEntityApiContract>();

            var inst = Db.Institutions.Include(institution1 => institution1.LabelSet.ItemLabels).FirstOrDefault(x => x.Tag.Equals(institution));
            if (inst == null)
                return NotFound();

            if (string.IsNullOrEmpty(organiser))
            {
                // nur auf Institutionsebene
                foreach (var itemLabel in inst.LabelSet.ItemLabels.ToList())
                {
                    var cohort = new CohortEntityApiContract
                    {
                        Id = itemLabel.Id,
                        Key = $"{inst.Tag}|{itemLabel.Name}",
                        Context = new CohortContextApiContract
                        {
                            Institution = inst.Tag,
                            Organiser = "",
                            Program = "",
                            Label = itemLabel.Name
                        }
                    };
                    list.Add(cohort);
                }

                return Ok(list);
            }

            // nur auf Organisatorebene
            var org = Db.Organisers.Include(activityOrganiser => activityOrganiser.LabelSet.ItemLabels).FirstOrDefault(x => x.ShortName.Equals(organiser) && x.Institution.Id == inst.Id);
            if (org == null)
                return NotFound();

            if (string.IsNullOrEmpty(program))
            {
                foreach (var itemLabel in org.LabelSet.ItemLabels.ToList())
                {
                    var cohort = new CohortEntityApiContract
                    {
                        Id = itemLabel.Id,
                        Key = $"{inst.Tag}|{org.ShortName}|{itemLabel.Name}",
                        Context = new CohortContextApiContract
                        {
                            Institution = inst.Tag,
                            Organiser = org.ShortName,
                            Program = "",
                            Label = itemLabel.Name
                        }
                    };
                    list.Add(cohort);
                }
                return Ok(list);
            }

            var programs = Db.Curricula.Where(x => x.Tag.Equals(program) && x.Organiser.Id == org.Id).Include(curriculum =>
                curriculum.LabelSet.ItemLabels).ToList();

            foreach (var curriculum in programs)
            {
                foreach (var itemLabel in curriculum.LabelSet.ItemLabels.ToList())
                {
                    var cohort = new CohortEntityApiContract
                    {
                        Id = itemLabel.Id,
                        Key = $"{inst.Tag}|{org.ShortName}|{curriculum.Tag}|{itemLabel.Name}",
                        Context = new CohortContextApiContract
                        {
                            Institution = inst.Tag,
                            Organiser = org.ShortName,
                            Program = curriculum.Tag,
                            Label = itemLabel.Name
                        }
                    };

                    // keine Duplikate, da Labels mehrfach vergeben werden können
                    if (list.All(x => x.Id != itemLabel.Id))
                    {
                        list.Add(cohort);
                    }

                }
            }

            return Ok(list);
        }

        [Route("{key}/schedule/{semester}")]
        [ResponseType(typeof(List<CohortScheduleApiContract>))]
        public IHttpActionResult GetSchedule(string key, string semester)
        {
            var list = new List<CohortScheduleApiContract>();

            // Kurse einsammeln


            return Ok(list);
        }
    }
}