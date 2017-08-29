using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    /*public class RoomService
    {
       public Room CreateRoom(string number, string name)
        {
            TimeTableDbContext db = new TimeTableDbContext();

            Room r = new Room();
            r.Number = number;
            r.Name = name;


            db.Rooms.Add(r);
            db.SaveChanges();

            return r;
        }

        public Room CreateRoom()
        {
            throw new NotImplementedException();
        }
    }
}*/


    public interface RoomService
    {
        Room GetRoomById(int id);
    }

    /*
    public class GetRoom
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Capacity { get; set;}
        
    
    
    
    }
     */
}