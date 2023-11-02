
namespace web2.Models {
	public class Report {
		public long RowID = 0;
		public long UID = 0;
		public long IDToReport = 0;
		public bool Resolved = false;
		public ProblemTypes ProblemType = ProblemTypes.NoType;

		public string ProblemText {
			get {
				switch (ProblemType) {
					case ProblemTypes.MissleadingorScam:
						return "Missleading or Scam";
					case ProblemTypes.SexuallyInappropriate:
						return "Sexually Inappropriate";
					case ProblemTypes.Offensive:
						return "Offensive";
					case ProblemTypes.Violent:
						return "Violent";
					case ProblemTypes.Spam:
						return "Spam";
					default: //NoType or anything else
						return "Not set";
				}
			}
		}

		public static string GetProblemText(ProblemTypes pt) {
			switch (pt) {
				case ProblemTypes.MissleadingorScam:
					return "Missleading or Scam";
				case ProblemTypes.SexuallyInappropriate:
					return "Sexually Inappropriate";
				case ProblemTypes.Offensive:
					return "Offensive";
				case ProblemTypes.Violent:
					return "Violent";
				case ProblemTypes.Spam:
					return "Spam";
				default: //NoType or anything else
					return "Not set";
			}
		}

		public enum ProblemTypes {
			NoType = 0,
			MissleadingorScam = 1,
			SexuallyInappropriate = 2,
			Offensive = 3,
			Violent = 4,
			Spam = 5
		}
	}
}