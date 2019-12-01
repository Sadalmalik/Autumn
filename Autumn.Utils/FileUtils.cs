namespace Autumn.Utils
{
	public static class FileUtils
	{
		private static string _currentPath;

		public static string CurrentPath
		{
			get
			{
				if (_currentPath == null)
					_currentPath = System.Reflection.Assembly.GetEntryAssembly().Location;
				return _currentPath;
			}
		}

		public static SmartFileHierarchy BuildGeneralHierarchy()
		{
			var hierarchy = new SmartFileHierarchy();
			hierarchy.searchHeight = 5;
			hierarchy.AddRootPaths(CurrentPath);
			hierarchy.AddExtensionPaths("", "data", "content", "files");
			return hierarchy;
		}
		
		public static SmartFileHierarchy BuildWebHierarchy()
		{
			var hierarchy = new SmartFileHierarchy();
			hierarchy.searchHeight = 5;
			hierarchy.AddGeneralPaths("web");
			hierarchy.AddRootPaths(CurrentPath);
			hierarchy.AddExtensionPaths("", "data", "content", "files");
			hierarchy.AddExtensionPaths(".html", "html", "view");
			hierarchy.AddExtensionPaths(".temp", "html", "view", "templates");
			hierarchy.AddExtensionPaths(".js", "js", "javascript", "scripts");
			hierarchy.AddExtensionPaths(".css", "css", "style", "styles");
			hierarchy.AddExtensionPaths(".jpg", "img", "images");
			hierarchy.AddExtensionPaths(".png", "img", "images");
			hierarchy.AddExtensionPaths(".ico", "img", "images");
			return hierarchy;
		}

		public static SmartFileHierarchy BuildNodeJSHierarchy()
		{
			var hierarchy = new SmartFileHierarchy();
			hierarchy.AddGeneralPaths("modules");
			return hierarchy;
		}
	}
}