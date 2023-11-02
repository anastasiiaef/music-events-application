using System.Web.Mvc;

namespace web2.Controllers{
	public class AboutUsController : Controller {
		// GET: AboutUs
		public ActionResult Index() {

			Models.User u = new Models.User();
			u.FirstName = "Anastasiia";
			u.LastName = "Efimova";
			u.Email = "aefimova@cincinnatistate.edu";

			return View(u);
		}

		[HttpPost]
		public ActionResult Index(FormCollection col) {

			Models.User u = new Models.User();

			if (col["btnSubmit"] == "close") return RedirectToAction("Index", "Home");
			if (col["btnSubmit"] == "more") return RedirectToAction("More", "AboutUs");

			return View(u);
		}

		public ActionResult More() {

			Models.User u = new Models.User();
			u.FirstName = "Anastasiia";
			u.LastName = "Efimova";
			return View(u);
		}

		[HttpPost]
		public ActionResult More(FormCollection col) {

			Models.User u = new Models.User();

			if (col["btnSubmit"] == "close") return RedirectToAction("Index", "AboutUs");
			return View(u);
		}
	}
}

