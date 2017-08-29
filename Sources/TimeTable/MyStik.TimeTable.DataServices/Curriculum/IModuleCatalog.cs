using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Curriculum
{
    public interface IModuleCatalog
    {
        /// <summary>
        /// Kurzname des Veranstalters, z.B. "FK 09"
        /// </summary>
        string Organiser { get; }

        /// <summary>
        /// Kurzname des Studiengangs, z.B. "WI"
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// Liefert die Liste aller Module mit Lehrveranstaltungen und Prüfungsleistungen
        /// </summary>
        /// <param name="options">Noch zu definierende Optionen, z.B. nur Studienrichtung etc.</param>
        /// <returns></returns>
        ICollection<CurriculumModule> GetModules(string options);
    }
}
