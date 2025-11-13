using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.Services
{
    public class ApiKeyService
    {
        private readonly TimeTableDbContext _db;

        public ApiKeyService(TimeTableDbContext db) {
            _db = db;
        }

        public OrganiserMember IsValidApiKey(string apiKey)
        {
            return null;
            /*
            var member = _db.Members.FirstOrDefault(x => !string.IsNullOrEmpty(x.ApiKey) && x.ApiKey.Equals(apiKey));

            if (member == null)
            {
                return null;
            }

            if (member.ApiKeyValidUntil.HasValue && member.ApiKeyValidUntil.Value < DateTime.Now)
            {
                return null;
            }

            return member;
            */
        }
    }
}