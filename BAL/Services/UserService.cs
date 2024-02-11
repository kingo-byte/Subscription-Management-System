using BAL.IServices;
using DAL.Repository;
using DAL.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        public User CheckUser(User user)
        {
            User foundUser = _db.Users
               .FirstOrDefault(u => u.UserName.Equals(user.UserName))!;

            return foundUser;
        }

        public bool RemoveUser(int id) {
            var foundUser = _db.Users.Find(id);

            if (foundUser != null)
            {
                _db.Users.Remove(foundUser);
                _db.SaveChanges();
                return true;
            }

            return false;
        }

        public User UpdateUser(User user)
        {
            if (user != null)
            {
                var foundUser = _db.Users.Find(user.Id);

                if (foundUser != null)
                {
                   
                    foundUser.UserName = user.UserName;
                    foundUser.Email = user.Email;
                    foundUser.DateOfBirth = user.DateOfBirth;
                    foundUser.Nationality = user.Nationality;

                    _db.Entry(foundUser).State = EntityState.Modified;

                    _db.SaveChanges();

                    return foundUser;
                }
                
            }
            return user!;
            
        }
    }
}
