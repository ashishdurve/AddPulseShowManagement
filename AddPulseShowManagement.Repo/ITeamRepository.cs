using AddPulseShowManagement.Data.DataTableModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Repo
{
    public interface ITeamRepository : IRepository
    {
        public Teams SaveUpdateTeam(Teams team);
        public List<Teams> GetTeams(long showID =0,string startDate="",string endDate="",string search="");
        public Teams GetTeamDetails(long teamID = 0);
        public List<Contestants> GetTeamContestants(long teamID = 0);
        public List<Contestants> GetSuggestedContestantList(long teamID = 0);
        public bool SaveUpdateContestantList(long teamID = 0, string contestantList = "");
        public bool VoteOutContestant(long teamContestantID = 0);
        public bool VoteinContestant(long teamContestantID = 0);
    }
    public interface ITeamService
    {
        public List<Teams> GetTeams(long showID = 0, string startDate = "", string endDate = "", string search = "");
        public Teams SaveUpdateTeam(Teams team);
        public Teams GetTeamDetails(long teamID = 0);
        public List<Contestants> GetTeamContestants(long teamID = 0);
        public List<Contestants> GetSuggestedContestantList(long teamID = 0);
        public bool SaveUpdateContestantList(long teamID = 0, string contestantList = "");
        public bool VoteOutContestant(long teamContestantID = 0);
        public bool VoteinContestant(long teamContestantID = 0);
    }
}
