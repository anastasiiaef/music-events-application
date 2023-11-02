
namespace web2.Models {
	public class Like {

		public long ID = 0;
		public Types Type = Types.NoType;
		
		public enum Types {
			NoType = 0,
			Event = 1,
			User = 2,
			Image = 3
		}
	}
}