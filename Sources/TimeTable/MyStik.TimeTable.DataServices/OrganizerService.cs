using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public class OrganizerService
    {
        private TimeTableDbContext _db;

        public OrganizerService()
        {
            _db = new TimeTableDbContext();
        }

        public OrganizerService(TimeTableDbContext db)
        {
            _db = db;
        }


        public ActivityOrganiser GetOrganiser(string shortName)
        {
            return _db.Organisers.SingleOrDefault(d => d.ShortName.Equals(shortName, StringComparison.OrdinalIgnoreCase));
        }

        public ActivityOrganiser GetOrganiser(Guid id)
        {
            return _db.Organisers.SingleOrDefault(d => d.Id == id);
        }

        public ICollection<OrganiserMember> GetLecturers(ActivityOrganiser org, Semester sem)
        {
            var activeLecturers =
            _db.Members.Where(m => m.Organiser.Id == org.Id)
                .OrderBy(m => m.Name)
                .ToList();
            
            return activeLecturers;
        }
    }
}
