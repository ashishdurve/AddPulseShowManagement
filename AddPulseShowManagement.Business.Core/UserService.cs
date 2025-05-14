using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Business.Core
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository userRepository)
        {
            this._repository = userRepository;
        }

        public async Task<List<Users>> GetManagerUsers()
        {
            try
            {
                return await Task.Run(() => _repository.GetManagerUsers());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Users> SaveUpdateUsers(Users users)
        {
            try
            {
                return await Task.Run(() => _repository.SaveUpdateUsers(users));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
