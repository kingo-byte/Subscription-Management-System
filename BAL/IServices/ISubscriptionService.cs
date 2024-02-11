using DAL.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.IServices
{
    public interface ISubscriptionService 
    {
        public List<Subscription> GetActiveSubscriptions(int userId);
        public int GetRemainingDays(int susubscriptionId);
        public Subscription AddSubscription(Subscription subscription);
        public bool RemoveSubscription(int id);
        public bool DeactivateSubscription(int Id);
        public bool ActivateSubscription(int Id);
        public Subscription UpdateSubscription(Subscription subscription);
    }
}
