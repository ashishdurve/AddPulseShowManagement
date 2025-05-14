using AddPulseShowManagement.Common;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Repo;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Google.Cloud.Firestore;
using System.Net.Mail;
using System.Net;

namespace AddPulseShowManagement.Controllers
{
    public class DiplomaController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MSSQLDbContext _context;
        private readonly AppConfig _appConfig;
        private readonly IUserService _userService;
        private readonly ITeamService _teamService;
        private readonly IShowService _showService;
        private readonly IContestantService _conestantService;

        public DiplomaController(ILogger<HomeController> logger,
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

        public IActionResult GetLivePulse(long teamID = 0)
        {
            try
            {
                LivePulse livePulses = new LivePulse();
                long liveHighest = 0, liveLowest = 0, liveAverage = 0, sessionHighest = 0, sessionLowest = 0, sessionAverage = 0;
                bool paused = false, stopped = false;

                Teams team = this._teamService.GetTeamDetails(teamID);
                //List<ContestantsView> cv = this._conestantService.GetTeamData(teamID) ?? new List<ContestantsView>();
                DataSet ds = this._conestantService.GetLivePulseTeamData(teamID) ?? new DataSet();

                if (team.ShowName == "Flukten fra Akershus Festning")
                    livePulses.imageName = "/Images/diploma11.png";
                else if (team.ShowName == "Tors Hammer")
                    livePulses.imageName = "/Images/diploma15.png";
                else
                    livePulses.imageName = "/Images/diploma12.png";

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        long.TryParse(ds.Tables[0].Rows[0]["Highest"].ToString(), out liveHighest);
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
                                int secondsDiff = (int)(DateTime.Now - modifiedDate).TotalSeconds;
                                if (secondsDiff > 0)
                                {
                                    int minutesDiff = secondsDiff / 60;
                                    secondsDiff = secondsDiff % 60;

                                    if (minutesLeft > minutesDiff)
                                    {
                                        //if (secondsDiff < secondsLeft)
                                        //    livePulses.timer = (minutesLeft - minutesDiff).ToString("00") + ":" + (secondsLeft - secondsDiff).ToString("00");
                                        //else
                                        //    livePulses.timer = (minutesLeft - minutesDiff - 1).ToString("00") + ":" + (secondsLeft - secondsDiff + 60).ToString("00");

                                        if (secondsDiff < secondsLeft)
                                            livePulses.timer = (minutesLeft - minutesDiff).ToString("00") + ":" + (secondsLeft - secondsDiff).ToString("00");
                                        else if ((secondsLeft - secondsDiff) == 0)
                                            livePulses.timer = (minutesLeft - minutesDiff).ToString("00") + ":" + (0).ToString("00");
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
                livePulses.liveHighest = liveHighest;
                livePulses.liveLowest = liveLowest;
                livePulses.liveAverage = liveAverage;

                livePulses.sessionLowest = sessionLowest;
                livePulses.sessionHighest = sessionHighest;
                livePulses.sessionAverage = sessionAverage;

                if (team.ShowName == "Flukten fra Akershus Festning")
                {
                    livePulses.imageName = "/Images/diploma11.png";
                    if (!string.IsNullOrEmpty(livePulses.timer))
                    {
                        livePulses.title1 = "FABELAKTIG!";
                        livePulses.title2 = "Laget ditt fullførte rommet med gjenstående tid:";
                    }
                    else
                    {
                        livePulses.title1 = "BRA JOBBET!";
                        livePulses.title2 = "Laget ditt gjorde en kjempeinnsats, \r\n og kom så nære på!";
                    }
                }
                else if (team.ShowName == "Tors Hammer")
                {
                    livePulses.imageName = "/Images/diploma15.png";
                    if (!string.IsNullOrEmpty(livePulses.timer))
                    {
                        livePulses.title1 = "FABELAKTIG!";
                        livePulses.title2 = "Laget ditt fullførte rommet med gjenstående tid:";
                    }
                    else
                    {
                        livePulses.title1 = "BRA JOBBET!";
                        livePulses.title2 = "Laget ditt gjorde en kjempeinnsats, \r\n og kom så nære på!";
                    }
                }
                else
                {
                    livePulses.imageName = "/Images/diploma12.png";
                    if (!string.IsNullOrEmpty(livePulses.timer))
                    {
                        livePulses.title1 = "FABELAKTIG!";
                        livePulses.title2 = "Laget ditt fullførte rommet med gjenstående tid:";
                    }
                    else
                    {
                        livePulses.title1 = "BRA JOBBET!";
                        livePulses.title2 = "Laget ditt gjorde en kjempeinnsats, \r\n og kom så nære på!";
                    }
                }

                //livePulses.timer = "11:30";


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

        public IActionResult GetDiplomaEmailModal(long teamID = 0,string teamName = "")
        {
            try
            {
                //ViewBag.ShowID = showID;
                //Shows show = new Shows();
                //if (showID > 0)
                //{
                //    show = this._context.Shows.FirstOrDefault(y => y.ShowID == showID) ?? new Shows();
                //}

                //ViewBag.show = show;
                //ViewBag.BtnTitle = showID > 0 ? "Update" : "Add";
                //ViewBag.PopTitle = showID > 0 ? "Edit Show" : "Add Show";
                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowController -> GetShowsModal POSt action method.", "");
                return View("login", "account");
            }
        }
        [HttpPost]
        public IActionResult SendDiplomaEmail(string data,string emails)
        {
            try
            {
                data = data.Replace("data:image/jpeg;base64,", string.Empty);
                var bytes =  Convert.FromBase64String(data);
                MemoryStream ms = new MemoryStream(bytes);

                //System.IO.File.WriteAllBytes(@"c:\\Users\\adurve\\Downloads\\test.jpeg", bytes);

                var smtpClient = new SmtpClient("smtp.sendgrid.net")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("apikey", "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"),
                    EnableSsl = true,
                };

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("escapehunt@addpulse.online", "Escape Hunt");

                string[] emailAddresses = emails.Split(',');
                foreach (string emailAddress in emailAddresses)
                {
                    mail.To.Add(emailAddress);
                }
                mail.Subject = "Escape Hunt diploma";
                mail.Body = "Vedlagt diplom";

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(ms, "Diploma.jpeg");
                mail.Attachments.Add(attachment);

                smtpClient.Send(mail);


                return PartialView();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error from ShowController -> GetShowsModal POSt action method.", "");
                return View("login", "account");
            }
        }
    }
}
