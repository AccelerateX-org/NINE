using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text;

namespace MyStik.TimeTable.Web.Models
{

    /// <summary>
    /// 
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool LikeEMails { get; set; }

        /// <summary>
        /// Datum der Registrierung
        /// </summary>
        public DateTime? Registered { get; set; }

        /// <summary>
        /// Datum des letzten Logins
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MemberState MemberState { get; set; }

        /// <summary>
        /// Wird gesetzt bei der Registrierung bzw. Änderung der E-Mail Adresse (+14 Tage)
        /// Nach Validierung der E-Mail Adresse kein Ablaufdatum
        /// Automatisch bei Prüfung auf "LastLogin" (+90/450 Tage, je nach MemberState guest/student)
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Datum des Fehlers
        /// </summary>
        public DateTime? Submitted { get; set; }

        /// <summary>
        /// Aktives Konto, erhält E-Mails
        /// Wird durch automatische Prozess und 
        /// SysAdmin auf false gesetzt
        /// Wird bei jedem Login auf true gesetzt
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

        /// <summary>
        /// Akademischer Grad
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Der Datentyp
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Das Prfilbild
        /// </summary>
        // Das ist erforderlich, sonst geht es z.B. in SQL-Server CE nicht
        // bzw. dort wird 4000 als maximale Länge automatisch angenommen!
        [MaxLength]
        public byte[] BinaryData { get; set; }


        #region veraltet

        /// <summary>
        /// Telefonnummern des Benutzers
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Country { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public bool? EmailValidated { get; set; }


        /// <summary>
        /// Studiengang => sollte nicht mehr verwendet werden
        /// </summary>
        public string Curriculum { get; set; }

        /// <summary>
        /// Studiengruppe (Curriculum) => sollte nicht mehr verwendet werden
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsSpamer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAnonymous { get; set; }


#endregion

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<UserDevice> Devices { get; set; }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public string FullName
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(LastName);

                if (!string.IsNullOrEmpty(FirstName))
                {
                    sb.AppendFormat(", {0} ", FirstName);
                }

                if (!string.IsNullOrEmpty(Title))
                {
                    sb.AppendFormat(" ({0})", Title);
                }

                return sb.ToString();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbSet<UserDevice> Devices { get; set; }
     
        /// <summary>
        /// 
        /// </summary>
        public IDbSet<UserContact> Contacts { get; set; }
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

    /// <summary>
    /// 
    /// </summary>
    public enum DevicePlatform
    {
        /// <summary>
        /// 
        /// </summary>
        IOS = 1,
        /// <summary>
        /// 
        /// </summary>
        Android = 2,
        /// <summary>
        /// 
        /// </summary>
        WinPhone = 3,
        /// <summary>
        /// 
        /// </summary>
        PWA = 4
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserDevice
    {
        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public virtual ApplicationUser User { get; set; }
    }

    /// <summary>
    /// Kontaktdaten eines Benutzers
    /// </summary>
    public class UserContact
    {
        /// <summary>
        /// Datenbankschlüssel
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// E-Mail Adresse
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// Status
        ///   true: E-Mail Adresse ist validiert. An diese Adresse werden E-Mails versendet
        ///   false: E-Mail Adresse ist nicht validiert. An diese Adresse werden keine E-Mails versendet
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// Fehlermeldung
        /// Wird gesetzt sobald beim Versand an diese Adresse ein Fehler auftritt.
        /// </summary>
        public string LastErrorMessage { get; set; }
    }


}