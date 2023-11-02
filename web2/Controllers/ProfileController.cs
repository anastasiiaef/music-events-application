using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace web2.Controllers {
	public class ProfileController : Controller {

		public ActionResult Event() {
			Models.User u = new Models.User();
			Models.Event e = new Models.Event();
			u = u.GetUserSession();
			e.User = u;

			if (e.User.IsAuthenticated) {
				if (RouteData.Values["id"] == null) { //add an empty event
					e.Start = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0);
					e.End = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);
				}
				else { //get the event
					long id = Convert.ToInt64(RouteData.Values["id"]);
					e = e.GetEvent(id);
				}
			}
			return View(e);
		}

		[HttpPost]
		public ActionResult Event(HttpPostedFileBase EventImage, FormCollection col) {
			Models.User u = new Models.User();
			u = u.GetUserSession();

			if (col["btnSubmit"] == "close") {
				if (col["from"] == null) return RedirectToAction("MyEvents");
				return RedirectToAction("Index", "Home");
			}

			if (col["btnSubmit"] == "event-gallery") {
				return RedirectToAction("EventGallery", new { @id = Convert.ToInt64(RouteData.Values["id"]) });
			}

			if (col["btnSubmit"] == "delete") {
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				return RedirectToAction("DeleteEvent", new { @id = lngID });
			}

			if (col["btnSubmit"] == "save") {

				Models.Event e = new Models.Event();

				if (RouteData.Values["id"] != null) e.ID = Convert.ToInt64(RouteData.Values["id"]);
				e.User = u;
				e.Title = col["Title"];
				if (col["IsActive"].ToString().Contains("true")) e.IsActive = true; else e.IsActive = false;
				e.Description = col["Description"];

				e.Start = DateTime.Parse(string.Concat(col["Start"].ToString(), " ", col["Start.TimeOfDay"]));
				e.End = DateTime.Parse(string.Concat(col["End"].ToString(), " ", col["End.TimeOfDay"]));

				e.Location = new Models.Location();
				e.Location.Title = col["Location.Title"];
				e.Location.Description = col["Location.Description"];

				e.Location.Address = new Models.Address();
				e.Location.Address.Address1 = col["Location.Address.Address1"];
				e.Location.Address.Address2 = col["Location.Address.Address2"];
				e.Location.Address.City = col["Location.Address.City"];
				e.Location.Address.State = col["Location.Address.State"];
				e.Location.Address.Zip = col["Location.Address.Zip"];

				if (e.Title.Length == 0 || e.Description.Length == 0 || e.Location.Title.Length == 0) {
					e.ActionType = Models.Event.ActionTypes.RequiredFieldsMissing;
					return View(e);
				}

				e.Save();

				if (EventImage != null) {
					e.EventImage = new Models.Image();
					if (col["EventImage.ImageID"].ToString() == "") {
						e.EventImage.ImageID = 0;
					}
					else {
						e.EventImage.ImageID = Convert.ToInt32(col["EventImage.ImageID"]);
					}

					e.EventImage.Primary = true;
					e.EventImage.FileName = Path.GetFileName(EventImage.FileName);
					if (e.EventImage.IsImageFile()) {
						e.EventImage.Size = EventImage.ContentLength;
						Stream stream = EventImage.InputStream;
						BinaryReader binaryReader = new BinaryReader(stream);
						e.EventImage.ImageData = binaryReader.ReadBytes((int)stream.Length);

						e.UpdatePrimaryImage();
					}
				}

				if (e.ID > 0) {
					return RedirectToAction("Event", new { @id = e.ID });
				}
			}
			return View();
		}

		public ActionResult EventGallery() {
			Models.Event e = new Models.Event();
			Models.User u = new Models.User();
			u = u.GetUserSession();
			e.User = u;

			if (e.User.IsAuthenticated) {
				Models.Database db = new Models.Database();
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				e = e.GetEvent(lngID);
				e.Images = db.GetEventImages(lngID);
			}
			return View(e);
		}

		[HttpPost]
		public ActionResult EventGallery(IEnumerable<HttpPostedFileBase> files) {
			Models.Event e = new Models.Event();
			e.User = new Models.User();
			e.User = e.User.GetUserSession();
			e.ID = Convert.ToInt64(RouteData.Values["id"]);
			foreach (var file in files) {
				e.AddEventImage(file);
			}
			return Json("file(s) uploaded successfully");
		}

		[HttpPost]
		public JsonResult DeleteEventImage(long UID, long ID) {
			try {
				string type = string.Empty;
				Models.Database db = new Models.Database();
				if (db.DeleteEventImage(ID)) return Json(new { Status = 1 }); //deleted
				return Json(new { Status = 0 }); //not deleted
			}
			catch (Exception ex) {
				return Json(new { Status = -1 }); //error
			}
		}

		public ActionResult DeleteEvent() {
			Models.Event e = new Models.Event();
			e.User = new Models.User();
			e.User = e.User.GetUserSession();
			if (e.User.IsAuthenticated) {
				long lngID = Convert.ToInt64(RouteData.Values["id"]);
				e = e.GetEvent(lngID);
			}
			return View(e);
		}

		[HttpPost]
		public ActionResult DeleteEvent(FormCollection col) {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated) {
				long lngID = Convert.ToInt64(RouteData.Values["id"]);

				if (col["btnSubmit"] == "close") return RedirectToAction("Event", new { @id = lngID });
				if (col["btnSubmit"] == "delete") {
					Models.Database db = new Models.Database();
					db.DeleteEvent(lngID);
				}
			}
			return RedirectToAction("MyEvents"); //this should never happen
		}

		public ActionResult Gallery() {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated) {
				Models.Database db = new Models.Database();
				u.Images = db.GetUserImages(u.UID);
			}
			return View(u);
		}

		[HttpPost]
		public ActionResult Gallery(IEnumerable<HttpPostedFileBase> files) {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			foreach (var file in files) {
				u.AddGalleryImage(file);
			}
			return Json("file(s) uploaded successfully");
		}

		[HttpPost]
		public JsonResult DeleteImage(long UID, long ID) {
			try {
				string type = string.Empty;
				Models.Database db = new Models.Database();
				if (db.DeleteUserImage(ID)) return Json(new { Status = 1 }); //deleted
				return Json(new { Status = 0 }); //not deleted
			}
			catch (Exception) {
				return Json(new { Status = -1 }); //error
			}
		}

		public ActionResult MyEvents() {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated)
				u.Events = u.GetEvents();
			return View(u);
		}
		public ActionResult Index() {
			Models.User u = new Models.User();
			u = u.GetUserSession();
			if (u.IsAuthenticated) {
				Models.Database db = new Models.Database();
				List<Models.Image> images = new List<Models.Image>();
				images = db.GetUserImages(u.UID, 0, true);
				u.UserImage = new Models.Image();
				if (images.Count > 0) u.UserImage = images[0];
			}
			return View(u);
		}

		[HttpPost]
		public ActionResult Index(HttpPostedFileBase UserImage, FormCollection col) {
			try {
				Models.User u = new Models.User();
				u = u.GetUserSession();

				u.FirstName = col["FirstName"];
				u.LastName = col["LastName"];
				u.Email = col["Email"];
				u.UserID = col["UserID"];
				u.Password = col["Password"];

				if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.UserID.Length == 0 || u.Password.Length == 0) {
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else {
					if (col["btnSubmit"] == "update") { //update button pressed
						u.Save();

						u.UserImage = new Models.Image();
						u.UserImage.ImageID = System.Convert.ToInt32(col["UserImage.ImageID"]);

						if (UserImage != null) {
							u.UserImage = new Models.Image();
							u.UserImage.ImageID = Convert.ToInt32(col["UserImage.ImageID"]);
							u.UserImage.Primary = true;
							u.UserImage.FileName = Path.GetFileName(UserImage.FileName);
							if (u.UserImage.IsImageFile()) {
								u.UserImage.Size = UserImage.ContentLength;
								Stream stream = UserImage.InputStream;
								BinaryReader binaryReader = new BinaryReader(stream);
								u.UserImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
								u.UpdatePrimaryImage();
							}
						}

						u.SaveUserSession();
						return RedirectToAction("Index");
					}
					return View(u);
				}
			}
			catch (Exception) {
				Models.User u = new Models.User();
				return View(u);
			}
		}

		public ActionResult SignIn() {
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignIn(FormCollection col) {
			try {
				Models.User u = new Models.User();
					u.UserID = col["UserID"];
					u.Password = col["Password"];
					if (u.UserID.Length == 0 || u.Password.Length == 0) {
						u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
						return View(u);
					}else{
					if (col["btnSubmit"] == "signin") 
						u = u.Login();
						if (u != null && u.UID > 0) {
							u.SaveUserSession();
							return RedirectToAction("Index");
						}
					else {
						u = new Models.User();
						u.UserID = col["UserID"];
						u.ActionType = Models.User.ActionTypes.LoginFailed;
					}
				}
				return View(u);
			}
			catch (Exception) {
				Models.User u = new Models.User();
				return View(u);
			}
		}
		public ActionResult SignUp() {
			Models.User u = new Models.User();
			return View(u);
		}

		[HttpPost]
		public ActionResult SignUp(FormCollection col) {
			try {
				Models.User u = new Models.User();

				u.FirstName = col["FirstName"];
				u.LastName = col["LastName"];
				u.Email = col["Email"];
				u.UserID = col["UserID"];
				u.Password = col["Password"];

				if (u.FirstName.Length == 0 || u.LastName.Length == 0 || u.Email.Length == 0 || u.UserID.Length == 0 || u.Password.Length == 0) {
					u.ActionType = Models.User.ActionTypes.RequiredFieldsMissing;
					return View(u);
				}
				else {
					if (col["btnSubmit"] == "signup") { //sign up button pressed
						Models.User.ActionTypes at = Models.User.ActionTypes.NoType;
						at = u.Save();
						switch (at) {
							case Models.User.ActionTypes.InsertSuccessful:
								u.SaveUserSession();
								return RedirectToAction("Index");
							//break;
							default:
								return View(u);
								//break;
						}
					}
					else {
						return View(u);
					}
				}
			}
			catch (Exception) {
				Models.User u = new Models.User();
				return View(u);
			}
		}
		public ActionResult SignOut() {
			Models.User u = new Models.User();
			u.RemoveUserSession();
			return RedirectToAction("Index", "Home");
		}
	}
}