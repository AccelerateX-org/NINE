using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WIQuest.Web.Data
{
    public class QuestDbContext : DbContext
    {
        public QuestDbContext() : base("QuestDb")
        {
            
        }

        public IDbSet<User> Users { get; set; }

        #region Allgemeine Infrastruktur
        public IDbSet<BinaryStorage> BinaryStorages { get; set; }
        #endregion

        #region Fragenkatalog
        public IDbSet<QuestionCategory> Categories { get; set; }

        public IDbSet<Question> Questions { get; set; }

        public IDbSet<QuestionAnswer> Answers { get; set; }
        #endregion

        #region Ausführungen
        public IDbSet<QuestLog> QuestLogs { get; set; }
        #endregion
    }
}