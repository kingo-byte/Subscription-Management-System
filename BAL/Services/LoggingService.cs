using BAL.IServices;
using DAL.Repository;
using DAL.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly SubscriptionManagementDbConext _db;
        public LoggingService(SubscriptionManagementDbConext db)
        {
            _db = db;
        }

        public void Log(string message)
        {
            Logging log = new Logging()
            {
                Description = message,
                CreationDate = DateTime.UtcNow,
            };

            _db.Logging.Add(log);
            _db.SaveChanges();
        }
    }
}
