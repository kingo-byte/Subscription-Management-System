using BAL.IServices;
using DAL.Repository;
using DAL.Repository.DbAccess;
using DAL.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
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

            if (response == null)
            {
                int remainingDays = (int)(response!.EndDate - response.StartDate).TotalDays;

                return remainingDays;
            }

            return -1;
        }

        public Subscription AddSubscription(Subscription subscription)
        {
            var addedSubscription = _db.Subscriptions.Add(subscription).Entity;

            _db.SaveChanges();

            return addedSubscription;
        }

        public bool RemoveSubscription(int id)
        {
            var foundSubscription = _db.Subscriptions.Find(id);

            if (foundSubscription != null)
            {
                _db.Subscriptions.Remove(foundSubscription);
                _db.SaveChanges();
                return true;
            }

            return false;
        }

        public bool DeactivateSubscription(int Id)
        {
            var foundSubscription = _db.Subscriptions.Find(Id);
            if (foundSubscription != null)
            {
                foundSubscription.IsActive = false;
                _db.SaveChanges();
                return true;
            }
            return false;
        }
        public bool ActivateSubscription(int Id)
        {
            var foundSubscription = _db.Subscriptions.Find(Id);
            if (foundSubscription != null)
            {
                foundSubscription.IsActive = true;
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        public Subscription UpdateSubscription(Subscription subscription)
        {
            if (subscription != null)
            {
                var response = _db.Subscriptions.Update(subscription).Entity;
                _db.SaveChanges();
                return response;
            }
            return subscription!;
        }
    }
}
