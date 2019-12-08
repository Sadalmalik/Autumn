using System;
using System.Collections.Generic;
using System.IO;
using Autumn.IOC;
using Autumn.Utils;
using Autumn.Utils.Logger;
using Newtonsoft.Json;

namespace TestBot
{
	public class AppConfig
	{
		public string WorkDirectory;
		public string ServerSecret;
		public string[] Prefixes;
		
		public string BitmexKey;
		public string BitmexSecret;
	}

	public class WebhookLog
	{
		public DateTime date;
		public string log;
	}
	
	public class AppManager : SharedObject
	{
		private SmartFileHierarchy _fileHierarchy;

		public AppConfig config;
		public int       indexCalls   = 0;
		public int       webhookCalls = 0;
		
		public List<WebhookLog> webhooks;
		
		public override void Init()
		{
			webhooks = new List<WebhookLog>();
			
			_fileHierarchy = FileUtils.BuildGeneralHierarchy();

			var path = _fileHierarchy.FindFile("config.json");

			var rawConfig = path != null ? File.ReadAllText(path) : "{\"prefixes\":[]}";

			Log.Temp($"Parse config:\n{rawConfig}");
			
			config = (AppConfig) JsonConvert.DeserializeObject(rawConfig, typeof(AppConfig));
		}

		public override void Dispose()
		{
		}

		public void LogWebhook(string message)
		{
			webhooks.Add(new WebhookLog
			{
				date = DateTime.Now, 
				log = message
			});
		}
	}
}