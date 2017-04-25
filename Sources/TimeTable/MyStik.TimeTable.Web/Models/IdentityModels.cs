using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using MyStik.TimeTable.Web.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Web.Models
{

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public bool LikeEMails { get; set; }

        /// <summary>
        /// Datum der Registrierung
        /// </summary>
        public DateTime? Registered { get; set; }

        /// <summary>
        /// Datum des letzten Logins
        /// </summary>
        public DateTime? LastLogin { get; set; }

        public MemberState MemberState { get; set; }

        /// <summary>
        /// Wird gesetzt bei der Registrierung bzw. Änderung der E-Mail Adresse (+14 Tage)
        /// Nach Validierung der E-Mail Adresse kein Ablaufdatum
        /// Automatisch bei Prüfung auf "LastLogin" (+90/450 Tage, je nach MemberState guest/student)
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// Datum des Fehlers
        /// </summary>
        public DateTime? Submitted { get; set; }

        /// <summary>
        /// Verwendung als "Eingeladen"
        /// Entkoppelt das Einlesen vom Einladen, d.h. Einladungsdateien
        /// können mehrfach eingelesen werden
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Verwendung als "Wer hat eingeladen"?
        /// UserId
        /// </summary>
        public string Faculty { get; set; }


        /// <summary>
        /// Wenn gesetzt, dann Datum der Vorwarnung
        /// </summary>
        public DateTime? Approved { get; set; }


#region veraltet

        /// <summary>
        /// Telefonnummern des Benutzers
        /// </summary>
        public string Phone { get; set; }

        public string Street { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }



        public bool? EmailValidated { get; set; }


        /// <summary>
        /// Studiengang => sollte nicht mehr verwendet werden
        /// </summary>
        public string Curriculum { get; set; }

        /// <summary>
        /// Studiengruppe (Curriculum) => sollte nicht mehr verwendet werden
        /// </summary>
        public string Group { get; set; }


        public bool? IsSpamer { get; set; }

        public bool IsAnonymous { get; set; }


#endregion

        public virtual ICollection<UserDevice> Devices { get; set; }


        public string AccountErrorMessage
        {
            get
            {
                if (Submitted != null)
                {
                    return string.Format("{0} ({1})", Remark, Submitted.Value.ToShortDateString());
                }
                else
                {
                    return Remark;
                }
            }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public IDbSet<UserDevice> Devices { get; set; }
    }


    /// <summary>
    /// Einordnung des Kontos
    /// </summary>
    public enum MemberState
    {
        /// <summary>
        /// Gäste: dürfen sich nur in Sprechstunden eintragen
        /// Automatisch jeder, der sich mit einer beliebigen E-Mail Adresse angemeldet hat
        /// </summary>
        Guest = 0,

        /// <summary>
        /// Automatisch jeder, der sich mit hm.edu E-Mail Adresse angemeldet hat
        /// </summary>
        Student = 1,

        /// <summary>
        /// Mitarbeiter, LBs, Profs etc.
        /// Wird manuell zugewiesen, bei Aufnahme in eine nicht studentische Organisation
        /// </summary>
        Staff = 2
    }

    public enum DevicePlatform
    {
        IOS = 1,
        Android = 2,
        WinPhone = 3
    }

    public class UserDevice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Id des Devices für Plattform
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Name des Devices
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Falls es auf Grund der Plattform unterschiedliche Versandwege geben sollte
        /// </summary>
        public DevicePlatform Platform { get; set; }

        /// <summary>
        /// Datum der Registrierung
        /// </summary>
        public DateTime Registered { get; set; }

        /// <summary>
        /// Notifications an das gerät senden
        /// </summary>
        public bool IsActive { get; set; }

        public virtual ApplicationUser User { get; set; }
    }


}