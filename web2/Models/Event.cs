using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace web2.Models {
	public class Event {
		public long ID = 0;
		public string Title = string.Empty;
		public string Description = string.Empty;
		public DateTime Start;
		public DateTime End;
		public User User;
		public Location Location;
		public List<Image> Images;
		public Image EventImage;
		public ActionTypes ActionType = ActionTypes.NoType;
		public bool IsActive = true;
		public int AverageRating = 0;
		public int TotalLikes = 0;

		public Image PrimaryImage {
			get {
				if (this.Images != null) {
					foreach (Image i in this.Images) {
						if (i.Primary) return i;
					}
				}
				return new Image();
			}
		}

		public bool Editable {
			get {
				if (this.Start == null) return true;
				if (this.Start > DateTime.Now) return true;
				return false;
			}
		}

		public Event GetEvent(long ID) {
			try {
				Database db = new Database();
				List<Event> events = new List<Event>();
				if (this.User == null) {
					events = db.GetEvents(ID);
				}
				else {
					events = db.GetEvents(ID, this.User.UID);
				}
				return events[0];
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public Event.ActionTypes Save() {
			try {
				Database db = new Database();
				if (ID == 0) { //insert new user
					this.ActionType = db.InsertEvent(this);
				}
				else {
					this.ActionType = db.UpdateEvent(this);
				}
				return this.ActionType;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public sbyte AddEventImage(HttpPostedFileBase f) {
			try {
				this.EventImage = new Image();
				this.EventImage.Primary = false;
				this.EventImage.FileName = Path.GetFileName(f.FileName);

				if (this.EventImage.IsImageFile()) {
					this.EventImage.Size = f.ContentLength;
					Stream stream = f.InputStream;
					BinaryReader binaryReader = new BinaryReader(stream);
					this.EventImage.ImageData = binaryReader.ReadBytes((int)stream.Length);
					this.UpdatePrimaryImage();
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }

		}


		public sbyte UpdatePrimaryImage() {
			try {
				Database db = new Database();
				long NewID;
				if (this.EventImage.ImageID == 0) {
					NewID = db.InsertEventImage(this);
					if (NewID > 0) EventImage.ImageID = NewID;
				}
				else {
					db.UpdateEventImage(this);
				}
				return 0;
			}
			catch (Exception ex) { throw new Exception(ex.Message); }
		}

		public enum ActionTypes {
			NoType = 0,
			InsertSuccessful = 1,
			UpdateSuccessful = 2,
			DuplicateEmail = 3,
			DuplicateUserID = 4,
			Unknown = 5,
			RequiredFieldsMissing = 6
		}


	}
}
