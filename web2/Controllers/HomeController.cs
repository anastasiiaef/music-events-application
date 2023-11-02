using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace web2.Controllers {
	public class HomeController : Controller {
		public ActionResult Index() {
			Models.Database db = new Models.Database();
			Models.HomeContent h = new Models.HomeContent();

			h.Events = new List<Models.Event>();
			h.Events = db.GetActiveEvents();

			h.User = new Models.User();
			h.User = h.User.GetUserSession();

			if (h.User.IsAuthenticated) {
				h.User.Likes = db.GetEventLikes(h.User.UID);
				h.User.Ratings = db.GetEventRatings(h.User.UID);
			}
			return View(h);
		}

		public ActionResult Event() {
			Models.EventContent ec = new Models.EventContent();
			Models.Database db = new Models.Database();

			ec.User = new Models.User();
			ec.User = ec.User.GetUserSession();

			if (ec.User.IsAuthenticated) {
				ec.User.Likes = db.GetEventLikes(ec.User.UID);
				ec.User.Ratings = db.GetEventRatings(ec.User.UID);
			}

			long id = Convert.ToInt64(RouteData.Values["id"]);
			ec.Event = new Models.Event();
			ec.Event = ec.Event.GetEvent(id);

			return View(ec);
		}

		[HttpPost]
		public ActionResult Event(FormCollection col) {
			//close button
			return RedirectToAction("Index");
		}



		[HttpPost]
		public JsonResult SaveReport(long UID, long IDToReport, int ProblemID) {
			try {
				Models.Database db = new Models.Database();
				System.Threading.Thread.Sleep(3000);
				bool b = false;
				b = db.InsertReport(UID, IDToReport, ProblemID);
				return Json(new { Status = b });
			}
			catch (Exception ex) {
				return Json(new { Status = -1 }); //error
			}
		}




		[HttpPost]
		public JsonResult ToggleEventLike(long UID, long ID) {
			try {
				Models.Database db = new Models.Database();
				int intReturn = 0;
				intReturn = db.ToggleEventLike(UID, ID);
				return Json(new { Status = intReturn });
			}
			catch (Exception ex) {
				return Json(new { Status = -1 }); //error
			}
		}

		[HttpPost]
		public JsonResult RateEvent(long UID, long ID, long Rating) {
			try {
				Models.Database db = new Models.Database();
				int intReturn = 0;
				intReturn = db.RateEvent(UID, ID, Rating);
				return Json(new { Status = intReturn });
			}
			catch (Exception ex) {
				return Json(new { Status = -1 }); //error
			}
		}
	}
}