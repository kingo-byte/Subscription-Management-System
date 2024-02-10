using DAL.Repository.Models;
using DAL.Repository.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.IServices
{
    public interface IUserService
    {
        public User AddUser(User user);
    }
}
