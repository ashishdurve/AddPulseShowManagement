using AddPulseShowManagement.Data.DBModels;

namespace AddPulseShowManagement.Repo
{
    public interface IRepository
    {
        public MSSQLDbContext dbContext();
    }
}
