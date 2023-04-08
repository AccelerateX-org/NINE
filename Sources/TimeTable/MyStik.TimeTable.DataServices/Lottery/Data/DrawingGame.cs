using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Lottery.Data
{
    public class DrawingGame
    {
        public DrawingGame()
        {
            Lots = new List<DrawingLot>();
            Seats = new List<DrawingSeat>();
        }

        public string UserId { get; set; }

        public TimeTable.Data.Lottery Lottery { get; set; }

        public Student Student { get; set; }

        public LotteryGame LotteryGame { get; set; }

        public string Message { get; set; }

        public DateTime LastChange { get; set; }

        public int CoursesWanted
        {
            get
            {
                if (LotteryGame != null)
                    return LotteryGame.CoursesWanted;
                return 0;
            }
        }

        public bool AcceptDefault
        {
            get
            {
                if (LotteryGame != null)
                {
                    return LotteryGame.AcceptDefault;
                }

                return true;
            }
        }

        public List<DrawingLot> Lots { get; }

        public List<DrawingSeat> Seats { get; }
    }
}
