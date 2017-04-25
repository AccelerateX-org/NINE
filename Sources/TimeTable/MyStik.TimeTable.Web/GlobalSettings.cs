using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Web
{
    public static class GlobalSettings
    {
        private static DateTime _today = DateTime.Now;

        public static void Init(DateTime refDate)
        {
            _today = refDate;
        }

        public static DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        public static DateTime Today
        {
            get
            {
                return DateTime.Today;
            }
        }

        public static DateTime Tomorrow
        {
            get { return Today.AddDays(1); }
        }
    }
}
