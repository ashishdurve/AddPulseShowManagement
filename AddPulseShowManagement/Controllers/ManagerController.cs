using AddPulseShowManagement.Common;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Repo;
using Microsoft.AspNetCore.Mvc;

namespace AddPulseShowManagement.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MSSQLDbContext _context;
        private readonly AppConfig _appConfig;
        private readonly IUserService _userService;
        public ManagerController(ILogger<HomeController> logger,
            MSSQLDbContext context,
            AppConfig appConfig,
            IUserService userService)
        {
            this._appConfig = appConfig;
            this._context = context;
            this._logger = logger;
            this._userService = userService;
        }
        public IActionResult Managers()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetManagers()
        {
            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ManagerController -> GetManagers POSt action method.", "");
                return View("login","account");
            }
        }

        public async Task<IActionResult> GetManagersList( string search="",int draw=0, int start = 0, int length = 10)
        {
            try
            {
                var userlst = await this._userService.GetManagerUsers()??new List<Users>();
                foreach (Users item in userlst)
                {
                    if(!string.IsNullOrEmpty(item.ProfileImage))
                    {
                        item.ProfileImage = _appConfig.fileLocations.UserProfileImage + item.UserID + "/" + item.ProfileImage;
                    }
                    else
                    {
                        item.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }
                }

                ReportData<Users> reportData = new ReportData<Users>();
                reportData.draw = draw;
                reportData.recordsTotal = userlst.Count;
                reportData.data = userlst;

                return Json(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ManagerController -> GetManagers POSt action method.", "");
                return View("login", "account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdateManager([FromForm] Users user )
        {
            try
            {
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserTypeID").Value) : 0;
                if (user.UserID == 0)
                {
                    //string PasswordValue = "";
                    //PasswordValue = HomeController.CreateRandomPasswordWithRandomLength();
                    //if (!string.IsNullOrEmpty(PasswordValue))
                    //{
                    //    user.Password = PasswordValue;
                    //}
                    user.ProfileImage = String.Empty;
                    user.CreatedDate = DateTime.UtcNow;
                    user.CreatedBy = UserID;
                    user.IsActive = true;
                    user.ModifiedDate = DateTime.UtcNow;
                    user.ModifiedBy = UserID;

                }
                else
                {
                    user.ModifiedDate = DateTime.UtcNow;
                    user.ModifiedBy = UserID;
                }
                user.LastName = " ";
                var updated = await this._userService.SaveUpdateUsers(user);
                return Redirect("/Manager/Managers");              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ManagerController -> SaveUpdateManager POSt action method.", "");
                return View("login", "account");
            }
        }

        public IActionResult GetManageManager(long UserID = 0)
        {
            try
            {
                //get user details and bind it in modal
                Users userInfo = new Users();
                if (UserID > 0)
                {
                    userInfo = this._context.Users.FirstOrDefault(y => y.UserID == UserID) ?? new Users();
                }

                if (!string.IsNullOrEmpty(userInfo.ProfileImage))
                {
                    userInfo.ProfileImage = _appConfig.fileLocations.UserProfileImage + userInfo.UserID + "/" + userInfo.ProfileImage;
                }
                else
                {
                    //default path for user image
                    userInfo.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                }
                ViewBag.user = userInfo;
                ViewBag.BtnTitle = UserID > 0 ? "Update" : "Add";
                ViewBag.PopTitle = UserID > 0 ? "Edit Manager" : "Add Manager";
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ManagerController -> GetManagers POSt action method.", "");
                return View("login", "account");
            }
        }

    }
}
