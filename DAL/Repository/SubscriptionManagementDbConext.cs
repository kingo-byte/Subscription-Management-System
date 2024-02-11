using DAL.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class SubscriptionManagementDbConext : DbContext
    {
        public SubscriptionManagementDbConext(DbContextOptions<SubscriptionManagementDbConext> options) : base(options) 
        {
            
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }

        public DbSet<Logging> Logging { get; set; }
    }
}
