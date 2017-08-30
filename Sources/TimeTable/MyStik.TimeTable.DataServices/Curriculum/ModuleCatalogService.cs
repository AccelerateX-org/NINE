using System.Collections.Generic;
using MyStik.TimeTable.DataServices.Curriculum.FK09_WI;

namespace MyStik.TimeTable.DataServices.Curriculum
{
    public class ModuleCatalogService
    {
        /// <summary>
        /// Gesamtheit aller (jetzt: hardcodierten) vorhandenen /
        /// (geplant: über PlugIn-Infrastruktur registierte) verfügabren Modulkataloge
        /// </summary>
        /// <returns>Liste alle Modulkataloge</returns>
        public ICollection<IModuleCatalog> GetAllCatalogs()
        {
            var allCatalogs = new List<IModuleCatalog>
            {
                new ModuleCatalogFK09WI()
            };


            return allCatalogs;
        }

        /// <summary>
        /// Explizite Suche
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="currName"></param>
        /// <returns></returns>
        public IModuleCatalog GetCatalog(string orgName, string currName)
        {
            if (string.Equals(orgName, "FK 09") && string.Equals(currName, "WI"))
            {
                return new ModuleCatalogFK09WI();
            }

            return null; // oder Exception werfen!
        }
    }
}
