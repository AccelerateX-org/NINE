using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using HtmlAgilityPack;
using MyStik.TimeTable.Web.Api.DTOs;

namespace MyStik.TimeTable.Web.Api.Controller
{
    [RoutePrefix("api/v2/trials")]

    public class TrialsController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        [Route("")]
        public IQueryable<TrialDto> GetTrials(string param1 = "", string param2 = "", string param3 = "")
        {
            var result = new List<TrialDto>();

            var url = "https://clinicaltrials.gov/ct2/results?cntry=DE&city=M%C3%BCnchen&dist=50&Search=Apply&recrs=e&age_v=&age=2&gndr=Male&type=Intr&rslt=With&phase=0";


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
            var web = new HtmlWeb();
            var doc = web.Load(url, "Get");

            //var node = doc.DocumentNode.SelectSingleNode("table[@id='theDataTable']");
            var nodes = doc.DocumentNode.SelectNodes("//table[@id='theDataTable']");

            var node = nodes.First();

            var rows = node.SelectNodes("tbody/tr");

            var model = new List<TrialDto>();

            var j = 0;

            foreach (var row in rows)
            {
                j++;
                var trial = new TrialDto();
                trial.Columns = new List<TrialColDto>();
                trial.Title = $"Studie {j}";

                var cols = row.SelectNodes("td");
                var i = 0;

                foreach (var col in cols)
                {
                    var text = col.InnerText;
                    i++;

                    var trialCol = new TrialColDto();
                    trialCol.ColNumber = i;
                    trialCol.ColContent = col.InnerText;

                    trial.Columns.Add(trialCol);
                }

                model.Add(trial);
            }


            return model.AsQueryable();
        }

    }


}
