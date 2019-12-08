using System;
using System.Linq;
using Autumn.IOC;
using Autumn.MVC;
using Autumn.Utils;
using Autumn.Utils.Logger;

namespace TestBot
{
	class Program
	{
		private static bool _active;
		
		private static Container _rootContainer;
		private static MvcContainer _mvcContainer;
		
		static void Main(string[] args)
		{
			bool develop = args.Contains("-dev");
			
			Log.AddDefaultTarget();
			Log.AddDefaultFileTarget();
			Log.Temp("Starting server!");
			
			_rootContainer = new Container(name: "Application");

			var app = _rootContainer.Add<AppManager>();

			_rootContainer.Init();

			var hierarchy = FileUtils.BuildWebHierarchy();
			if (develop)
			{
				hierarchy.AddDefaultRootPath();
				hierarchy.searchHeight = 35;
			}
			else
			{
				hierarchy.AddRootPaths(app.config.WorkDirectory);
				hierarchy.searchHeight = 0;
			}
			
			_mvcContainer = new MvcContainer(_rootContainer, smartFileHierarchy: hierarchy);
			_mvcContainer.AddPrefixes(app.config.Prefixes);
			var face = _mvcContainer.Add<AppFaceController>();
			face.OnExit += Stop;
			_mvcContainer.Add<AppWebhookController>();
			_mvcContainer.AddDefaultFilesComponent();
			_mvcContainer.Start();

			_active = true;
			Console.WriteLine("Press any key to complete application...");
			Console.ReadKey();

			Stop();
		}

		static void Stop()
		{
			if (!_active)
			{
				Log.Temp("Trying stop server multiple times!");
				return;
			}

			_active = false;
			Log.Temp("Stopping server!");
			
			_mvcContainer.Stop();
			
			_rootContainer.Dispose();
		}
	}
}