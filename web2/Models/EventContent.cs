
namespace web2.Models {
	public class EventContent {
		public Event Event;
		public User User;

		public bool CurrentUserIsOwner {
			get {
				if (Event == null) return false;
				if (Event.User == null) return false;
				if (User == null) return false;
				if (User.UID != Event.User.UID) return false;
				return true;
			}
		}
	}
}