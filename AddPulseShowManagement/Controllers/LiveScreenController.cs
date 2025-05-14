using AddPulseShowManagement.Common;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Data.ViewModels;
using AddPulseShowManagement.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Data;

namespace AddPulseShowManagement.Controllers
{
    public class LiveScreenController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MSSQLDbContext _context;
        private readonly AppConfig _appConfig;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IShowService _showService;
        private readonly IContestantService _conestantService;

        public LiveScreenController(ILogger<HomeController> logger,
            MSSQLDbContext context,
            AppConfig appConfig,
            IUserService userService,
            ITeamService teamService,
            IContestantService conestantService,
            IShowService showService)
        {
            this._appConfig = appConfig;
            this._context = context;
            this._logger = logger;
            this._userService = userService;
            this._teamService = teamService;
            this._showService = showService;
            this._conestantService = conestantService;
        }

        public IActionResult Index()
        {
            try
            {
                var teamlst = this._teamService.GetTeams() ?? new List<Teams>();
                ViewBag.Teams = teamlst;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetManagers POSt action method.", "");
                return View("login", "account");
            }

           
        }

        public IActionResult GetLivePulse(long teamID=0)
        {
            try
            {
                LivePulse livePulses = new LivePulse();
                long liveHighest=0, liveLowest=0, liveAverage = 0, sessionHighest =0,sessionLowest=0,sessionAverage=0;

                Teams team = this._teamService.GetTeamDetails(teamID);
                //List<ContestantsView> cv = this._conestantService.GetTeamData(teamID) ?? new List<ContestantsView>();
                DataSet ds = this._conestantService.GetLivePulseTeamData(teamID) ?? new DataSet();



                if (team.ShowName == "Flukten fra Akershus Festning")
                    livePulses.imageName = "/Images/livescreen11.png";
                else if (team.ShowName == "Tors Hammer")
                    livePulses.imageName = "/Images/livescreen13.png";
                else if(team.ShowName == "71 grader Nord" || team.ShowName == "Lindesnes")
                    livePulses.imageName = "/Images/IMG_8438.webp";
                else
                    livePulses.imageName = "/Images/livescreen12.png";


                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        long.TryParse(ds.Tables[0].Rows[0]["Highest"].ToString(),out  liveHighest);
                        long.TryParse(ds.Tables[0].Rows[0]["Average"].ToString(), out liveAverage);
                        long.TryParse(ds.Tables[0].Rows[0]["Lowest"].ToString(), out liveLowest);
                    }

                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        long.TryParse(ds.Tables[1].Rows[0]["Highest"].ToString(), out sessionHighest);                        
                        long.TryParse(ds.Tables[1].Rows[0]["Average"].ToString(), out sessionAverage);
                        long.TryParse(ds.Tables[1].Rows[0]["Lowest"].ToString(), out sessionLowest);
                    }

                    if (ds.Tables[2].Rows.Count > 0)
                    {
                        bool paused = false, stopped = false;
                        int minutes = 0, seconds = 0, minutesLeft = 0, secondsLeft = 0;
                        DateTime modifiedDate = DateTime.Now;

                        bool.TryParse(ds.Tables[2].Rows[0]["Paused"].ToString(), out paused);
                        bool.TryParse(ds.Tables[2].Rows[0]["Stopped"].ToString(), out stopped);
                        int.TryParse(ds.Tables[2].Rows[0]["Minutes"].ToString(), out minutes);
                        int.TryParse(ds.Tables[2].Rows[0]["Seconds"].ToString(), out seconds);
                        int.TryParse(ds.Tables[2].Rows[0]["MinutesLeft"].ToString(), out minutesLeft);
                        int.TryParse(ds.Tables[2].Rows[0]["SecondsLeft"].ToString(), out secondsLeft);
                        DateTime.TryParse(ds.Tables[2].Rows[0]["ModifiedDate"].ToString(), out modifiedDate);

                        if (stopped == true)
                            livePulses.timer = (minutesLeft).ToString("00") + ":" + (secondsLeft).ToString("00");
                        else
                        {
                            if (paused)
                            {
                                livePulses.timer = (minutesLeft).ToString("00") + ":" + (secondsLeft).ToString("00");
                            }
                            else
                            {
                                int secondsDiff = (int) (DateTime.Now -  modifiedDate).TotalSeconds;
                                if (secondsDiff > 0)
                                {
                                    int minutesDiff = secondsDiff / 60;
                                    secondsDiff = secondsDiff % 60;

                                    if (minutesLeft > minutesDiff)
                                    {
                                        if(secondsDiff < secondsLeft)
                                            livePulses.timer = (minutesLeft - minutesDiff).ToString("00") + ":" + (secondsLeft - secondsDiff).ToString("00");
                                        else if((secondsLeft - secondsDiff) ==0 )
                                            livePulses.timer = (minutesLeft - minutesDiff).ToString("00") + ":" + (0).ToString("00");
                                        else
                                            livePulses.timer = (minutesLeft - minutesDiff - 1).ToString("00") + ":" + (secondsLeft - secondsDiff+60).ToString("00");
                                    }
                                    else
                                    {
                                        if (minutesLeft == minutesDiff)
                                        {
                                            if (secondsLeft > secondsDiff)
                                                livePulses.timer = "00:" + (secondsLeft - secondsDiff).ToString("00");
                                            else
                                                livePulses.timer = "00:00";
                                        }
                                        else
                                        {
                                            livePulses.timer = "00:00";
                                        }
                                    }
                                }
                                else
                                    livePulses.timer = "00:00";
                            }
                        }
                    }
                    else
                    {
                        livePulses.timer = "00:00";
                    }
                }
                livePulses.liveHighest = liveHighest;
                livePulses.liveLowest = liveLowest;
                livePulses.liveAverage = liveAverage;

                livePulses.sessionLowest = sessionLowest;
                livePulses.sessionHighest = sessionHighest;
                livePulses.sessionAverage = sessionAverage;

                


                //if (cv.Count > 0)
                //{
                //    livePulses.sessionHighest = cv.Max(t => t.HighestPulse);

                //    livePulses.sessionLowest = cv.Min(t => t.LowestPulse);
                //}
                

                return Json(livePulses);
                //return Json(new { livepulses = livePulses });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetManagers POSt action method.", "");
                return Json("");
            }
        }

        public IActionResult CreateCountDown(long teamID = 0, int minutes = 0, int seconds=0)
        {


            SqlConnection con = new SqlConnection(this._appConfig.connectionStrings.Default);
            SqlCommand cmd = new SqlCommand("APS_01_CreateCountDown", con);
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("@TeamID", SqlDbType.BigInt).Value = teamID;
            cmd.Parameters.Add("@Minutes", SqlDbType.Int).Value = minutes;
            cmd.Parameters.Add("@Seconds", SqlDbType.Int).Value = seconds;


            con.Open();
            int isRows = cmd.ExecuteNonQuery();

            con.Close();

            return Json("");
        }

        public IActionResult PauseCountDown(long teamID = 0, int minutes = 0, int seconds = 0)
        {


            SqlConnection con = new SqlConnection(this._appConfig.connectionStrings.Default);
            SqlCommand cmd = new SqlCommand("APS_01_PauseCountDown", con);
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("@TeamID", SqlDbType.BigInt).Value = teamID;
            cmd.Parameters.Add("@MinutesLeft", SqlDbType.Int).Value = minutes;
            cmd.Parameters.Add("@SecondsLeft", SqlDbType.Int).Value = seconds;


            con.Open();
            int isRows = cmd.ExecuteNonQuery();

            con.Close();

            return Json("");
        }

        public IActionResult RestartCountDown(long teamID = 0, int minutes = 0, int seconds = 0)
        {


            SqlConnection con = new SqlConnection(this._appConfig.connectionStrings.Default);
            SqlCommand cmd = new SqlCommand("APS_01_RestartCountDown", con);
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("@TeamID", SqlDbType.BigInt).Value = teamID;
            cmd.Parameters.Add("@MinutesLeft", SqlDbType.Int).Value = minutes;
            cmd.Parameters.Add("@SecondsLeft", SqlDbType.Int).Value = seconds;


            con.Open();
            int isRows = cmd.ExecuteNonQuery();

            con.Close();

            return Json("");
        }

        public IActionResult StopCountDown(long teamID = 0, int minutes = 0, int seconds = 0)
        {


            SqlConnection con = new SqlConnection(this._appConfig.connectionStrings.Default);
            SqlCommand cmd = new SqlCommand("APS_01_StopCountDown", con);
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("@TeamID", SqlDbType.BigInt).Value = teamID;
            cmd.Parameters.Add("@MinutesLeft", SqlDbType.Int).Value = minutes;
            cmd.Parameters.Add("@SecondsLeft", SqlDbType.Int).Value = seconds;


            con.Open();
            int isRows = cmd.ExecuteNonQuery();

            con.Close();

            return Json("");
        }

        public IActionResult UpdateCountDown(long teamID = 0, int minutes = 0, int seconds = 0)
        {
            SqlConnection con = new SqlConnection(this._appConfig.connectionStrings.Default);
            SqlCommand cmd = new SqlCommand("APS_01_UpdateCountDown", con);
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("@TeamID", SqlDbType.BigInt).Value = teamID;
            cmd.Parameters.Add("@MinutesLeft", SqlDbType.Int).Value = minutes;
            cmd.Parameters.Add("@SecondsLeft", SqlDbType.Int).Value = seconds;


            con.Open();
            int isRows = cmd.ExecuteNonQuery();

            con.Close();

            return Json("");
        }
    }

    public class LivePulse
    {
        public long liveHighest { get; set; }
        public long liveLowest { get; set; }
        public long liveAverage { get; set; }
        public long sessionHighest { get; set; }
        public long sessionLowest { get; set; }
        public long sessionAverage { get; set; }

        public string timer { get; set; }

        public string imageName { get; set; }
        public string title1 { get; set; }
        public string title2 { get; set; }

        public LivePulse(long liveHighest, long liveLowest, long liveAverage, long sessionHighest,long sessionAverage, long sessionLowest, string timer = "", string imageName = "",string title1= "",string title2="")
        {
            this.liveHighest = liveHighest;
            this.liveLowest = liveLowest;
            this.liveAverage = liveAverage;
            this.sessionHighest = sessionHighest;
            this.sessionLowest = sessionLowest;
            this.timer = timer;
            this.imageName = imageName;
            this.title1 = title1;
            this.title2 = title2;
        }

        public LivePulse()
        {
            timer = "";
            imageName = "";
        }
    }
}
