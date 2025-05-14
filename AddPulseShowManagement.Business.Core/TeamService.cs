using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Business.Core
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _repository;
        public TeamService(ITeamRepository repository)
        {
            this._repository = repository;
        }

        public List<Teams> GetTeams(long showID = 0, string startDate = "", string endDate = "", string search = "")
        {
            try
            {
                return _repository.GetTeams(showID,startDate,endDate,search);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Teams SaveUpdateTeam(Teams team)
        {
            try
            {
                return _repository.SaveUpdateTeam(team);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public Teams GetTeamDetails(long teamID = 0)
        {
            try
            {
                return _repository.GetTeamDetails(teamID);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public List<Contestants> GetTeamContestants(long teamID = 0)
        {
            try
            {
                return _repository.GetTeamContestants(teamID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Contestants> GetSuggestedContestantList(long teamID = 0)
        {
            try
            {
                return _repository.GetSuggestedContestantList(teamID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool SaveUpdateContestantList(long teamID = 0, string contestantList = "")
        {
            try
            {
                return _repository.SaveUpdateContestantList(teamID, contestantList);
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool VoteOutContestant(long teamContestantID = 0)
        {
            try
            {
                return _repository.VoteOutContestant(teamContestantID);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool VoteinContestant(long teamContestantID = 0)
        {
            try
            {
                return _repository.VoteinContestant(teamContestantID);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
