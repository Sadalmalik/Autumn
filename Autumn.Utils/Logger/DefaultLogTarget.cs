using System;

namespace Autumn.Utils.Logger
{
	public class DefaultLogTarget : LogTarget
	{
		public override void LogTemp(string message)
		{
			Console.Out.WriteLine(message);
		}

		public override void LogInfo(string message)
		{
			Console.Out.WriteLine(message);
		}

		public override void LogWarning(string message)
		{
			Console.Out.WriteLine(message);
		}

		public override void LogError(string message)
		{
			Console.Error.WriteLine(message);
		}
	}
}