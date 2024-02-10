using BAL.IServices;
using DAL.Repository.DbAccess;
using DAL.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly DbAccess _dbAccess;
        public SubscriptionService(DbAccess dbAccess) 
        {
            _dbAccess = dbAccess;   
        }    
        public List<Subscription> GetActiveSubscriptions(int userId)
        {
            return _dbAccess.GetSubscriptions(userId);
        }
    }
}
