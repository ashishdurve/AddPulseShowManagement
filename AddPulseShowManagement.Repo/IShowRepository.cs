using AddPulseShowManagement.Data.DataTableModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Repo
{
    public interface IShowRepository: IRepository
    {
        public List<Shows> GetShows();
        public List<Shows> GetShowList(string startdate="",string endate="");
        public Shows SaveUpdateShow(Shows show);
        public List<Teams> GetTeamsOfShow(long showID = 0);
    }

    public interface IShowService
    {
        public List<Shows> GetShows();
        public Task<List<Shows>> GetShowList(string startdate = "", string endate = "");
        public Shows SaveUpdateShow(Shows show);
        public List<Teams> GetTeamsOfShow(long showID = 0);
    }
}
