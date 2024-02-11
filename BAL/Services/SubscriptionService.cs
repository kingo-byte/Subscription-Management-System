using BAL.IServices;
using DAL.Repository;
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
        private readonly SubscriptionManagementDbConext _db;
        public SubscriptionService(DbAccess dbAccess, SubscriptionManagementDbConext db)
        {
            _dbAccess = dbAccess;
            _db = db;
        }

        public List<Subscription> GetActiveSubscriptions(int userId)
        {
            return _dbAccess.GetActiveSubscriptions(userId);
        }

        public int GetRemainingDays(int subscriptionId) 
        {
            var response = _db.Subscriptions.Find(subscriptionId);

            if(response == null) 
            {
                int remainingDays = (int)(response!.EndDate - response.StartDate).TotalDays;

                return remainingDays;
            }

            return -1;
        }
    }
}
