using System;
using Autumn.IOC;
using Autumn.MVC;
using Autumn.Utils;

namespace TestBot
{
	class Program
	{
		static void Main(string[] args)
		{
			var appRoot = new Container(name: "Application");

			var app = appRoot.Add<AppManager>();

			appRoot.Init();

			var hierarchy = FileUtils.BuildWebHierarchy();
			hierarchy.AddRootPaths(app.config.WorkDirectory);
			hierarchy.searchHeight = 0;
			
			var mvc = new MvcContainer(appRoot, smartFileHierarchy: hierarchy);
			mvc.AddPrefixes(app.config.Prefixes);
			mvc.Add<AppFaceController>();
			mvc.Add<AppWebhookController>();
			mvc.AddDefaultFilesComponent();
			mvc.Start();
			
			Console.WriteLine("Press any key to complete application...");
			Console.ReadKey();

			mvc.Stop();
			
			appRoot.Dispose();
		}
	}
}