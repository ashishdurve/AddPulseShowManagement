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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MSSQLDbContext _context;
        private readonly AppConfig _appConfig;
        private readonly ITeamService _teamService;
        private readonly IContestantService _conestantService;
        private readonly IUserService _userService;
        public HomeController(ILogger<HomeController> logger,
              MSSQLDbContext context,
            AppConfig appConfig,
            IContestantService conestantService,
              IUserService userService,
            ITeamService teamService)
        {
            this._appConfig = appConfig;
            this._context = context;
            this._logger = logger;
            this._teamService = teamService;
            this._conestantService = conestantService;
            this._userService = userService;
        }

        public IActionResult Dashboard()
        {
            try
            {
                string userEmail = User.Claims.First(m => m.Type == "Email").Value;
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserID").Value) : 0;
                string username = !string.IsNullOrEmpty(HttpContext.Session.GetString("FullName")) ? HttpContext.Session.GetString("FullName") : "";
                //set values in session if values are null
                if (string.IsNullOrEmpty(username))
                {
                    Users user = this._context.Users.FirstOrDefault(y => y.UserID == UserID) ?? new Users();
                    HttpContext.Session.SetString("FullName", user.FirstName + " " + user.LastName);
                    HttpContext.Session.SetInt32("UserID", String.IsNullOrEmpty(user.UserID.ToString()) ? 0 : Convert.ToInt32(user.UserID));
                    HttpContext.Session.SetInt32("ToggleValue", 0);

                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        user.ProfileImage = _appConfig.fileLocations.UserProfileImage + user.UserID + "/" + user.ProfileImage;
                    }
                    else
                    {
                        user.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }
                    HttpContext.Session.SetString("UserProfile", user.ProfileImage);
                }

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


                bool isConfigured = false;
                long showid = 0;
                string uniVal = "1 Hr";
                int version = 0;
                if (configurations != null)
                {
                    isConfigured = true;
                    showid = !string.IsNullOrEmpty(configurations.ShowID.ToString())? configurations.ShowID.Value:0;
                    string unit = configurations.TimePeriodUnit;
                    if (unit.ToUpper().Equals("HH"))
                    {
                        unit = "Hr";
                    }
                    else if (unit.ToUpper().Equals("MINUTE"))
                    {
                        unit = "Mins";
                    }
                    else if (unit.ToUpper().Equals("DD"))
                    {
                        unit = "Days";
                    }
                    else if (unit.ToUpper().Equals("WK"))
                    {
                        unit = "Week";
                    }
                    uniVal = configurations.TimePeriod + " " + unit;

                    version = !string.IsNullOrEmpty(configurations.Version.ToString()) ? configurations.Version.Value : 0;
                }
                else
                {
                    version=  0;
                }

                ViewBag.isConfigured = isConfigured;
                ViewBag.version = version;
                Shows show = _context.Shows.Where(y => y.ShowID == showid).FirstOrDefault() ?? new Shows();

                show.StartDate = !string.IsNullOrEmpty(show.StartDate.ToString()) ? show.StartDate : DateTime.MinValue;
                show.EndDate = !string.IsNullOrEmpty(show.EndDate.ToString()) ? show.EndDate : DateTime.MaxValue;
                ViewBag.show = show;
                ViewBag.uniVal = uniVal;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from HomeController -> Dashboard action method.", "");
                return Redirect("/Account/Login"); 
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult GetDashboard(int isConfigUpdated=1)
        {
            try
            {
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
                if (configurations == null)
                    configurations = _context.DashboardConfigurations.OrderByDescending(x=>x.ConfigID).FirstOrDefault();

                bool isConfigured = false;
                long showid = 0;
                string uniVal = "1 Hr";
                int version = 0;
                if (configurations != null)
                {
                    isConfigured = true;
                    showid = !string.IsNullOrEmpty(configurations.ShowID.ToString()) ? configurations.ShowID.Value : 0;
                    string unit = configurations.TimePeriodUnit;
                    if (unit.ToUpper().Equals("HH"))
                    {
                        unit = "Hr";
                    }
                    else if (unit.ToUpper().Equals("MINUTE"))
                    {
                        unit = "Mins";
                    }
                    else if (unit.ToUpper().Equals("DD"))
                    {
                        unit = "Days";
                    }
                    else if (unit.ToUpper().Equals("WK"))
                    {
                        unit = "Week";
                    }
                    uniVal = configurations.TimePeriod + " " + unit;

                    version = !string.IsNullOrEmpty(configurations.Version.ToString()) ? configurations.Version.Value : 0;
                    configurations.Card1 = configurations.Card1.HasValue ? configurations.Card1.Value : 0;
                    configurations.Card2 = configurations.Card2.HasValue ? configurations.Card2.Value : 0;
                    configurations.Card3 = configurations.Card3.HasValue ? configurations.Card3.Value : 0;
                }
                else
                {
                    version = 0;

                    configurations.Card1 = 0;
                    configurations.Card2 =  0;
                    configurations.Card3 =  0;
                }

                if(configurations.TeamCardID == null)                
                    configurations.TeamCardID = 0;
                if (configurations.TeamCardID2 == null)
                    configurations.TeamCardID2 = 0;
                if(configurations.TeamCardID3 == null)                
                    configurations.TeamCardID3 = 0;


                ViewBag.version = version;
                ViewBag.isConfigured = isConfigured;
                ViewBag.configurations = configurations;

                Shows show = _context.Shows.Where(y => y.ShowID == showid).FirstOrDefault() ?? new Shows();

                show.StartDate = !string.IsNullOrEmpty(show.StartDate.ToString()) ? show.StartDate : DateTime.MinValue;
                show.EndDate = !string.IsNullOrEmpty(show.EndDate.ToString()) ? show.EndDate : DateTime.MaxValue;
                ViewBag.show = show;
                ViewBag.uniVal = uniVal;
                ViewBag.isConfigUpdated = isConfigUpdated;

                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IActionResult Index()
        {
            IActionResult response = Unauthorized();
            
            try
            {
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserID").Value) : 0;
                if (UserID > 0)
                {
                    response = Redirect("/home/dashboard");
                }                
                else
                {                    
                    response = Redirect("/Account/Login");
                }

                return response;
            }
            catch (Exception ex)
            {
                response = Redirect("/Account/Login");
                return response;
            }


        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public static string CreateRandomPasswordWithRandomLength()
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-()+";
            Random random = new Random();

            // Minimum size 8. Max size is number of all allowed chars.  
            int size = random.Next(8, validChars.Length);

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }


        public IActionResult VerifyExistingEmail(string email = "")
        {
            try
            {
                bool IsVaild = false;
                IsVaild = _context.Users.Any(y => y.Email.ToLower().Equals(email.ToLower()));   
                return Json(IsVaild);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from HomeController -> VerifyEmail action method.", "");
                return Json(false);
            }
        }


        public IActionResult LivePulse()
        {
            try
            {

                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                long showid = 0;
                List<long> teamid = new List<long>();
                IDictionary<long, string> teams = new Dictionary<long, string>();
                if (configurations != null)
                {
                    showid = (long)configurations.ShowID;
                    teamid = _context.Teams.Where(y => y.ShowID == showid && y.IsActive == true).Select(d => d.TeamID).ToList();
                    // teams = _context.Teams.Where(y => y.ShowID == showid).ToList().ForEach(x=> { _context[x.TeamID] = x.TeamName });
                    foreach (long item in teamid)
                    {
                        Teams t = _context.Teams.Where(y => y.TeamID == item).FirstOrDefault()??new Teams();
                        teams.Add(new KeyValuePair<long, string>(t.TeamID, t.TeamName));                      

                    }
                }

                int refreshTime = configurations.RefreshTimePeriod.HasValue ? configurations.RefreshTimePeriod.Value : 5;


                ViewBag.refreshTimeVal = refreshTime * 1000;

                List<LivePulseView> members = this._conestantService.GetLivePulseData(showid) ?? new List<LivePulseView>();
                ViewBag.teamid = teamid;
                ViewBag.teams = teams;
                ViewBag.members = members;
                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public IActionResult UpdateLivePulse()
        {
            try
            {

                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                long showid = 0;
                List<long> teamid = new List<long>();
                if (configurations != null)
                {
                    showid = (long)configurations.ShowID;
                    teamid = _context.Teams.Where(y => y.ShowID == showid).Select(d => d.TeamID).ToList();
                }

                List<LivePulseView> members = this._conestantService.GetLivePulseData(showid) ?? new List<LivePulseView>();
                ViewBag.teamid = teamid;
                ViewBag.members = members;
                return Json(new { members= members, teamid= teamid });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IActionResult SaveCard(int cardid=1,int cardVal=4,long teamID=0,long showID=0,string unitID="HH",long timeInseconds=3600,int timeperiod =1,int refreshTime=5)
        {
            DashboardConfigurations configurations;
            try
            {
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserID").Value) : 0;
                configurations = _context.DashboardConfigurations.Where(y => y.UserID == UserID).FirstOrDefault();

                if(configurations == null)
                  configurations= _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                //if (UserID > 0 && configurations == null)
                //{
                //    configurations.ConfigID = 0;
                //}

                bool isSaved = this._conestantService.SaveCardData(UserID,showID,cardid,cardVal,teamID, configurations.ConfigID,unitID,timeInseconds,timeperiod,refreshTime);
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from HomeController -> SaveCard action method.", "");
                return Json(false);
            }
        }

        
        public IActionResult SavePlayerSequence(string PlayerIDs="",int noTeams=1)
        {
            try
            {

                //save data in configuration
                List<string> playerID=new List<string>();
                if(!string.IsNullOrEmpty(PlayerIDs))
                {
                    playerID = PlayerIDs.Split("|").ToList();
                    //save data in PlayerVsPlayers
                    int iindex = 1;
                    bool saved = this._conestantService.SavePlayerVsPlayer(playerID.Count);
                    foreach (string item in playerID)
                    {
                        if (!string.IsNullOrEmpty(item) && (!string.Equals(",",item)))
                        {
                            bool playersaved = this._conestantService.SavePlayersSequence(1, iindex, item);
                            iindex = iindex + 1;

                            //save data in PlayersSequence
                        }
                    }

                }

                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from HomeController -> SavePlayerSequence action method.", "");
                return Redirect("/Account/Login");
            }
        }


        public IActionResult EditProfileModal(long Userid=0)
        {
            try
            {
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserID").Value) : 0;
                Users user = new Users();
                //if (UserID > 0)
                //{
                //    user = this._context.Users.FirstOrDefault(y => y.UserID == UserID);
                //    if (!string.IsNullOrEmpty(user.ProfileImage))
                //    {
                //        user.ProfileImage = _appConfig.fileLocations.UserProfileImage + user.UserID + "/" + user.ProfileImage;
                //    }
                //    else
                //    {
                //        user.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                //    }
                //}

                string profilePath = "";
                if (UserID > 0)
                {
                    user = this._context.Users.FirstOrDefault(y => y.UserID == UserID);
                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        profilePath = _appConfig.fileLocations.UserProfileImage + user.UserID + "/" + user.ProfileImage;
                    }
                    else
                    {
                        profilePath = _appConfig.fileLocations.DefaultUserLogo;
                    }
                }

                ViewBag.profilePath = profilePath;
                ViewBag.user = user;

                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from HomeController -> EditProfileModal action method.", "");
                return Redirect("/Account/Login");
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveUpdateUser([FromForm] Users user)
        {
            try
            {
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserTypeID").Value) : 0;
               
                user.ModifiedDate = DateTime.UtcNow;
                user.ModifiedBy = UserID;
                user.IsActive = true;
                user.LastName = " ";
                var updated = await this._userService.SaveUpdateUsers(user);
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ManagerController -> SaveUpdateManager POSt action method.", "");
                return View("login", "account");
            }
        }


        public IActionResult VerifyConfigurationChange(int version=0)
        {
            try
            {
                DashboardConfigurations configurations = _context.DashboardConfigurations.OrderByDescending(x => x.ConfigID).FirstOrDefault();
                bool isUpdated = false;
                if (version != configurations.Version)
                {
                    isUpdated = true;
                }
                int sVersion = configurations.Version.HasValue ? configurations.Version.Value : 0;
                return Json(new { isUpdated, sVersion });
            }
            catch (Exception  ex)
            {
                return Json(false);
            }
        }

        public IActionResult SetMenuToggle(int btnval=0)
        {
            try
            {
                int fval = 0;
                int val = HttpContext.Session.GetInt32("ToggleValue").HasValue ? HttpContext.Session.GetInt32("ToggleValue").Value : 0;
                if(val == 1)
                {
                    HttpContext.Session.SetInt32("ToggleValue", 0);
                    fval = 0;
                }
                else
                {
                    HttpContext.Session.SetInt32("ToggleValue", 1);
                    fval = 1;
                }

               
                return Json(fval);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        //public void FirebaseMethod()
        //{
        //    string filepath = this._appConfig.fireBase.FireBaseEnvironment;
        //    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);

        //    FirestoreDb db = FirestoreDb.Create("add-pulse-show");

        //    //DocumentReference docRef = db.Collection("Players").Document("SF");
        //    CollectionReference docRef = db.Collection("Players");
        //    Query query = db.Collection("Players").OrderBy("PlayerID");
        //    //FirestoreChangeListener listener = query.Listen(snapshot =>
        //    //{
        //    //    Console.WriteLine("Callback received document snapshot.");
        //    //    Console.WriteLine("Document exists? {0}", snapshot.Exists);
        //    //    if (snapshot.Exists)
        //    //    {
        //    //        Console.WriteLine("Document data for {0} document:", snapshot.Id);
        //    //        Dictionary<string, object> city = snapshot.ToDictionary();
        //    //        foreach (KeyValuePair<string, object> pair in city)
        //    //        {
        //    //            Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
        //    //        }
        //    //    }
        //    //}); 

        //    FirestoreChangeListener listener = query.Listen(snapshot =>
        //    {
        //        foreach (DocumentChange change in snapshot.Changes)
        //        {
        //            if (change.ChangeType.ToString() == "Added")
        //            {
        //                Console.WriteLine("New city: {0}", change.Document.Id);
        //            }
        //            else if (change.ChangeType.ToString() == "Modified")
        //            {
        //                Console.WriteLine("Modified city: {0}", change.Document.Id);
        //            }
        //            else if (change.ChangeType.ToString() == "Removed")
        //            {
        //                Console.WriteLine("Removed city: {0}", change.Document.Id);
        //            }
        //        }
        //    });

        //}
    }
}