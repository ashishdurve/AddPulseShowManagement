using AddPulseShowManagement.Data.DataTableModels;
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
    public class TeamRepository: BaseRepository<MSSQLDbContext>, ITeamRepository
    {
        private readonly MSSQLDbContext _context;
        public TeamRepository(MSSQLDbContext context) : base(context)
        {
            _context = context;
        }
        public new MSSQLDbContext dbContext() => base.dbContext;

       
        public List<Teams> GetTeams(long showID = 0, string startDate = "", string endDate = "", string search = "")
        {
            try
            {
                var teams = this.ExecuteRows<Teams>(CommandType.StoredProcedure, "APS_01_GetTeamList",
                    ("@ShowID", showID),
                    ("@StartDate", startDate),
                    ("@EndDate", endDate),
                    ("@Search", search))?? new List<Teams>();
                return teams;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Teams SaveUpdateTeam(Teams team)
        {
            try
            {
                var teamChk = this.dbContext().Teams.Find(team.TeamID);
                Teams team1 = teamChk ?? new Teams();
                
                if (teamChk != null)
                {
                    if(string.IsNullOrEmpty(team.CreatedDate.ToString()))
                    {
                        team.CreatedDate = teamChk.CreatedDate;
                    }

                    if (string.IsNullOrEmpty(team.CreatedBy.ToString()))
                    {
                        team.CreatedBy = teamChk.CreatedBy;
                    }

                    var local = this.dbContext().Set<Teams>()
                                        .Local
                                        .FirstOrDefault(entry => entry.TeamID.Equals(team.TeamID));

                    // check if local is not null  
                    if (local != null)
                    {
                        // detach
                        this.dbContext().Entry(local).State = EntityState.Detached;
                    }

                    this.dbContext().Entry(team).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    this.dbContext().Teams.Add(team);
                }
                this.dbContext().SaveChanges();

                return team;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public Teams GetTeamDetails(long teamID=0)
        {
            try
            {
                var teams = this.ExecuteRows<Teams>(CommandType.StoredProcedure, "APS_01_GetTeamDetails",
                    ("@TeamID", teamID)).FirstOrDefault() ?? new Teams();
                return teams;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<Contestants> GetTeamContestants(long teamID = 0)
        {
            try
            {
                var contestants = this.ExecuteRows<Contestants>(CommandType.StoredProcedure, "APS_01_GetTeamContestants",
                    ("@TeamID", teamID)) ?? new List<Contestants>();
                return contestants;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<Contestants> GetSuggestedContestantList(long teamID = 0)
        {
            try
            {
                var contestants = this.ExecuteRows<Contestants>(CommandType.StoredProcedure, "APS_01_GetSuggestedContestantList",
                    ("@TeamID", teamID)) ?? new List<Contestants>();
                return contestants;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        
        public bool SaveUpdateContestantList(long teamID=0,string contestantList = "")
        {
            try
            {
               this.ExecuteNonQuery(CommandType.StoredProcedure, "APS_01_SaveUpdateContestantList",
                    ("@TeamID", teamID),
                    ("@ContestantIDs", contestantList));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
                
        public bool VoteOutContestant(long teamContestantID = 0)
        {
            try
            {
                this.ExecuteNonQuery(CommandType.StoredProcedure, "APS_01_VoteOutContestant",
                     ("@TeamContestantID", teamContestantID));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool VoteinContestant(long teamContestantID = 0)
        {
            try
            {
                this.ExecuteNonQuery(CommandType.StoredProcedure, "APS_01_VoteInContestant",
                     ("@TeamContestantID", teamContestantID));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
