using System.Web.Optimization;

namespace web2 {
	public class BundleConfig {
		public static void RegisterBundles(BundleCollection bundles) {
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/web2.js",
						"~/Scripts/modernizr-{version}.js",
						"~/Scripts/jquery.filedrop.js",
						"~/Scripts/jquery-{version}.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
						"~/Content/site.css"));
		}
	}
}
