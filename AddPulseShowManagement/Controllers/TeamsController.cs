using AddPulseShowManagement.Common;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Data.ViewModels;
using AddPulseShowManagement.Repo;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AddPulseShowManagement.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MSSQLDbContext _context;
        private readonly AppConfig _appConfig;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IShowService _showService;
        private readonly IContestantService _conestantService;

        public TeamsController(ILogger<HomeController> logger,
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

        public IActionResult Teams()
        {
            try
            {
                List<Shows> shows = this._showService.GetShows();
                ViewBag.shows = shows;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> Teamss GET action method.", "");
                return View("login", "account");
            }
           
        }

        [HttpGet]
        public IActionResult GetTeams()
        {
            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetTeams GET action method.", "");
                return View("login", "account");
            }
        }



        public IActionResult TeamDetail(long teamID=0)
        {
            try
            {
                ViewBag.TeamID = teamID;

                Teams team = this._teamService.GetTeamDetails(teamID);
                

                ViewBag.team = team;
               
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> TeamDetail GET action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult GetContenstantList(long teamID = 0)
        {
            try
            {
                List<Contestants> members = this._teamService.GetTeamContestants(teamID) ?? new List<Contestants>();

                foreach (Contestants item in members)
                {
                    if (!string.IsNullOrEmpty(item.ProfileImage))
                    {
                        item.ProfileImage = _appConfig.fileLocations.ContestantImage + item.ContestantID + "/" + item.ProfileImage;
                    }
                    else
                    {
                        item.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }
                }

                ReportData<Contestants> reportData = new ReportData<Contestants>();
                reportData.draw = 10;
                reportData.recordsTotal = members.Count;
                reportData.data = members;

                return Json(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetContenstantList action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult GetContenstantListTable(long teamID = 0)
        {
            try
            {
                ViewBag.TeamID = teamID;
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetContenstantListTable action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult ContestantsList(long teamID=0)
        {
            try
            {
                ViewBag.teamID = teamID;
               
   
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> ContestantsList action method.", "");
                return View("login", "account");
            }
         
        }
        public IActionResult ContestantsListView(long teamID = 0)
        {
            try
            {
                ViewBag.teamID = teamID;
                string teamName = this._context.Teams.Where(y => y.TeamID == teamID &&  y.IsActive ==true).Select(s => s.TeamName).FirstOrDefault() ?? "";
                ViewBag.teamName = teamName;
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> ContestantsList action method.", "");
                return View("login", "account");
            }

        }

        public IActionResult ContestantsTableList(long teamID = 0)
        {
            try
            {
                var teamlst = this._teamService.GetSuggestedContestantList(teamID) ?? new List<Contestants>();

                ReportData<Contestants> reportData = new ReportData<Contestants>();
                reportData.draw = 10;
                reportData.recordsTotal = teamlst.Count;
                reportData.data = teamlst;

                return Json(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> ContestantsList action method.", "");
                return View("login", "account");
            }

        }

        public IActionResult GetTeamsModal(long teamID=0)
        {
            try
            {
                ViewBag.TeamID = teamID;
                List<Shows> shows = this._showService.GetShows() ?? new List<Shows>();
                
                Teams team = new Teams();
                if (teamID > 0)
                {
                    team = this._context.Teams.FirstOrDefault(y => y.TeamID == teamID) ?? new Teams();
                }
                
                ViewBag.team = team;
                ViewBag.BtnTitle = teamID > 0 ? "Update" : "Add";
                ViewBag.PopTitle = teamID > 0 ? "Edit Team" : "Add Team";
                ViewBag.shows = shows;

                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetTeamsModal GET action method.", "");
                return View("login", "account");
            }

        }

        public IActionResult GetEliminatedContestant(long teamContestantID = 0,long contestantID = 0)
        {
            try
            {
                Contestants contestant = _context.Contestants.Where(y => y.ContestantID == contestantID).FirstOrDefault();

                ViewBag.contestant = contestant;
                ViewBag.teamContestantID = teamContestantID;
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetEliminatedContestant action method.", "");
                return View("login", "account");
            }
        }


        public IActionResult GetContenstantVoteIn(long teamContestantID = 0, long contestantID = 0)
        {
            try
            {
                Contestants contestant = _context.Contestants.Where(y => y.ContestantID == contestantID).FirstOrDefault();

                ViewBag.contestant = contestant;
                ViewBag.teamContestantID = teamContestantID;
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetContenstantVoteIn action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult DashboardGetTeamData(string teamDiv = "teamDiv", int isConfigUpdated = 1, long teamID = 0, int hour = 1)
        {
            try
            {
                Teams team = _context.Teams.Where(y => y.TeamID == teamID).FirstOrDefault() ?? new Teams();

                // Get Show data
                Shows show = _context.Shows.Where(y => y.ShowID == team.ShowID).FirstOrDefault() ?? new Shows();

                List<ContestantsView> cv = this._conestantService.GetTeamData(teamID, isConfigUpdated, hour) ?? new List<ContestantsView>();

                List<string> gdates = new List<string>();
                DateTime stdate = DateTime.UtcNow.AddDays(-6);
                DateTime enddate = DateTime.UtcNow;

                TeamsVsData teamdata = new TeamsVsData();

                gdates = Extensions.GetDaysDatesBetween(stdate, enddate);
                int totalSum = 0;
                int totalcnt = 0;

                foreach (ContestantsView item in cv)
                {
                    item.GraphVal = !string.IsNullOrEmpty(item.GraphVal) ? item.GraphVal.TrimEnd(',') : "";
                    item.XGraphVal = !string.IsNullOrEmpty(item.GraphVal) ? item.XGraphVal.TrimEnd(',') : "";
                    if (!string.IsNullOrEmpty(item.ProfileImage))
                    {
                        item.ProfileImage = _appConfig.fileLocations.ContestantImage + item.ContestantID + "/" + item.ProfileImage;
                    }
                    else
                    {
                        item.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }
                    List<int> pulses = new List<int>();
                    if (!string.IsNullOrEmpty(item.GraphVal))
                    {
                        pulses = item.GraphVal.Split(',').Select(int.Parse).ToList();
                        totalSum = totalSum + pulses.Sum();
                        totalcnt = totalcnt + pulses.Count();
                    }
                }

                int hp = 0;
                if (cv.Count > 0)
                    hp = cv.Max(t => t.HighestPulse);
                teamdata.highTeamPulse = hp;

                teamdata.highPulseName = cv.Where(y => hp == y.HighestPulse).Select(x => x.Name).FirstOrDefault();

                int minp = 0;
                if (cv.Count > 0)
                    minp = cv.Min(t => t.LowestPulse);
                teamdata.lowTeamPulse = minp;
                teamdata.LowPulseName = cv.Where(y => minp == y.LowestPulse).Select(x => x.Name).FirstOrDefault();

                decimal avg = 0;
                //int sumval = cv.Sum(x => x.LivePulse);
                //int cntval = cv.Count();
                //cntval = cntval == 0 ? 1 : cntval;
                //// avg = sumval / cntval;
                //totalcnt = totalcnt == 0 ? 1 : totalcnt;
                //avg = totalSum / totalcnt;

                //new avg
                totalSum = 0;
                totalcnt = 0;
                totalSum = cv.Sum(x => x.AvgPulse);
                totalcnt = cv.Count;

                //totalcnt = totalcnt == 0 ? 1 : totalcnt;
                if (totalcnt > 0)
                {
                    avg = totalSum / totalcnt;
                }
                else
                {
                    avg = 0;
                }


                teamdata.avgTeamPulse = (int)avg;

                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                long points = 3600;
                long Tpoints = 3600;

                if (configurations != null)
                {
                    points = configurations.TimeInSeconds.HasValue ? configurations.TimeInSeconds.Value : 3600;
                }


                //commented out to stop flickering on the Dashboard
                //int refreshTime = configurations.RefreshTimePeriod.HasValue ? configurations.RefreshTimePeriod.Value : 5;
                int refreshTime = 5;


                if (configurations != null)
                {
                    points = configurations.TimeInSeconds.HasValue ? configurations.TimeInSeconds.Value : 3600;
                    Tpoints = (int)(points / refreshTime);
                }



                //Check for Countdown
                LivePulse livePulses = new LivePulse();
                //List<ContestantsView> cv = this._conestantService.GetTeamData(teamID) ?? new List<ContestantsView>();
                DataSet ds = this._conestantService.GetLivePulseTeamData(teamID) ?? new DataSet();
                bool paused = false, stopped = false;
                if (ds != null)
                {
                    if (ds.Tables[2].Rows.Count > 0)
                    {

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
                            livePulses.timer = "";
                        else
                        {
                            if (paused)
                            {
                                livePulses.timer = (minutesLeft).ToString("00") + ":" + (secondsLeft).ToString("00");
                            }
                            else
                            {
                                int secondsDiff = (int)(DateTime.UtcNow - modifiedDate).TotalSeconds;
                                if (secondsDiff > 0)
                                {
                                    int minutesDiff = secondsDiff / 60;
                                    secondsDiff = secondsDiff % 60;

                                    if (minutesLeft > minutesDiff)
                                    {
                                        if (secondsDiff < secondsLeft)
                                            livePulses.timer = (minutesLeft - minutesDiff).ToString("00") + ":" + (secondsLeft - secondsDiff).ToString("00");
                                        else
                                            livePulses.timer = (minutesLeft - minutesDiff - 1).ToString("00") + ":" + (secondsLeft - secondsDiff + 60).ToString("00");
                                    }
                                    else
                                    {
                                        if (minutesLeft == minutesDiff)
                                        {
                                            if (secondsLeft > secondsDiff)
                                                livePulses.timer = "00:" + (secondsLeft - secondsDiff).ToString("00");
                                            else
                                                livePulses.timer = "";
                                        }
                                        else
                                        {
                                            livePulses.timer = "";
                                        }
                                    }
                                }
                                else
                                    livePulses.timer = "";
                            }
                        }
                    }
                    else
                    {
                        livePulses.timer = "";
                    }
                }

                if (!string.IsNullOrEmpty(livePulses.timer))
                {
                    ViewBag.continueCD = true;
                    ViewBag.timer = livePulses.timer;
                    ViewBag.paused = paused;
                }
                else
                {
                    ViewBag.continueCD = false;
                    ViewBag.timer = "60:00";
                }

                //Countdown ends



                ViewBag.points = Tpoints;
                ViewBag.refreshTimeVal = refreshTime * 1000;

                ViewBag.team = team;
                ViewBag.show = show;
                ViewBag.cv = cv;
                ViewBag.teamDiv = teamDiv;
                ViewBag.gdates = string.Join(",", gdates);
                ViewBag.teamID = teamID;
                ViewBag.isConfigUpdated = isConfigUpdated;
                ViewBag.hour = hour;
                ViewBag.teamdata = teamdata;

                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> DashboardGetTeamData action method.", "");
                return View("login", "account");
            }
        }


        public IActionResult UpdateTeamCardValues(string teamDiv = "teamDiv", int isConfigUpdated = 1, long teamID = 0, int hour = 1)
        {
            try
            {
                Teams team = _context.Teams.Where(y => y.TeamID == teamID).FirstOrDefault() ?? new Teams();

                List<ContestantsView> cv = this._conestantService.GetTeamData(teamID, isConfigUpdated, hour) ?? new List<ContestantsView>();

                List<string> gdates = new List<string>();
                DateTime stdate = DateTime.UtcNow.AddDays(-6);
                DateTime enddate = DateTime.UtcNow;
                TeamsVsData teamdata = new TeamsVsData();
                gdates = Extensions.GetDaysDatesBetween(stdate, enddate);
                int totalSum = 0;
                int totalcnt = 0;
                foreach (ContestantsView item in cv)
                {
                    item.GraphVal = !string.IsNullOrEmpty(item.GraphVal) ? item.GraphVal.TrimEnd(',') : "";
                    item.XGraphVal = !string.IsNullOrEmpty(item.GraphVal) ? item.XGraphVal.TrimEnd(',') : "";
                    if (!string.IsNullOrEmpty(item.ProfileImage))
                    {
                        item.ProfileImage = _appConfig.fileLocations.ContestantImage + item.ContestantID + "/" + item.ProfileImage;
                    }
                    else
                    {
                        item.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }

                    List<int> pulses = new List<int>();
                    if (!string.IsNullOrEmpty(item.GraphVal))
                    {
                        pulses = item.GraphVal.Split(',').Select(int.Parse).ToList();
                        totalSum = totalSum + pulses.Sum();
                        totalcnt = totalcnt + pulses.Count();
                    }
                }


                int hp = 0;
                if(cv.Count >0)
                   hp = cv.Max(t => t.HighestPulse);
                teamdata.highTeamPulse = hp;

                

                teamdata.highPulseName = cv.Where(y => hp == y.HighestPulse).Select(x => x.Name).FirstOrDefault();

                int minp = 0;
                if(cv.Count >0)
                   minp = cv.Min(t => t.LowestPulse);
                teamdata.lowTeamPulse = minp;
                teamdata.LowPulseName = cv.Where(y => minp == y.LowestPulse).Select(x => x.Name).FirstOrDefault();



                decimal avg = 0;
                int sumval = cv.Sum(x => x.LivePulse);
                int cntval = cv.Count();
                cntval = cntval == 0 ? 1 : cntval;
                //avg = sumval / cntval;
                totalcnt = totalcnt == 0 ? 1 : totalcnt;


                //new avg
                totalSum = 0;
                totalcnt = 0;
                totalSum = cv.Sum(x => x.AvgPulse);
                totalcnt = cv.Count;

                //totalcnt = totalcnt == 0 ? 1 : totalcnt;
                if (totalcnt > 0)
                {
                    avg = totalSum / totalcnt;
                }
                else
                {
                    avg = 0;
                }


                teamdata.avgTeamPulse = (int)avg;


                return Json(new{ team= team, cv= cv , teamDiv = teamDiv, teamID= teamID , teamdata = teamdata });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> DashboardGetTeamData action method.", "");
                return View("login", "account");
            }
        }


        public IActionResult DashboardGetTeamVsTeam(string teamDiv = "teamDiv", int isConfigUpdated = 1, long showID=0,int hour=1)
        {
            try
            {
                List<string> gdates = new List<string>();
                DateTime stdate = DateTime.UtcNow.AddDays(-6);
                DateTime enddate = DateTime.UtcNow;

                gdates = Extensions.GetDaysDatesBetween(stdate,enddate);
                List<Teams> team = _context.Teams.Where(y => y.ShowID == showID).ToList() ?? new List<Teams>();
                List<TeamsVsData> maincv = new List<TeamsVsData>();
                foreach (Teams item in team)
                {
                    TeamsVsData td = new TeamsVsData();
                    
                    List<ContestantsView> cv = this._conestantService.GetTeamData(item.TeamID, isConfigUpdated, hour) ?? new List<ContestantsView>();
                    int totalSum = 0;
                    int totalcnt = 0;
                    foreach (ContestantsView dt in cv)
                    {
                        if (!string.IsNullOrEmpty(dt.ProfileImage))
                        {
                            dt.ProfileImage = _appConfig.fileLocations.ContestantImage + dt.ContestantID + "/" + dt.ProfileImage;
                        }
                        else
                        {
                            dt.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                        }

                        dt.GraphVal = !string.IsNullOrEmpty(dt.GraphVal)? dt.GraphVal.TrimEnd(','):"";                        
                        dt.XGraphVal = !string.IsNullOrEmpty(dt.XGraphVal) ? dt.XGraphVal.TrimEnd(','):"";                        
                        int hp = cv.Max(t => t.HighestPulse);
                        td.highTeamPulse = hp;

                        List<int> pulses = new List<int>();
                        if (!string.IsNullOrEmpty(dt.GraphVal))
                        {
                            pulses = dt.GraphVal.Split(',').Select(int.Parse).ToList();
                            totalSum = totalSum + pulses.Sum();
                            totalcnt = totalcnt + pulses.Count();
                        }

                        td.highPulseName = cv.Where(y=> hp==y.HighestPulse ).Select(x => x.Name).FirstOrDefault();

                        int minp = cv.Min(t => t.LowestPulse);
                        td.lowTeamPulse = minp;
                        td.LowPulseName = cv.Where(y => minp == y.LowestPulse).Select(x => x.Name).FirstOrDefault();

                    }
                    decimal avg = 0;
                    //new avg
                    totalSum = 0;
                    totalcnt = 0;
                    totalSum = cv.Sum(x => x.AvgPulse);
                    totalcnt = cv.Count;

                    //totalcnt = totalcnt == 0 ? 1 : totalcnt;
                    if (totalcnt > 0)
                    {
                        avg = totalSum / totalcnt;
                    }
                    else
                    {
                        avg = 0;
                    }

                    td.avgTeamPulse = (int)avg;

                    td.team= item.CopyTo<Teams>();
                    td.contestantsView = cv;
                    
                    maincv.Add(td);

                }

                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                long points = 3600;
                long Tpoints = 3600;
                int refreshTime = configurations.RefreshTimePeriod.HasValue ? configurations.RefreshTimePeriod.Value : 5;


                if (configurations != null)
                {
                    points = configurations.TimeInSeconds.HasValue ? configurations.TimeInSeconds.Value : 3600;
                    Tpoints = (int)(points / refreshTime);
                }

                ViewBag.refreshTimeVal = refreshTime * 1000;

                ViewBag.points = Tpoints;

                ViewBag.gdates = string.Join(",",gdates);
                ViewBag.team = team;
                ViewBag.hour = hour;
                ViewBag.maincv = maincv;
                ViewBag.teamDiv = teamDiv;               
                ViewBag.showID = showID;
                ViewBag.isConfigUpdated = isConfigUpdated;
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> DashboardGetTeamVsTeam action method.", "");
                return View("login", "account");
            }
        }


        public IActionResult UpdateTeamVsTeamCardValues(string teamDiv = "teamDiv",int isConfigUpdated = 1, long showID = 0, int hour = 1)
        {
            try
            {
                List<string> gdates = new List<string>();
                //DateTime stdate = DateTime.UtcNow.AddDays(-6);
                //DateTime enddate = DateTime.UtcNow;

                //gdates = Extensions.GetDaysDatesBetween(stdate, enddate);
                List<Teams> team = _context.Teams.Where(y => y.ShowID == showID).ToList() ?? new List<Teams>();
                List<TeamsVsData> maincv = new List<TeamsVsData>();
                List<Teams> teamlst = new List<Teams>();
                List<ContestantsView> conlst = new List<ContestantsView>();
                int totalSum = 0;
                int totalcnt = 0;
                foreach (Teams item in team)
                {
                    TeamsVsData td = new TeamsVsData();

                    List<ContestantsView> cv = this._conestantService.GetTeamData(item.TeamID, isConfigUpdated, hour) ?? new List<ContestantsView>();
                    foreach (ContestantsView dt in cv)
                    {
                        if (!string.IsNullOrEmpty(dt.ProfileImage))
                        {
                            dt.ProfileImage = _appConfig.fileLocations.ContestantImage + dt.ContestantID + "/" + dt.ProfileImage;
                        }
                        else
                        {
                            dt.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                        }

                        dt.GraphVal = !string.IsNullOrEmpty(dt.GraphVal) ? dt.GraphVal.TrimEnd(',') : "";
                        dt.XGraphVal = !string.IsNullOrEmpty(dt.XGraphVal) ? dt.XGraphVal.TrimEnd(',') : "";
                        int hp = cv.Max(t => t.HighestPulse);
                        td.highTeamPulse = hp;

                        List<int> pulses = new List<int>();
                        if (!string.IsNullOrEmpty(dt.GraphVal))
                        {
                            pulses = dt.GraphVal.Split(',').Select(int.Parse).ToList();
                            totalSum = totalSum + pulses.Sum();
                            totalcnt = totalcnt + pulses.Count();
                        }


                        td.highPulseName = cv.Where(y => hp == y.HighestPulse).Select(x => x.Name).FirstOrDefault();

                        int minp = cv.Min(t => t.LowestPulse);
                        td.lowTeamPulse = minp;
                        td.LowPulseName = cv.Where(y => minp == y.LowestPulse).Select(x => x.Name).FirstOrDefault();

                        conlst.Add(dt);

                    }

                    decimal avg = 0;

                    //int cntval = cv.Count();
                    //cntval = cntval == 0 ? 1 : cntval;
                    ////avg = sumval / cntval;
                    //totalcnt = totalcnt == 0 ? 1 : totalcnt;


                    //new avg
                    totalSum = 0;
                    totalcnt = 0;
                    totalSum = cv.Sum(x => x.AvgPulse);
                    totalcnt = cv.Count;

                    //totalcnt = totalcnt == 0 ? 1 : totalcnt;
                    if (totalcnt > 0)
                    {
                        avg = totalSum / totalcnt;
                    }
                    else
                    {
                        avg = 0;
                    }


                    

                    td.avgTeamPulse = (int)avg;
                    td.team = item.CopyTo<Teams>();
                    td.contestantsView = cv;


                    teamlst.Add(td.team);
                    maincv.Add(td);

                }

                return Json(new { team= team, maincv= maincv, teamDiv= teamDiv, showID= showID, conlst= conlst, teamlst= teamlst });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> UpdateTeamVsTeamCardValues action method.", "");
                return View("login", "account");
            }
        }



        public IActionResult DashboardGetPlayerVsPlayerID(long showID = 0, int isConfigUpdated = 1, int hour=1)
        {
            try
            {
                List<PlayerVsCardView> pc = new List<PlayerVsCardView>();
                List<PlayerVsCardDataViewModel> data = new List<PlayerVsCardDataViewModel>();
                List<string> gdates = new List<string>();
                DateTime stdate = DateTime.UtcNow.AddDays(-6);
                DateTime enddate = DateTime.UtcNow;

                gdates = Extensions.GetDaysDatesBetween(stdate, enddate);

                pc = this._conestantService.GetPlayerVsPlayerData(showID,isConfigUpdated, hour) ?? new List<PlayerVsCardView>();

                foreach (PlayerVsCardView item in pc)
                {
                    if (!string.IsNullOrEmpty(item.ProfileImage))
                    {
                        item.ProfileImage = _appConfig.fileLocations.ContestantImage + item.ContestantID + "/" + item.ProfileImage;
                    }
                    else
                    {
                        item.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }

                    item.GraphVal = !string.IsNullOrEmpty(item.GraphVal)?item.GraphVal.TrimEnd(','):"";
                    item.XGraphVal = !string.IsNullOrEmpty(item.XGraphVal) ?item.XGraphVal.TrimEnd(','):"";                  

                }

                List<int> seqlst = new List<int>();

                if (pc.Count>0)
                {                   
                    seqlst = pc.DistinctBy(y => y.Seq).Select(x=>x.Seq).ToList();
                    for (int i = 0; i < seqlst.Count; i++)
                    {
                        PlayerVsCardDataViewModel pcm = new PlayerVsCardDataViewModel();
                        int seq1 = seqlst[i];
                        List<PlayerVsCardView> pclst = pc.Where(x => x.Seq == seq1).ToList() ?? new List<PlayerVsCardView>();
                        bool isOk = pclst.Any(x => x.ContestantID == 0) ? false : true;
                        if (isOk == true)
                        {
                            pcm.Seq = seq1;
                            pcm.cardData = pclst;
                            data.Add(pcm);
                        }
                    }
                }


                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                long points = 3600;
                long Tpoints = 3600;

                int refreshTime = configurations.RefreshTimePeriod.HasValue ? configurations.RefreshTimePeriod.Value : 5;

                if (configurations != null)
                {
                    points = configurations.TimeInSeconds.HasValue ? configurations.TimeInSeconds.Value : 3600;
                    Tpoints = (int)(points / refreshTime);
                }

                ViewBag.points = Tpoints;
                ViewBag.refreshTimeVal = refreshTime * 1000;
                ViewBag.gdates = string.Join(",", gdates);
                ViewBag.data = data;
                ViewBag.showID = showID;
                ViewBag.hour = hour;
                ViewBag.isConfigUpdated = isConfigUpdated;

                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> DashboardGetPlayerVsPlayerID  action method.", "");
                return View("login", "account");
            }
        }


        public IActionResult UpdatePlayerVsPlayerCardValues(long showID = 0, int isConfigUpdated = 1, int hour = 1)
        {
            try
            {
                List<PlayerVsCardView> pc = new List<PlayerVsCardView>();
                List<PlayerVsCardDataViewModel> data = new List<PlayerVsCardDataViewModel>();
                List<PlayerVsCardView> mpclst = new List<PlayerVsCardView>();
               
                pc = this._conestantService.GetPlayerVsPlayerData(showID, isConfigUpdated, hour) ?? new List<PlayerVsCardView>();

                foreach (PlayerVsCardView item in pc)
                {
                    if (!string.IsNullOrEmpty(item.ProfileImage))
                    {
                        item.ProfileImage = _appConfig.fileLocations.ContestantImage + item.ContestantID + "/" + item.ProfileImage;
                    }
                    else
                    {
                        item.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }

                    item.GraphVal = !string.IsNullOrEmpty(item.GraphVal) ? item.GraphVal.TrimEnd(',') : "";
                    item.XGraphVal = !string.IsNullOrEmpty(item.XGraphVal) ? item.XGraphVal.TrimEnd(',') : "";

                }

                List<int> seqlst = new List<int>();

                if (pc.Count > 0)
                {
                    seqlst = pc.DistinctBy(y => y.Seq).Select(x => x.Seq).ToList();
                    for (int i = 0; i < seqlst.Count; i++)
                    {
                        PlayerVsCardDataViewModel pcm = new PlayerVsCardDataViewModel();
                        int seq1 = seqlst[i];
                        List<PlayerVsCardView> pclst = pc.Where(x => x.Seq == seq1).ToList() ?? new List<PlayerVsCardView>();
                        pcm.Seq = seq1;
                        pcm.cardData = pclst;
                        foreach (PlayerVsCardView item in pclst)
                        {
                            mpclst.Add(item);
                        }
                       
                        data.Add(pcm);
                    }

                }
               
                return Json(new { datalst=data,pclst= mpclst });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> UpdatePlayerVsPlayerCardValues  action method.", "");
                return View("login", "account");
            }
        }


        public async Task<IActionResult> GetTeamsList(string search = "", int draw = 0, int start = 0, int length = 10,long showID=0,string startDate="",string endDate="")
        {
            try
            {
                var teamlst= this._teamService.GetTeams(showID,startDate,endDate,search) ?? new List<Teams>();
               
                ReportData<Teams> reportData = new ReportData<Teams>();
                reportData.draw = draw;
                reportData.recordsTotal = teamlst.Count;
                reportData.data = teamlst;

                return Json(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetManagers POSt action method.", "");
                return View("login", "account");
            }
        }


        public IActionResult GetContestantLiveGraphData(long contestantID=0)
        {
            try
            {
                LiveData liveData = this._conestantService.GetContestantLiveGraphData(contestantID) ?? new LiveData();

                return Json(liveData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetContestantLiveGraphData  action method.", "");
                return View("login", "account");
            }
        }


        #region Save & Updates

        public IActionResult SaveUpdateTeams([FromForm] Teams team, int IsActive = 1)
        {
            try
            {
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserTypeID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserTypeID").Value) : 0;
                if (IsActive == 0)
                {
                    team.IsActive = false;
                }
                else
                {
                    team.IsActive = true;
                }
                if (team.TeamID == 0)
                {


                    team.CreatedDate = DateTime.UtcNow;
                    team.CreatedBy = UserID;
                    team.IsActive = true;
                    team.ModifiedDate = DateTime.UtcNow;
                    team.ModifiedBy = UserID;
                }
                else
                {
                    team.ModifiedDate = DateTime.UtcNow;
                    team.ModifiedBy = UserID;
                }

                var updated = this._teamService.SaveUpdateTeam(team);
                return Json(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> SaveUpdateTeams  action method.", "");
                return View("login", "account");
            }
        }

        public bool SaveUpdateContestantList(long teamID = 0, string contestantList = "")
        {
            try
            {
                var updated = this._teamService.SaveUpdateContestantList(teamID, contestantList);
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> SaveUpdateContestantList action method.", "");
                return false;
            }
        }

        #region Vote OUT and IN

        public bool VoteOutContestant(long teamContestantID = 0)
        {
            try
            {
                var updated = this._teamService.VoteOutContestant(teamContestantID);
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> VoteOutContestant action method.", "");
                return false;
            }
        }

        public bool VoteInContestant(long teamContestantID = 0)
        {
            try
            {
                var updated = this._teamService.VoteinContestant(teamContestantID);
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> VoteInContestant action method.", "");
                return false;
            }
        }


        #endregion

        #endregion

    }
}
