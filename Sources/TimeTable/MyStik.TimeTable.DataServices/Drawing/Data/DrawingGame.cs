using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Drawing.Data
{
    public class DrawingGame
    {
        public DrawingGame()
        {
            Lots = new List<DrawingLot>();
            Seats = new List<DrawingLot>();
        }

        public Student Student { get; set; }

        public LotteryGame Game { get; set; }

        public string Message { get; set; }

        public List<DrawingLot> Lots { get; }

        public List<DrawingLot> Seats { get; }
    }
}
