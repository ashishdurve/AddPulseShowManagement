using AddPulseShowManagement.Common;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AddPulseShowManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MSSQLDbContext _context;
        private readonly AppConfig _appConfig;
        public AccountController(ILogger<HomeController> logger,
            MSSQLDbContext context,
            AppConfig appConfig)
        {
            this._appConfig = appConfig;
            this._context = context;
            this._logger = logger;
        }

        #region Login

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.title = "Login";
            return View();
        }

        /// <summary>
        /// This method verifies the user is authenicatation
        /// </summary>
        /// <param name="login">Send usermodel which contains username and password</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost]
        public async Task<int> Login([FromForm] UserModel login)
        {
            try
            {
                //IActionResult response = Unauthorized();
                int response =2;
                var user = AuthenticateUser(login);

                if (user != null)
                {
                    bool IsActiveUser = (bool)(user.IsActive != null ? user.IsActive : false);
                    int IsRemember = !string.IsNullOrEmpty(login.chkRememberMe) ? 1 : 0;
                    if (IsActiveUser)
                    {
                        var tokenString = GenerateJSONWebToken(user, IsRemember);
                        var userIdentity = new ClaimsIdentity(tokenString.Item2, "User Identity");
                        var userPrincipal = new ClaimsPrincipal(new[] { userIdentity });
                        this.Request.HttpContext.User = userPrincipal;
                        await this.Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                        HttpContext.Session.SetInt32("ToggleValue",0); 
                        // response = Redirect("/home/dashboard");
                        response = 1;
                    }
                    else
                    {
                        ViewBag.error = "User inactive. Please activate user and try again.";
                        response = 2;                        
                    }
                }
                else
                {
                    ViewBag.error = "Invalid email or password.";
                    response = 3;
                }
                return response;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from AccountController -> login post action method.", "");
                return 0;
            }
        }

        #region Login Private Methods

        private Users AuthenticateUser(UserModel login)
        {
            try
            {
                UserModel user = new UserModel();
                var _u = this._context.Users.FirstOrDefault(m => m.Email == login.userEmail);

                //return both active and inactive user
                return (_u?.Password == login.userPassword) ? _u : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from  AccountController ->  AuthenticateUser action method.", "");
                return null;
            }

        }


        /// <summary>
        /// Stored user details in claims and session, so they can be used throughout the userlogin
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private (string, List<Claim>) GenerateJSONWebToken(Users userInfo, int IsRemember=0)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._appConfig.jwtConfig.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            if (!string.IsNullOrEmpty(userInfo.ProfileImage))
            {

                userInfo.ProfileImage = _appConfig.fileLocations.UserProfileImage + userInfo.UserID + "/" + userInfo.ProfileImage;
            }
            else
            {
                //default path for user image
                userInfo.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
            }

            int days = IsRemember == 0 ? 1 : 365;

            HttpContext.Session.SetString("UserProfile", userInfo.ProfileImage);
            HttpContext.Session.SetString("FullName", (userInfo.FirstName + " " + userInfo.LastName));
            HttpContext.Session.SetInt32("UserID", String.IsNullOrEmpty(userInfo.UserID.ToString()) ? 0 : Convert.ToInt32(userInfo.UserID));
            string UserFullName = userInfo.FirstName + " " + userInfo.LastName;
            var token = new JwtSecurityToken(
              this._appConfig.jwtConfig.Issuer,
              this._appConfig.jwtConfig.Issuer,
              claims: new Claim[]
              {
                  new Claim(type: "UserID", value: userInfo.UserID.ToString()),                  
                  new Claim(type: "FullName", value: UserFullName),
                  new Claim(type: "FirstName", value: userInfo.FirstName),
                  new Claim(type: "LastName", value: userInfo.FirstName),
                  new Claim(type: "Email", value: userInfo.Email),
                  new Claim(type: "User", value: JsonConvert.SerializeObject(userInfo)),
                  new Claim(type: "UserTypeID", value: userInfo.UserTypeID.ToString()),
                  new Claim(ClaimTypes.Role, value: userInfo.UserTypeID.ToString()),
              },
              expires: DateTime.Now.AddDays(days),
              signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), token.Claims.ToList());
        }

        #endregion

        #endregion

        #region Logout
        /// <summary>
        /// Need to clear the session/terminate the session
        /// </summary>
        /// <returns>View for login page</returns>
        public async Task<IActionResult> Logout()
        {
            await this.Request.HttpContext.SignOutAsync();
            return RedirectToActionPermanent(actionName: "login");
        }
        #endregion

        public IActionResult GetProfileData()
        {
            try
            {
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserID").Value) : 0;
                Users user = new Users();
                if (UserID > 0)
                {
                    user = this._context.Users.FirstOrDefault(y => y.UserID == UserID);
                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        user.ProfileImage = _appConfig.fileLocations.UserProfileImage + user.UserID + "/" + user.ProfileImage;
                    }
                    else
                    {
                        user.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }
                }

                ViewBag.user = user;
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetManagers POSt action method.", "");
                return View("login", "account");
            }

        }

        public IActionResult Profile()
        {
            try
            {
                long UserID = !string.IsNullOrEmpty(User.Claims.First(m => m.Type == "UserID").Value) ? Convert.ToInt32(User.Claims.First(m => m.Type == "UserID").Value) : 0;
                Users user= new Users();
                if (UserID > 0)
                {
                    user = this._context.Users.FirstOrDefault(y => y.UserID == UserID);
                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        user.ProfileImage = _appConfig.fileLocations.UserProfileImage + user.UserID + "/" + user.ProfileImage;
                    }
                    else
                    {
                        user.ProfileImage = _appConfig.fileLocations.DefaultUserLogo;
                    }
                }

                
                ViewBag.user = user;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from TeamsController -> GetManagers POSt action method.", "");
                return View("login", "account");
            }
           
        }
    }
}
