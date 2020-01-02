using System;
using System.Collections.Generic;
using System.IO;

// ReSharper disable UnusedMember.Global

namespace Autumn.Utils.Logger
{
	public static class Log
	{
		private static bool            emptyTargets_ = true;
		private static List<LogTarget> targets_;

		public static List<LogTarget> targets
		{
			get
			{
				if (targets_ == null)
					targets_ = new List<LogTarget>();
				return targets_;
			}
		}

		public static bool logTime = true;
		public static string timeFormat = "yyyy.MM.dd HH:mm:ss";
		public static string timeFileFormat = "yyyyMMddHHmmss";
		
		public static string GetDateTime()
		{
			return DateTime.Now.ToString(timeFormat);
		}
		
		public static string GetFileTime()
		{
			return DateTime.Now.ToString(timeFileFormat);
		}
		
		public static LogTarget AddDefaultTarget()
		{
			emptyTargets_ = false;
			var target = new DefaultLogTarget();
			targets.Add(target);
			return target;
		}
		
		public static LogTarget AddDefaultFileTarget()
		{
			emptyTargets_ = false;
			string path = Path.Join(FileUtils.CurrentPath, $"logs.{GetFileTime()}.txt");
			var target = new FileLogTarget(path);
			targets.Add(target);
			return target;
		}

		public static LogTarget AddTarget<T>() where T : LogTarget, new()
		{
			emptyTargets_ = false;
			var target = new T();
			targets.Add(target);
			return target;
		}

		public static LogTarget AddTarget<T>(T target) where T : LogTarget
		{
			emptyTargets_ = false;
			targets.Add(target);
			return target;
		}

		public static void Temp(string message, params object[] args)
		{
			if (emptyTargets_)
				Console.Error.WriteLine("Log targets not defined!");

			string _final = args.Length > 0 ? string.Format(message, args) : message;
			string _message = $"{(logTime?GetDateTime()+" ":"")}[TEMP] {_final}";
			foreach (var target in targets)
				target.LogTemp(_message);
		}

		public static void Info(string message, params object[] args)
		{
			if (emptyTargets_)
				Console.Error.WriteLine("Log targets not defined!");
			
			string _final = args.Length > 0 ? string.Format(message, args) : message;
			string _message = $"{(logTime?GetDateTime()+" ":"")}[INFO] {_final}";
			foreach (var target in targets)
				target.LogInfo(_message);
		}

		public static void Warning(string message, params object[] args)
		{
			if (emptyTargets_)
				Console.Error.WriteLine("Log targets not defined!");
			
			string _final = args.Length > 0 ? string.Format(message, args) : message;
			string _message = $"{(logTime?GetDateTime()+" ":"")}[WARNING] {_final}";
			foreach (var target in targets)
				target.LogWarning(_message);
		}

		public static void Error(string message, params object[] args)
		{
			if (emptyTargets_)
				Console.Error.WriteLine("Log targets not defined!");
			
			string _final = args.Length > 0 ? string.Format(message, args) : message;
			string _message = $"{(logTime?GetDateTime()+" ":"")}[ERROR] {_final}";
			foreach (var target in targets)
				target.LogError(_message);
		}
	}
}