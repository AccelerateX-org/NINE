using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class IntegrationController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(string m)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://www6.primuss.de/showcase/index.php");
            var request = new HttpRequestMessage(HttpMethod.Post, "");

            var byteArray = new UTF8Encoding().GetBytes("hinz:hnz789");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var formData = new List<KeyValuePair<string, string>>();
            formData.Add(new KeyValuePair<string, string>("service", "test"));

            request.Content = new FormUrlEncodedContent(formData);
            var response = await client.SendAsync(request);

            var str = await response.Content.ReadAsStringAsync();

            var model = JsonConvert.DeserializeObject(str, typeof(PrimussApiTest));

            ((PrimussApiTest) model).m = m;

            return View(model);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PrimussApiTest
    {
        /// <summary>
        /// 
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string m { get; set; }
    }
}