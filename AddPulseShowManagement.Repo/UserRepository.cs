using AddPulseShowManagement.Data.DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Repo
{
    public class UserRepository : BaseRepository<MSSQLDbContext>, IUserRepository
    {
        public UserRepository(MSSQLDbContext context) : base(context)
        {

        }
        public new MSSQLDbContext dbContext() => base.dbContext;
        public List<Users> GetManagerUsers()
        {
            try
            {
                var users = this.ExecuteRows<Users>(CommandType.StoredProcedure, "APS_01_GetManagerList")
                    ?? new List<Users>();
                return users;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Users SaveUpdateUsers(Users users)
        {
            try
            {
                var userChk = this.dbContext().Users.Find(users.UserID);
                Users user = userChk ?? new Users();
                user.ModifiedDate = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(users.ProfileImage))
                {
                    user.ProfileImage = users.ProfileImage;
                }
                if (userChk != null)
                {
                    var local = this.dbContext().Set<Users>()
                                        .Local
                                        .FirstOrDefault(entry => entry.UserID.Equals(users.UserID));

                    // check if local is not null  
                    if (local != null)
                    {
                        // detach
                        this.dbContext().Entry(local).State = EntityState.Detached;
                    }

                    this.dbContext().Entry(users).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    this.dbContext().Users.Add(users);
                }
                this.dbContext().SaveChanges();

                return users;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
