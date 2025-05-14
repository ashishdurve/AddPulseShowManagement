using AddPulseShowManagement.Common;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Data.ViewModels;
using AddPulseShowManagement.Models;
using AddPulseShowManagement.Repo;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AddPulseShowManagement.Controllers
{
    public class ConfigurationController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly MSSQLDbContext _context;
        private readonly AppConfig _appConfig;
        private readonly ITeamService _teamService;
        private readonly IContestantService _conestantService;
        private readonly IShowService _showService;
    

        public ConfigurationController(ILogger<HomeController> logger,
              MSSQLDbContext context,
            AppConfig appConfig,
            IContestantService conestantService,
            IShowService showService,
            ITeamService teamService)
        {
            this._appConfig = appConfig;
            this._context = context;
            this._logger = logger;
            this._teamService = teamService;
            this._conestantService = conestantService;
            this._showService = showService;
        }


        public IActionResult GetTeamCardView(long showID=0,short cardID =0)
        {
            try
            {

                List<Teams> teams = new List<Teams>();

                teams = _context.Teams.Where(y => y.ShowID == showID).ToList() ?? new List<Teams>();

                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserID").Value) : 0;

                DashboardConfigurations configurations;
                try
                {
                    configurations = _context.DashboardConfigurations.Where(y => y.UserID == UserID).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    configurations = null;
                }
                if(configurations == null)
                    configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                    
                if (configurations != null)
                {
                    configurations.TeamCardID = !string.IsNullOrEmpty(configurations.TeamCardID.ToString()) ? configurations.TeamCardID : 0;
                    configurations.TeamCardID2 = !string.IsNullOrEmpty(configurations.TeamCardID2.ToString()) ? configurations.TeamCardID2 : 0;
                    configurations.TeamCardID3 = !string.IsNullOrEmpty(configurations.TeamCardID3.ToString()) ? configurations.TeamCardID3 : 0;
                }
                if(cardID == 1)
                    ViewBag.TeamCardID = configurations.TeamCardID;
                if (cardID == 2)
                    ViewBag.TeamCardID = configurations.TeamCardID2;
                if (cardID == 3)
                    ViewBag.TeamCardID = configurations.TeamCardID3;
                ViewBag.teams = teams;

                return PartialView();

            }
            catch (Exception ex )
            {
                _logger.LogError(ex, "Error from ShowsController -> ShowDetail action method.", "");
                return View("login", "account");
            }
          
        }


        public IActionResult GetPlayerVsPlayerCardView(long showID = 0)
        {
            try
            {

                List<Teams> teams = new List<Teams>();

                teams = _context.Teams.Where(y => y.ShowID == showID).ToList() ?? new List<Teams>();
                List<Contestants> contestants = new List<Contestants>();

                ViewBag.teams = teams;
                

                return PartialView();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowsController -> ShowDetail action method.", "");
                return View("login", "account");
            }

        }

        public IActionResult Configuration()
        {
            try
            {
                List<Shows> shows = this._showService.GetShows() ?? new List<Shows>();
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserID").Value) : 0;

                DashboardConfigurations configurations;
                try
                {
                    configurations = _context.DashboardConfigurations.Where(y => y.UserID == UserID).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    configurations = null;
                }
                if(configurations== null)
                    configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();

                bool isConfigured = false;
                long ShowID = 0;
                int card1 = 0, card2 = 0, card3 = 0;
                long teamID = 0, teamID2=0,teamID3=0;
                string timeUnit = "HH";
                int timePeriod = 1;
                int refreshTimePeriod = 1;
                if (configurations != null)
                {
                    isConfigured = true;
                    ShowID = configurations.ShowID.HasValue? configurations.ShowID.Value:0;

                    card1 = configurations.Card1.HasValue ? configurations.Card1.Value : 0;
                    card2 = configurations.Card2.HasValue ? configurations.Card2.Value : 0;
                    card3 = configurations.Card3.HasValue ? configurations.Card3.Value : 0;
                    teamID = configurations.TeamCardID.HasValue ? configurations.TeamCardID.Value : 0;
                    teamID2 = configurations.TeamCardID2.HasValue ? configurations.TeamCardID2.Value : 0;
                    teamID3 = configurations.TeamCardID3.HasValue ? configurations.TeamCardID3.Value : 0;
                    timeUnit = !string.IsNullOrEmpty(configurations.TimePeriodUnit) ? configurations.TimePeriodUnit : "HH";
                    timePeriod = configurations.TimePeriod.HasValue ? configurations.TimePeriod.Value : 1;
                    refreshTimePeriod = configurations.RefreshTimePeriod.HasValue ? configurations.RefreshTimePeriod.Value : 1;
                   // teamID = !string.IsNullOrEmpty(configurations.TeamCardID.ToString())? configurations.TeamCardID : 0;
                }
                else
                {
                    card1 = 4;
                    card2 = 4;
                    card3 = 4;
                }

                ViewBag.card1 = card1;
                ViewBag.card2 = card2;
                ViewBag.card3 = card3;
                ViewBag.teamID = teamID;
                ViewBag.teamID2 = teamID2;
                ViewBag.teamID3 = teamID3;
                ViewBag.timeUnit = timeUnit;
                ViewBag.timePeriod = timePeriod;
                ViewBag.refreshTimePeriod = refreshTimePeriod;

                List<CardDescription> cardDescriptions = _context.CardDescription.Where(y => y.IsActive == true).ToList() ?? new List<CardDescription>();

                ViewBag.cardDescriptions = cardDescriptions;
                ViewBag.shows = shows;
                ViewBag.ShowID = ShowID;
                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }


        public IActionResult GetPlayercardView(long showID=0)
        {
            try
            {

                List<long> teamlst = _context.Teams.Where(y => y.ShowID == showID).Select(x => x.TeamID).ToList();
                //new List<long>(Array.ConvertAll(teamIDs.Split(','), long.Parse));
                //,string teamIDs = ""
                string teamIDs = "";
                
                // List<int> cntContestants = new List<int>();
                List<PlayerVSPlayerList> contestantlst = new List<PlayerVSPlayerList>();

                List<Contestants> contestants = new List<Contestants>();
                
                if(teamlst.Count > 0)
                {
                    teamIDs = String.Join(",", teamlst);
                    foreach (long id in teamlst)
                    {
                        List<Contestants> c1 = new List<Contestants>();
                        c1 = _teamService.GetTeamContestants(id) ?? new List<Contestants>();
                        contestants.AddRange(c1);
                    }
                }

                foreach (long teamID in teamlst)
                {
                    int cnt = _context.TeamContestants.Where(y=>y.TeamID == teamID).Count();
                    PlayerVSPlayerList p1 = new PlayerVSPlayerList();
                    p1.TeamID = teamID;
                    p1.contestants = contestants;

                    contestantlst.Add(p1);
                   // cntContestants.Add(cnt);
                }

                //int maxVal = cntContestants.Max();
                int maxVal = contestants.Count;


                List<PlayersSequence> playersSequences = _context.PlayersSequence.Where(y => y.IsActive == true).ToList() ?? new List<PlayersSequence>();
                ViewBag.playersSequences = playersSequences;

                ViewBag.cntteam = teamlst.Count;
                ViewBag.maxVal = maxVal;
                ViewBag.teamIDs = teamIDs;
                ViewBag.contestantlst = contestantlst;
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ConfigurationController -> GetPlayercardView action method.", "");
                return View("login", "account");
            }
        }

    }
}
