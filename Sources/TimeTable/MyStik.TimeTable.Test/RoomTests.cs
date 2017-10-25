using System;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using System.Data.Entity;
using Moq;
using NUnit.Framework;



namespace MyStik.TimeTable.Test
{
    [TestFixture]
    public class Test
    {
        [OneTimeSetUp]
        public static void AssemblyInit()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", TestContext.CurrentContext.WorkDirectory);

            // TODO:
            // Datenbank hier manuell detachten
            // Infos hardcoded aus Connection String hier benutzen

            // Test.TimeTableDB => die muss raus, bevor die neue DB unter dem selben Namen, aber in einem
            // anderen Verzeichnis neu angelegt wird.
            
            // das wird benötigt, um die Datenbank neu anzulegen
            Database.SetInitializer(new TestDbInitializer());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TimeTableDbContext, Data.Migrations.Configuration>());
        }

 
        [Test]
        [Ignore("Database not implemented yet.")]
        public void RoomOK()
        {

            TimeTableDbContext db = TestDataService.GetDataContext();

           
            //Zählt angelegt Räume
            int n1 = db.Rooms.Count();



            //Muss 1 sein
            Assert.IsNotNull(n1);
            Assert.AreEqual(1, n1);

        }

        [Test]
        [Ignore("Database Context not implemented yet.")]
        public void TestFail()
        {

            TimeTableDbContext db = TestDataService.GetDataContext();

            //neue Datenbankverknüpfunt
            var db2 = new TimeTableDbContext();

            //Zählt angelegt Räume
            int n2 = db2.Rooms.Count();

            
            //Muss 1 sein
            Assert.AreEqual(1, n2);

        }

        [Test]
        [Ignore("Database not implemented yet.")]
        public void RoomAdd()
        {

            TimeTableDbContext db = TestDataService.GetDataContext();

            //Testen ob wirklich nur 1 Raum in der Datenbank ist
            int n2 = db.Rooms.Count();
            Assert.AreEqual(1, n2);

            // Neuen Raum hinzufügen
            Room r2 = new Room() 
            {   Name = "Computerlabor",
                Number = "R BG.089 " 
            };

            db.Rooms.Add(r2);

            // Schreib es in die Datenbank
            db.SaveChanges();

            // Zähle ob jetzt 2 Räume da sind
            var db3 = new TimeTableDbContext();
            int n3 = db3.Rooms.Count();

            Assert.AreEqual(2, n3);
        }

        [Test]
        public void RoomMock()
        {

            Room room = new Room() { Name = "IT-Labor", Number = "R 1.089" };
            Room room2 = new Room() { Name = "Hörsaal1", Number = "R 0.001" };

            var mock = new Mock<RoomService>();
            mock.Setup(service => service.GetRoomById(2)).Returns(room2);
            
            
            Assert.AreEqual(room2, mock.Object.GetRoomById(2));

        }
          


        [Test]
        public void ShowHowToMockAnInterface()
        {
            Person person = new Person() { LastName = "Graber", FirstName = "Johnny" };

            var mock = new Mock<IPersonService>();
            mock.Setup(service => service.GetPersonById(1)).Returns(person);

            Assert.AreEqual(person, mock.Object.GetPersonById(1));
        }






        [Test]
        [Ignore("Database not implemented yet.")]
        public void RoomName()
        {

            TimeTableDbContext db = TestDataService.GetDataContext();

            Room z1 = db.Rooms.OfType<Room>().SingleOrDefault(r1 => r1.Name.Equals("IT Labor"));
            


        }

        [Test]
        [Ignore("Database not implemented yet.")]
        public void CourseOK()
        {
            TimeTableDbContext db = TestDataService.GetDataContext();

       
            //int o = db.occurrencesubscriptions.count();
            //Course c = db.Activities.OfType<Course>().First();

            //Assert.AreEqual(tm.Occurrence.Subscriptions.Count, 1);

            int o = db.Activities.Count();

            Assert.AreEqual(2, o);
        }



        [Test]
        [Ignore("Database not implemented yet.")]
        public void CourseSubscritpions()
        {
            TimeTableDbContext db = TestDataService.GetDataContext();


            Course c1 = db.Activities.OfType<Course>().SingleOrDefault(a1 => a1.ShortName.Equals("TM"));
            Course c2 = db.Activities.OfType<Course>().SingleOrDefault(a2 => a2.ShortName.Equals("Info"));
           
        }














        [Test]
        [Ignore("Database not implemented yet.")]
        public void Inventory()
        {
            // Initialisiere die Testdaten muss am Anfang jeder Testmethode rein, um 
            // tatsächlich die Datenbankanzulegen und zu befüllen
            // Da man nicht weiß, welche Testmethode zuerst ausgeführt wird, muss es
            // halt in alle rein. "First come firt serve"
            TimeTableDbContext db = TestDataService.GetDataContext();

          
            

            
            // Testablauf
            // 1. Testfallobjekt besorgen
            //Course c = db.Activities.OfType<Course>().SingleOrDefault(a => a.ShortName.Equals("TM"));
            int n = db.Rooms.Count();
            // das sollte 1 sein

            Assert.IsNotNull(n);

            // das ist jetzt der explizite code, der gestestez wird
            
            //Room r = new Room();
            //db.Rooms.Add(r);
            // schreib es in die Datenbank
            //db.SaveChanges();

            //int n2 = db.Rooms.Count();
            // n2 ist 2, wir wissen aber nicht, ob es auch tatsächlich in der Datenbank gespeichert ist

            // ein besserer Test
            // ein neuer frischer DB-Context, d.h. leer, keine Verbindung zu db
            //var db2 = new TimeTableDbContext();
            //int n3 = db2.Rooms.Count();
            // muss zwinged 2 sein, wenn nicht, dann Fehler



           // Course c = db.Activities.OfType<Course>().First();


            /*
            Guid userId = new Guid();
            // 2. Vorbebingungen prüfen
            // z.B. Kurs voll geht nicht immer (Logik)
            //Assert(c.Occurrence.Subscriptions.Count == c.Occurrence.Capacity);
            
             * 
            // Kurs hat 3 Eintragungen (Faktum)

            // 3. der eigentliche Test
             * abcd = new ServiceXYZ()
            var myResult = abcd.SubscribeCourse(c.Id, userId);

            // 4. Ergebnis des Serviceaufrufs abgleichen
            // Ticket muss "bestzt" sein
             * Assert.IsEqual(myResult.ab, 4)
            */

            // ggf. 5. Nachbedingung
            //db.
        }


      /*  [TestMethod]
        public void CreateRoom()
        {
            // 1. Testobjekte aufbauen
            // MOCK-Objekte aufbauen
            Room r = new Room();
            r.Name = "IT Labor";
            r.Number = "R 1.083";

            // 2. Use Case ausführen
            // nicht existent

            // 3. Abgleich mit erwartetem Ergebnis
            Assert.IsNotNull(r.Name);
            Assert.IsNotNull(r.Number);
        }

        [TestMethod]
        public void CreateRoomFromService()
        {
            // 1. Testobjekte aufbauen
            // gibt es hier nicht
            string number = "R 1.083";
            string labName = "IT Labor";
            string wrongName = "Bio Labor";

            
            // 2. Use Case ausführen
            RoomService roomService = new RoomService();

            Room r = roomService.CreateRoom(number, wrongName);

            // 3. Abgleich mit erwartetem Ergebnis
            Assert.IsNotNull(r);
            Assert.IsNotNull(r.Name);
            Assert.IsNotNull(r.Number);
            Assert.Equals(r.Dates.Count, 0);
        }

        [TestMethod]
        public void CreateExistigRoomFromService()
        {
            // 1. Testobjekte aufbauen
            // gibt es hier nicht

            string number = "R 1.083";
            string labName = "IT Labor";
            string wrongName = "Bio Labor";

            // 2. Use Case ausführen
            RoomService roomService = new RoomService();

            Room r = roomService.CreateRoom(number, wrongName);

            // 3. Abgleich mit erwartetem Ergebnis
            Assert.Equals(labName, r.Name);
            Assert.Equals(number, r.Number);
            Assert.Equals(r.Dates.Count, 0);
        }

        public object room { get; set; }
    }

    [TestClass]
    public class RoomTests2
    {
        // www.stevemichael.net/how-do-i-use-moq-with-asp-net-mvc/
        // www.codeproject.com/Articles/460175/Two-strategies-for-testing-Entity-Framework-Effort
        // www.msdn.microsoft.com/en-us/data/dn314429.aspx



        */

    }






}
