using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    public class BaseService
    {
        private readonly TimeTableDbContext _db;

        public BaseService()
        {
            _db = new TimeTableDbContext();
        }

        public BaseService(TimeTableDbContext db)
        {
            _db = db;
        }

        protected TimeTableDbContext Db => _db;
    }
}