using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyStik.TimeTable.Web.Controllers
{
    public class MoneyBanksController : ApiController
    {
        // GET: api/MoneyBanks  
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MoneyBanks/5  
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MoneyBanks  
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/MoneyBanks/5  
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/MoneyBanks/5  
        public void Delete(int id)
        {
        }
    }
}
