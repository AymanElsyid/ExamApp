using Domain;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bl.services
{
    public interface IClsUser
    {
        ApplicationUser GetData(string UserName);
        string GetUserId(string UserName);
        ApplicationUser GetUserById(string Id);

    }
    public class ClsUser : IClsUser
    {
        private readonly  UserManager<ApplicationUser> userManager;
        private readonly  ExamAppDbContext db;
        public ClsUser(UserManager<ApplicationUser> _userManager, ExamAppDbContext _db)
        {
            db= _db;
            userManager = _userManager;
        }
        public ApplicationUser GetData(string UserName)
        {
            try
            {
                var User = db.users.FirstOrDefault(x=>x.UserName==UserName);
                return User ==null?new ApplicationUser():User;
            }
            catch
            {
                return new ApplicationUser();
            }
        }

        public ApplicationUser GetUserById(string Id)
        {
            try { 
            var User=userManager.Users.FirstOrDefault(x=>x.Id==Id);
                return User??new ApplicationUser();
            }
            catch
            {
                return new ApplicationUser();
            }
        }

        public string GetUserId(string UserName)
        {
            try
            {
                var User = db.users.FirstOrDefault(x => x.UserName == UserName);
                return User == null ? string.Empty:User.Id;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
