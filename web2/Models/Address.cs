
using System.ComponentModel;

namespace web2.Models {
	public class Address {

		public string Address1 = string.Empty;
		public string Address2 = string.Empty;
		public string City = string.Empty;
		public string State = string.Empty;
		public string Zip = string.Empty;

		public string FullAddress { //read only
			get {
				if (this.Address1.Length == 0 || this.City.Length == 0 || this.State.Length == 0 || this.Zip.Length == 0) return string.Empty;
				if (this.Address2.Length == 0)
					return string.Concat(this.Address1, ", ", this.City, ", ", this.State, ", ", this.Zip);
				else
					return string.Concat(this.Address1, ", ", this.Address2, ", ", this.City, ", ", this.State, ", ", this.Zip);
			}
		}
	}
}