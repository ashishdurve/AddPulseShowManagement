using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Repo
{
    public interface IContestantRepository : IRepository
    {
        public List<Contestants> GetContestantList();
        public List<ContestantsView> GetTeamData(long teamID = 0, int isConfigUpdated = 1, int hour = 1);
        public List<LivePulseView> GetLivePulseData(long showID = 0);

        public DataSet GetLivePulseTeamData(long teamID = 0);
        public List<ContestantViewModel> GetContestantsData(long showID = 0, string search = "");
        public List<PlayerVsCardView> GetPlayerVsPlayerData(long showID = 0, int isConfigUpdated = 1, int hour = 1);
        public bool SaveCardData(long userID = 0, long showID = 0, long cardID = 1, int cardval = 4, long teamID = 0, long configurationID = 0, string unitID = "HH", long timeInseconds = 3600, int timeperiod = 1, int refreshTime = 5);
        public LiveData GetContestantLiveGraphData(long contestantID = 0);
        public bool SavePlayersSequence(int PPid = 1, int Seq = 1, string PlayerIDs = "");
        public bool SavePlayerVsPlayer(int noPlayer = 1);
    }
    public interface IContestantService
    {
        public List<Contestants> GetContestantList();
        public List<ContestantsView> GetTeamData(long teamID = 0, int isConfigUpdated = 1, int hour = 1);
        public List<LivePulseView> GetLivePulseData(long showID = 0);

        public DataSet GetLivePulseTeamData(long showID = 0);
        public List<ContestantViewModel> GetContestantsData(long showID = 0, string search = "");
        public List<PlayerVsCardView> GetPlayerVsPlayerData(long showID = 0, int isConfigUpdated = 1, int hour = 1);
        public bool SaveCardData(long userID = 0, long showID = 0, long cardID = 1, int cardval = 4, long teamID = 0, long configurationID = 0, string unitID = "HH", long timeInseconds = 3600, int timeperiod = 1, int refreshTime = 5);
        public LiveData GetContestantLiveGraphData(long contestantID = 0);
        public bool SavePlayersSequence(int PPid = 1, int Seq = 1, string PlayerIDs = "");
        public bool SavePlayerVsPlayer(int noPlayer = 1);
    }
}
