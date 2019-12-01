using System;
using Autumn.IOC;
using Autumn.MVC;

namespace TestBot
{
	class Program
	{
		static void Main(string[] args)
		{
			var appRoot = new Container(name: "Application");

			var app = appRoot.Add<AppManager>();

			appRoot.Init();

			var mvc = new MvcContainer(appRoot);
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