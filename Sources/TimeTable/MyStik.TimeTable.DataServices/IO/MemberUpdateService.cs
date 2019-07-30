using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.IO
{
    public class MemberUpdate
    {
        public OrganiserMember Member { get; set; }

        public string NewShortName { get; set; }

    }

    /// <summary>
    /// Wartung der Dozentenkürzel, v.a. Untis
    /// Generische Datei mit Aufbau
    /// altes Kürzel | neues Kürzel
    /// Es werden nur vorhandene Namen aktualisiert
    /// Es werden keine neuen Member angelegt => das passiert weiterhin beim IMport
    /// </summary>
    public class MemberUpdateService
    {
        private readonly char seperator = ';';

        private BaseImportContext ctx = new BaseImportContext();

        private TimeTableDbContext _db = new TimeTableDbContext();

        private ActivityOrganiser _org;

        private List<MemberUpdate> _member = new List<MemberUpdate>();

        public MemberUpdateService(string orgName)
        {
            _org = _db.Organisers.SingleOrDefault(x => x.ShortName.Equals(orgName));

            foreach (var member in _org.Members)
            {
                var update = new MemberUpdate
                {
                    Member = member
                };
                _member.Add(update);
            }

        }

        public BaseImportContext Context
        {
            get { return ctx; }
        }


        private string[] GetFileContent(string directory, string gpuFile)
        {
            var path = Path.Combine(directory, gpuFile);

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(Path.Combine(directory, gpuFile), Encoding.Default);
                ctx.AddErrorMessage(gpuFile, string.Format("Anzahl Einträge: {0}", lines.Length), false);
                return lines;
            }

            ctx.AddErrorMessage(gpuFile, string.Format("Datei {0} nicht vorhanden", gpuFile), true);
            return new string[] { };
        }


        public void ReadFile(string directory, string fileName)
        {
            var lines = GetFileContent(directory, fileName);


            var i = 0;
            foreach (var line in lines)
            {
                if (i > 0)
                {
                    // Zeile aufspalten
                    var words = line.Split(seperator);

                    var oldName = words[0].Trim();
                    var newName = words[1].Trim();


                    if (string.IsNullOrEmpty(oldName))
                    {
                        // Eintrag muss neu sein
                        // nach neuem Namen suchen, ob es ihn nicht schon gibt
                        var member = _member.SingleOrDefault(x => x.Member.ShortName.Equals(newName));

                        // wenn es ihn gibt, dann einen Fehler melden
                        if (member != null)
                        {
                            ctx.AddErrorMessage(fileName, $"Diesen neuen Namen gibt es schon: {newName}", true);
                        }
                        else
                        {
                            // das wird eine neuer Member
                        }
                    }
                    else
                    {
                        // nach altem Namen suchen
                        var member = _member.SingleOrDefault(x => x.Member.ShortName.Equals(oldName));

                        // Fall 1 - es gibt den Member zu dem alten Kürzel
                        if (member != null)
                        {
                            // neuen Namen speichern
                            member.NewShortName = newName;
                        }
                        else
                        {
                            // alten Namen gibt es nicht => wird beim Import angelegt
                            // 
                            ctx.AddErrorMessage(fileName, $"Diesen alten Namen gibt es nicht: {oldName} - wird beim Import angelegt", false);
                        }
                    }
                }

                i++;
            }

            // jetzt is alles gelesen
            // neue Namen übertragen
            foreach (var member in _member)
            {
                if (!string.IsNullOrEmpty(member.NewShortName))
                {
                    member.Member.ShortName = member.NewShortName;
                }
            }

            // nach doppelten suchen
            var n1 = _member.OrderBy(x => x.Member.ShortName).Distinct().Count();
            var n2 = _member.Count;

            if (n1 == n2)
            {
                // speichern
                _db.SaveChanges();
            }
            else
            {
                ctx.AddErrorMessage(fileName, "Doppelte Bezeichnungen", true);
            }
        }
    }
}