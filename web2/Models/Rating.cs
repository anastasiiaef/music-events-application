
namespace web2.Models {
	public class Rating {
		public long RateID = 0;
		public long UID = 0;
		public long ID = 0;
		public byte Rate = 0;
		public Types Type = Types.NoType;

		public enum Types {
			NoType = 0,
			Event = 1,
			User = 2,
			Image = 3
		}
	}
}