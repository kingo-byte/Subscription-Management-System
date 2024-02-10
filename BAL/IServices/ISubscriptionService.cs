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
    }
}
