using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.DataServices.IO.Json.Data
{
    public class CourseLottery
    {
        public CourseLottery()
        {
            Lots = new List<CourseLotteryLot>();
        }

        public string Name { get; set; }

        public int SlotCount { get; set; }

        public DateTime FirstDrawing { get; set; }

        public DateTime LastDrawing { get; set; }

        public int Frequency { get; set; }


        public List<CourseLotteryLot> Lots { get; private set; }
    }
}
