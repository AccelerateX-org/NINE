using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Test
{
    public class TestDbInitializer : DropCreateDatabaseAlways<TimeTableDbContext>
    {
        protected override void Seed(TimeTableDbContext context)
        {
            // das ist das Objekt
            Course c = new Course();
            c.Name = "TM";
            c.ShortName = "TM";
            c.Occurrence = new Occurrence();
            c.Occurrence.Capacity = 3;
            c.Occurrence.Subscriptions.Add(new OccurrenceSubscription());


            // jetzt wird das Objekt der "Tabelle" zugeordnet
            context.Activities.Add(c);

            // jetzt wird in die Datenbank geschrieben
            context.SaveChanges();


            base.Seed(context);
        }
    }
}
