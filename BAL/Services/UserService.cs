using BAL.IServices;
using DAL.Repository;
using DAL.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services
{
    public class UserService : IUserService
    {
        private readonly SubscriptionManagementDbConext _db;

        public UserService(SubscriptionManagementDbConext db) 
        {
            _db = db;
        }

        public User AddUser(User user)
        {
            User newUser = _db.Users.Add(user).Entity;

            _db.SaveChanges();

            return newUser;
        }
    }
}
