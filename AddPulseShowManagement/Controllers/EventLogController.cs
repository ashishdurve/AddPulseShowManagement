using Microsoft.AspNetCore.Mvc;

namespace AddPulseShowManagement.Controllers
{
    public class EventLogController : Controller
    {
        public IActionResult EventLog()
        {
            return View();
        }
    }
}
