using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.EntityFrameworkCore;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.ViewModels;

namespace AddPulseShowManagement.Repo
{
    public class ContestantRepository:BaseRepository<MSSQLDbContext>, IContestantRepository
    {
        private readonly MSSQLDbContext _context;
        public ContestantRepository(MSSQLDbContext context) : base(context)
        {
            _context = context;
        }
        public new MSSQLDbContext dbContext() => base.dbContext;

        public List<Contestants> GetContestantList()
        {
            try
            {
                var list = _context.Contestants.Where(y=>y.IsActive == true).ToList();
                //var users = this.ExecuteRows<Contestants>(CommandType.StoredProcedure, "APS_01_GetContestantList")
                //    ?? new List<Contestants>();
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //get data for dashboard team with graph
        public List<ContestantsView> GetTeamData(long teamID = 0, int isConfigUpdated = 1, int hour = 1)
        {
            try
            {
                DateTime currentdt = DateTime.UtcNow;
                //DateTime currentdt = new DateTime(2023,02,10,12,35,00);
                //2023-02-10 12:38:41.550
                var contestants = this.ExecuteRows<ContestantsView>(CommandType.StoredProcedure, "APS_01_GetShowTeamListWithGraph",
                    ("@TeamID", teamID),
                    ("@isConfigUpdated", isConfigUpdated),
                    ("@date", currentdt),
                    ("@hour", hour)) ?? new List<ContestantsView>();
                return contestants;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<LivePulseView> GetLivePulseData(long showID = 0)
        {
            try
            {
                var contestants = this.ExecuteRows<LivePulseView>(CommandType.StoredProcedure, "APS_01_GetLivePulseData",
                    ("@showID", showID)) ?? new List<LivePulseView>();
                return contestants;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet GetLivePulseTeamData(long teamID = 0)
        {
            try
            {
                DataSet contestants = this.ExecuteDataSet(CommandType.StoredProcedure, "APS_01_GetTeamLivePulse",
                    ("@TeamID", teamID));
                return contestants;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<ContestantViewModel> GetContestantsData(long showID=0,string search="")
        {
            try
            {
                var contestants = this.ExecuteRows<ContestantViewModel>(CommandType.StoredProcedure, "APS_01_GetContestantsData",
                    ("@search", search), ("@ShowID", showID)) ?? new List<ContestantViewModel>();
                return contestants;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<PlayerVsCardView> GetPlayerVsPlayerData(long showID = 0, int isConfigUpdated=1, int hour = 1)
        {
            try
            {
                DateTime currentdt = DateTime.UtcNow;
                var contestants = this.ExecuteRows<PlayerVsCardView>(CommandType.StoredProcedure, "APS_01_GetPlayerVsPlayerData",
                    ("@ShowID", showID),
                    ("@isConfigUpdated", isConfigUpdated),
                    ("@date", currentdt),
                    ("@hour", hour)) ?? new List<PlayerVsCardView>();
                return contestants;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public bool SaveCardData(long userID=0,long showID=0,long cardID=1,int cardval=4,long teamID=0,long configurationID=0, string unitID = "HH", long timeInseconds = 3600, int timeperiod = 1, int refreshTime = 5)
        {
            try
            {                
                //refresh time is in seconds

                this.ExecuteNonQuery(CommandType.StoredProcedure, "APS_01_SaveUpdateConfiguration",
                                 ("@userID", userID),
                                 ("@cardID", cardID),
                                 ("@teamID", teamID),
                                 ("@showID", showID),
                                 ("@cardval", cardval),
                                 ("@configurationID", configurationID),
                                 ("@unitID", unitID),
                                 ("@timeInseconds", timeInseconds),
                                 ("@timeperiod", timeperiod),
                                 ("@refreshTime", refreshTime));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public LiveData GetContestantLiveGraphData(long contestantID=0)
        {
            try
            {
                var contestants = this.ExecuteRows<LiveData>(CommandType.StoredProcedure, "APS_01_GetContestantLiveGraphData",
                    ("@ContestantID", contestantID)).FirstOrDefault() ?? new LiveData();
                return contestants;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public bool SavePlayerVsPlayer(int noPlayer=1)
        {
            try
            {
                this.ExecuteNonQuery(CommandType.StoredProcedure, "APS_01_SavePlayerVsPlayers",
                                 ("@Players", noPlayer));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SavePlayersSequence(int PPid = 1,int Seq=1,string PlayerIDs="")
        {
            try
            {
                this.ExecuteNonQuery(CommandType.StoredProcedure, "APS_01_SavePlayersSequence",
                                 ("@PPid", PPid),
                                 ("@Seq", Seq),
                                 ("@PlayerIDs", PlayerIDs));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
