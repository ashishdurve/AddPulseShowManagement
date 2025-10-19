using AddPulseShowManagement.Common;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Data.ViewModels;
using AddPulseShowManagement.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AddPulseShowManagement.Controllers
{
    public class ContestantsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MSSQLDbContext _context;
        private readonly AppConfig _appConfig;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IShowService _showService;
        private readonly IContestantService _contestantService;

        public ContestantsController(ILogger<HomeController> logger,
               MSSQLDbContext context,
               AppConfig appConfig,
               IUserService userService,
               ITeamService teamService,
               IContestantService contestantService,
               IShowService showService)
        {
            this._appConfig = appConfig;
            this._context = context;
            this._logger = logger;
            this._userService = userService;
            this._teamService = teamService;
            this._showService = showService;
            this._contestantService = contestantService;
        }

        public IActionResult Contestants()
        {
            try
            {
                List<Shows> shows = this._showService.GetShows();
                ViewBag.shows = shows;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ContestantsController -> Contestants action method.", "");
                return View("login", "account");
            }
            
        }

        public async Task<IActionResult> GetContestantsList(string search = "", int draw = 0, int start = 0, int length = 10, long showID = 0, string startDate = "", string endDate = "")
        {
            try
            {
                var teamlst = this._contestantService.GetContestantList() ?? new List<Contestants>();

                ReportData<Contestants> reportData = new ReportData<Contestants>();
                reportData.draw = draw;
                reportData.recordsTotal = teamlst.Count;
                reportData.data = teamlst;

                return Json(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetContestantsList action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult GetContestantTableView(long showID = 0)
        {
            try
            {
                ViewBag.showID = showID;
                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                int refreshTime = configurations.RefreshTimePeriod.HasValue ? configurations.RefreshTimePeriod.Value : 5;

                ViewBag.refreshTime = refreshTime;
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ContestantController -> GetContestantTableView action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult GetContestantCardView(long showID = 0, string search = "")
        {
            try
            {
                ViewBag.showID = showID;
                List<ContestantViewModel> memberlst = this._contestantService.GetContestantsData(showID, search) ?? new List<ContestantViewModel>();

                foreach (ContestantViewModel item in memberlst)
                {
                    if (!string.IsNullOrEmpty(item.ProfileImage))
                    {
                        item.ProfileImage = _appConfig.fileLocations.ContestantImage + item.ContestantID + "/" + item.ProfileImage;
                    }
                    else
                    {
                        item.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }

                    item.Gval =!string.IsNullOrEmpty(item.Gval) ? item.Gval.TrimEnd(','):"";
                }

                List<string> gdates = new List<string>();
                DateTime stdate = DateTime.UtcNow.AddDays(-6);
                DateTime enddate = DateTime.UtcNow;

                gdates = Extensions.GetDaysDatesBetween(stdate, enddate);

                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                long points = 3600;
                long Tpoints = 3600;

                if (configurations != null)
                {
                    points = configurations.TimeInSeconds.HasValue ? configurations.TimeInSeconds.Value : 3600;
                }

                int refreshTime = configurations.RefreshTimePeriod.HasValue ? configurations.RefreshTimePeriod.Value : 5;


                if (configurations != null)
                {
                    points = configurations.TimeInSeconds.HasValue ? configurations.TimeInSeconds.Value : 3600;
                    Tpoints = (int)(points / refreshTime);
                }

                ViewBag.points = Tpoints;
                ViewBag.refreshTimeVal = refreshTime * 1000;

                ViewBag.gdates = string.Join(",", gdates);
                ViewBag.memberlst = memberlst;

                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ContestantController -> GetContestantTableView action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult UpdateContestantCardView(long showID = 0, string search = "")
        {
            try
            {
                ViewBag.showID = showID;
                List<ContestantViewModel> memberlst = this._contestantService.GetContestantsData(showID, search) ?? new List<ContestantViewModel>();

                foreach (ContestantViewModel item in memberlst)
                {
                    if (!string.IsNullOrEmpty(item.ProfileImage))
                    {
                        item.ProfileImage = _appConfig.fileLocations.ContestantImage + item.ContestantID + "/" + item.ProfileImage;
                    }
                    else
                    {
                        item.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }

                    item.Gval = !string.IsNullOrEmpty(item.Gval) ? item.Gval.TrimEnd(',') : "";
                }


                ViewBag.memberlst = memberlst;

                return Json(new { memberlst, showID });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ContestantController -> GetContestantTableView action method.", "");
                return View("login", "account");
            }
        }

        public async Task<IActionResult> GetContestantTable(long showID=0)
        {
            try
            {
                List<ContestantViewModel> memberlst = this._contestantService.GetContestantsData(showID) ?? new List<ContestantViewModel>();
                foreach (ContestantViewModel item in memberlst)
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
                ReportData<ContestantViewModel> reportData = new ReportData<ContestantViewModel>();
                reportData.draw = 10;
                reportData.recordsTotal = memberlst.Count;
                reportData.data = memberlst;

                return Json(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetContestantsList action method.", "");
                return View("login", "account");
            }
        }

        public async Task<IActionResult> GetUpdateContestantTable(long showID = 0)
        {
            try
            {
                List<ContestantViewModel> memberlst = this._contestantService.GetContestantsData(showID) ?? new List<ContestantViewModel>();
                foreach (ContestantViewModel item in memberlst)
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

                return Json(memberlst);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetContestantsList action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult DeleteContestant(long contestantID = 0)
        {
            SqlConnection con = new SqlConnection(this._appConfig.connectionStrings.Default);
            SqlCommand cmd = new SqlCommand("APS_01_DeleteContestant", con);
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.Add("@ContestantID", SqlDbType.BigInt).Value = contestantID;           


            con.Open();
            int isRows = cmd.ExecuteNonQuery();

            con.Close();

            return Json("");
        }

        [HttpPost]
        public IActionResult UpdateNormalPulse(long contestantID, int normalPulse)
        {
            try
            {
                var contestant = _context.Contestants.FirstOrDefault(c => c.ContestantID == contestantID);
                
                if (contestant == null)
                {
                    return Json(new { success = false, message = "Contestant not found." });
                }

                contestant.NormalPulse = normalPulse;
                contestant.ModifiedDate = DateTime.UtcNow;
                // Optionally set ModifiedBy if you have user context
                // contestant.ModifiedBy = GetCurrentUserId();
                
                _context.SaveChanges();

                return Json(new { success = true, message = "Normal pulse updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ContestantsController -> UpdateNormalPulse action method.", "");
                return Json(new { success = false, message = "An error occurred while updating normal pulse." });
            }
        }
    }
}
