using AddPulseShowManagement.Data.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Repo
{
    public interface IUserRepository:IRepository
    {
        public List<Users> GetManagerUsers();
        public Users SaveUpdateUsers(Users users);
    }
    public interface IUserService
    {
        public Task<List<Users>> GetManagerUsers();
        public Task<Users> SaveUpdateUsers(Users users);
    }
}
