using AddPulseShowManagement.Common;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Data.ViewModels;
using AddPulseShowManagement.Repo;
using Microsoft.AspNetCore.Mvc;

namespace AddPulseShowManagement.Controllers
{
    public class ShowsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MSSQLDbContext _context;
        private readonly AppConfig _appConfig;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IShowService _showService;

        public ShowsController (ILogger<HomeController> logger,
            MSSQLDbContext context,
            AppConfig appConfig,
            IUserService userService,
            ITeamService teamService,
            IShowService showService)
        {
            this._appConfig = appConfig;
            this._context = context;
            this._logger = logger;
            this._userService = userService;
            this._teamService = teamService;
            this._showService = showService;
        }

        public IActionResult Shows()
        {
            return View();
        }
        public IActionResult ShowDetail(long showID = 0)
        {
            try
            {
                //get list of teams 
                List<TeamsViewModal> teamsViews = new List<TeamsViewModal>();
                List<Teams> teams = this._showService.GetTeamsOfShow(showID) ??new  List<Teams>();
                Shows show = _context.Shows.Where(y => y.ShowID == showID).FirstOrDefault()?? new Data.DataTableModels.Shows();

                foreach (Teams item in teams)
                {
                    TeamsViewModal tm = new TeamsViewModal();
                    tm.TeamID = item.TeamID;
                    tm.TeamName = item.TeamName;
                    tm.ModifiedDate = item.ModifiedDate;
                    tm.ModifiedBy = item.ModifiedBy;

                    Contestants contestant = new Contestants();

                    List<Contestants> contestant1 = this._teamService.GetTeamContestants(item.TeamID) ?? new List<Contestants>();

                    foreach (Contestants itm in contestant1)
                    {
                        if(!string.IsNullOrEmpty(itm.ProfileImage))
                        {
                            itm.ProfileImage = _appConfig.fileLocations.ContestantImage + itm.ContestantID + "/" + itm.ProfileImage;
                        }
                        else
                        {
                            itm.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                        }
                    }

                    tm.contestants=contestant1;

                    teamsViews.Add(tm);
                }

                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                int refreshTime = configurations.RefreshTimePeriod.HasValue ? configurations.RefreshTimePeriod.Value : 5;


                ViewBag.refreshTimeVal = refreshTime * 1000;

                ViewBag.teamsViews = teamsViews;
                ViewBag.show = show;
                ViewBag.showID = showID;


                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowsController -> ShowDetail action method.", "");
                return View("login", "account");
            }

           
        }

        public IActionResult UpdateShowDetail(long showID = 0)
        {
            try
            {
                //get list of teams 
                List<TeamsViewModal> teamsViews = new List<TeamsViewModal>();
                List<Teams> teams = this._showService.GetTeamsOfShow(showID) ?? new List<Teams>();
                Shows show = _context.Shows.Where(y => y.ShowID == showID).FirstOrDefault();
                List<Contestants> contestantlst = new List<Contestants>();
                foreach (Teams item in teams)
                {
                    TeamsViewModal tm = new TeamsViewModal();
                    tm.TeamID = item.TeamID;
                    tm.TeamName = item.TeamName;
                    tm.ModifiedDate = item.ModifiedDate;
                    tm.ModifiedBy = item.ModifiedBy;

                    Contestants contestant = new Contestants();

                    List<Contestants> contestant1 = this._teamService.GetTeamContestants(item.TeamID) ?? new List<Contestants>();

                    //foreach (Contestants itm in contestant1)
                    //{
                    //    if (!string.IsNullOrEmpty(itm.ProfileImage))
                    //    {
                    //        itm.ProfileImage = _appConfig.fileLocations.ContestantImage + itm.ContestantID + "/" + itm.ProfileImage;
                    //    }
                    //    else
                    //    {
                    //        itm.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    //    }
                    //}

                    tm.contestants = contestant1;
                    contestantlst.AddRange(tm.contestants);
                    teamsViews.Add(tm);
                }

                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                int refreshTime = configurations.RefreshTimePeriod.HasValue ? configurations.RefreshTimePeriod.Value : 5;

                //ViewBag.teamsViews = teamsViews;
               
                return Json(new { teamsViews = teamsViews , contestantlst = contestantlst });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowsController -> ShowDetail action method.", "");
                return View("login", "account");
            }


        }

        public IActionResult ManageTeam(long teamID=0)
        {
            try
            {
                ViewBag.teamID = teamID;

                long showID = 0;
                    
                long.TryParse(_context.Teams.Where(y => y.TeamID == teamID).Select(x => x.ShowID).FirstOrDefault().ToString(),out showID);
                ViewBag.showID = showID;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowsController -> ManageTeam action method.", "");
                return View("login", "account");
            }
            
        }

        [HttpGet]
        public IActionResult GetShowTable()
        {
            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ManagerController -> GetShowTable GET action method.", "");
                return View("login", "account");
            }
        }

        public async Task<IActionResult> GetShowList(string search = "", int draw = 0, int start = 0, int length = 10, long showID = 0, string startDate = "", string endDate = "")
        {
            try
            {
                var showlst = await this._showService.GetShowList(startDate, endDate) ?? new List<Shows>();
               
                ReportData<Shows> reportData = new ReportData<Shows>();
                reportData.draw = draw;
                reportData.recordsTotal = showlst.Count;
                reportData.data = showlst;

                return Json(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowController -> GetShowList POSt action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult GetShows()
        {
            try
            {
                List<Shows> showlst =  this._showService.GetShows() ?? new List<Shows>();


                return Json(showlst);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowController -> GetShowList POSt action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult SaveUpdateShows([FromForm] Shows show, int IsActive = 1)
        {
            try
            {
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserTypeID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserTypeID").Value) : 0;
                if (IsActive == 0)
                {
                    show.IsActive = false;
                }
                else
                {
                    show.IsActive = true;
                }
                if (show.ShowID == 0)
                {
                    show.CreatedDate = DateTime.UtcNow;
                    show.CreatedBy = UserID;
                    show.IsActive = true;                   
                }
                
                show.ModifiedDate = DateTime.UtcNow;
                show.ModifiedBy = UserID;

                var updated = this._showService.SaveUpdateShow(show);
                return Json(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> SaveUpdateTeams  action method.", "");
                return View("login", "account");
            }
        }


        

        #region Modals

        public IActionResult GetShowsModal(long showID=0)
        {
            try
            {
                ViewBag.ShowID = showID;
                Shows show = new Shows();
                if (showID > 0)
                {
                    show = this._context.Shows.FirstOrDefault(y => y.ShowID == showID) ?? new Shows();
                }

                ViewBag.show = show;
                ViewBag.BtnTitle = showID > 0 ? "Update" : "Add";
                ViewBag.PopTitle = showID > 0 ? "Edit Show" : "Add Show";
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowController -> GetShowsModal POSt action method.", "");
                return View("login", "account");
            }
        }
        #endregion

        [HttpPost]
        public IActionResult StartShow(long showID)
        {
            try
            {
                var show = _context.Shows.FirstOrDefault(s => s.ShowID == showID);

                if (show == null)
                {
                    return Json(new { success = false, message = "Show not found." });
                }

                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserTypeID").Value)
                    ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserTypeID").Value)
                    : 0;

                // Set new start date and clear end date to allow restarting
                show.StartDate = DateTime.UtcNow;
                show.EndDate = null;
                show.ModifiedDate = DateTime.UtcNow;
                show.ModifiedBy = UserID;

                _context.SaveChanges();

                return Json(new { success = true, message = "Show started successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowsController -> StartShow action method.");
                return Json(new { success = false, message = "An error occurred while starting the show." });
            }
        }

        [HttpPost]
        public IActionResult StopShow(long showID)
        {
            try
            {
                var show = _context.Shows.FirstOrDefault(s => s.ShowID == showID);

                if (show == null)
                {
                    return Json(new { success = false, message = "Show not found." });
                }

                if (!show.StartDate.HasValue)
                {
                    return Json(new { success = false, message = "Show has not been started yet." });
                }

                if (show.EndDate.HasValue)
                {
                    return Json(new { success = false, message = "Show has already been stopped." });
                }

                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserTypeID").Value)
                    ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserTypeID").Value)
                    : 0;

                show.EndDate = DateTime.UtcNow;
                show.ModifiedDate = DateTime.UtcNow;
                show.ModifiedBy = UserID;

                _context.SaveChanges();

                return Json(new { success = true, message = "Show stopped successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowsController -> StopShow action method.");
                return Json(new { success = false, message = "An error occurred while stopping the show." });
            }
        }
    }
}
