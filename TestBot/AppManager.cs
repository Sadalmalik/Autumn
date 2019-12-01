using System;
using System.IO;
using Autumn.IOC;
using Autumn.Utils;
using Newtonsoft.Json;

namespace TestBot
{
	public class AppConfig
	{
		public string[] Prefixes;
		public string BitmexKey;
		public string BitmexSecret;
	}
	
	public class AppManager : SharedObject
	{
		private SmartFileHierarchy _fileHierarchy;

		public AppConfig config;
		public int       indexCalls   = 0;
		public int       webhookCalls = 0;
		
		public override void Init()
		{
			_fileHierarchy = FileUtils.BuildGeneralHierarchy();

			var path = _fileHierarchy.FindFile("config.json");

			var rawConfig = path != null ? File.ReadAllText(path) : "{\"prefixes\":[]}";

			Console.WriteLine($"Parse config: {rawConfig}");
			
			config = (AppConfig) JsonConvert.DeserializeObject(rawConfig, typeof(AppConfig));
		}

		public override void Dispose()
		{
		}
	}
}