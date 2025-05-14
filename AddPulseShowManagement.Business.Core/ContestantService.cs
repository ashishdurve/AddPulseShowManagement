using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Repo;
using AddPulseShowManagement.Data.ViewModels;
using System.Data;

namespace AddPulseShowManagement.Business.Core
{
    public class ContestantService : IContestantService
    {
        private readonly IContestantRepository _repository;
        public ContestantService(IContestantRepository repository)
        {
            this._repository = repository;
        }

        public List<Contestants> GetContestantList()
        {
            try
            {
                return _repository.GetContestantList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ContestantsView> GetTeamData(long teamID = 0, int isConfigUpdated = 1, int hour = 1)
        {
            try
            {
                return _repository.GetTeamData(teamID, isConfigUpdated, hour);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public List<LivePulseView> GetLivePulseData(long teamID = 0)        
        {
            try
            {
                return _repository.GetLivePulseData(teamID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataSet GetLivePulseTeamData(long teamID = 0)
        {
            try
            {
                return _repository.GetLivePulseTeamData(teamID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<ContestantViewModel> GetContestantsData(long showID = 0, string search = "")
        {
            try
            {
                return _repository.GetContestantsData(showID, search);
            }
            catch (Exception)
            {
                return null;
            }
        }
         public List<PlayerVsCardView> GetPlayerVsPlayerData(long showID = 0, int isConfigUpdated = 1, int hour = 1)        
        {
            try
            {
                return _repository.GetPlayerVsPlayerData(showID, isConfigUpdated, hour);
            }
            catch (Exception)
            {
                return null;
            }
        }
      public bool SaveCardData(long userID=0,long showID=0,long cardID=1,int cardval=4,long teamID=0,long configurationID=0, string unitID = "HH", long timeInseconds = 3600, int timeperiod = 1, int refreshTime = 5)
        {
            try
            {
                return _repository.SaveCardData(userID,showID,cardID,cardval,teamID,configurationID,unitID,timeInseconds,timeperiod, refreshTime);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public LiveData GetContestantLiveGraphData(long contestantID = 0)        
        {
            try
            {
                return _repository.GetContestantLiveGraphData(contestantID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool SavePlayersSequence(int PPid = 1, int Seq = 1, string PlayerIDs = "")
        {
            try
            {
                return _repository.SavePlayersSequence(PPid,Seq,PlayerIDs);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SavePlayerVsPlayer(int noPlayer = 1)
        {
            try
            {
                return _repository.SavePlayerVsPlayer(noPlayer);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
