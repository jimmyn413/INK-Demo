using Sabio.Web.Models.ViewModels;
using Sabio.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sabio.Web.Controllers
{
    [RoutePrefix("events")]
    public class EventsController : BaseController
    {
        // GET: Events
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Route("manage/{eventId:int}")]
        [Route("create")]
        public ActionResult Manage(int? eventId=null)
        {
            ItemViewModel<int?> vm = new ItemViewModel<int?>();
            vm.Item = eventId;
            return View(vm);
        }

        [Authorize]
        public ActionResult Organize()
        {
            return View();
        }

        public ActionResult Discover()
        {
            return View();
        }
        [Authorize]
        public ActionResult Upcoming()
        {
            return View();
        }

        [Authorize]
        public ActionResult OrganizeNG()
        {
            return View();
        }
        

        public ActionResult Discoverng()
        {
            return View();
        }

        public ActionResult Location()
        {
            return View();
        }

        [Route("details/{id:int}")]
        public ActionResult Details(int? id = null)   //  adding "?" to Guid type (or several other types) makes it nullable
        {
            ItemViewModel<int?> vm = new ItemViewModel<int?>();
            vm.Item = id;
            return View("details", vm); //  here we specify the name of the view in case you want to use a different view then  "Manage.cshtml" (the default behavior). this is possible but not required. 
        }

        [Route("tag/{slug}")]
        [Route("tag")]
        public ActionResult EventTags(string slug)
        {
            ItemViewModel<string> vm = new ItemViewModel<string>();
            vm.Item = slug;
            return View(vm);
        }
    }
}