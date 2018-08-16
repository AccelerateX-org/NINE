using System;
using MyStik.TimeTable.DataServices;
using NUnit.Framework;

namespace MyStik.TimeTable.Test
{
    [TestFixture]
    public class StudentTest
    {
        [Test]
        public void SubscribeCourse()
        {
            // Situation
            // Student s möchte sich in Kurs k eintragen
            // Ich brauche einen Kurs
            // - ohne Beschränkung <====
            // - mit Beschränkung, aber freiem Platz

            // TestInfrastructureService tis = new TestInfrastructureService();
            // Guid courseId = tis.CreateCourse("TM", "WI", "1", "A", 10, 0);
            // Guid userId = tis.CreateStudent("MBA", "3");


            //SubscriptionService service = new SubscriptionService();

            // student => user => id
            // course => id

            //Guid userId = Guid.NewGuid();
            //Guid courseId = Guid.NewGuid();

            //CourseSubscriptionTicket ticket = service.SubscribeCourse(userId, courseId);
            //NewsletterSubscriptionTicket ticket = service.SubscribeNewsletter(userId, courseId);
            //OfficeHourSubscriptionTicket ticket = service.SubscribeOffieHourDate(userId, courseId);
            //OfficeHourSubscriptionTicket ticket = service.SubscribeOffieHourSlot(userId, courseId);
            //EventSubscriptionTicket ticket = service.SubscribeEvent(userId, courseId);
            //EventDateSubscriptionTicket ticket = service.SubscribeEventDate(userId, courseId);


            // Bequemlichkeitsfunktion für Statusabfrage
            // hier frage ich ein Detail direkt ab
            //bool state = service.BinIchSchonDrin(userId, courseId);

            // Hier bekomme ich die ganze Funktion
            // SubscriptionState state2 =  service.GetSubscriptionState(userId, courseId);




            // das Ticket soll einen Kurs haben und zwar den richtigen
            // ticket.Course.Name ...

            // Status: wo bin ich
            // ticket.State == Teilnehmer

            // PersonalPlanService pps =  new PersonalPlanService();

            // bool IsSubscribed =  pps.HasSubscriptionFor(student, ticket.Course);

            //Assert.Equals(IsSubscribed, true);

        }

        [Test]
        public void UnsubscribeCourse()
        {

            // Situation
            // Student s möchte sich aus Kurs k wieder austragen
            // Ich brauche
            // Student s ist in Kurs k eingetragen


            // stundent -> user -> id

            Guid userId = Guid.NewGuid();

        }

        [Test]
        public void SubscribeOfficeHour()
        {

            // Situation
            // Student s möchte sich in Sprechstunde ch eintragen
            // Ich brauche
            // Sprechstunde frei
            // Sprechstunde belegt evtl mit Warteliste?



            // student -> user -> id
            // Sprechstunden -> id

            Guid userId = Guid.NewGuid();
            Guid officehourId = Guid.NewGuid();
            


        }



        [Test]
        public void SubscribeEvent()
        {

            // Situation
            // Student s möchte sich in Event e eintragen
            // Ich brauche
            // Event e 
            // Auch Austragen testen



            // student -> user -> id
            // Event -> id

            Guid userId = Guid.NewGuid();
            Guid eventId = Guid.NewGuid();
            

        }

        [Test]
        public void SubscribeMailingList()
        {
            // Situaion
            // Student s möchte sich in MaillingList m eintragen
            // Ich brauche 
            // MailingList m
            // Auch Austragen testen

            // student -> user -> id
            // Mailing List -> id

            Guid userId = Guid.NewGuid();
            Guid mailinglistId = Guid.NewGuid();


        }

        [Test]
        public void SubscribeNewsletter()
        {
            // Situaion
            // Student s möchte sich in Newslettern eintragen
            // Ich brauche
            // Newsletter n
            // Auch Austragen testen

            // student -> user -> id
            // Newsletter -> id

            Guid userId = Guid.NewGuid();
            Guid newsletterId = Guid.NewGuid();
        }

        [Test]
        public void LogInLogOut()
        {

            // Situaion
            // LogIn und LogOut soll überprüft werden
            // Benötigt wird Studend s

            // student -> user -> id

            Guid userId = Guid.NewGuid();

            // LogIn ID & TimeStamp -> vorhanden und durchgeführt
            // LogOut ID & TimeStamp -> vorhanden und durchgeführt

        }

        [Test]
        public void WPMPlatzverlosung()
        {
            // Situation 
            // Belegung/Platzverlosung WPM
            // Weiter Infos benötigt(hab keine genauen Infos)


        }

        [Test]
        public void FreieRäume()
        {
            // Student s sucht einen freien Raum
            // benötigt werden Räume mit und ohne Belegung
            // Ausgegeben werden sollen nur die ohne Belegung für die jeweilige Zeit

            // create new rooms
            // set blocked times
            // search for free rooms
            // bool room is really free
            

        }

    }
}
