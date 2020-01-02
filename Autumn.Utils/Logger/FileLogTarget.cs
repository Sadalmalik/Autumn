using System.IO;
using System.Threading.Tasks;

namespace Autumn.Utils.Logger
{
	public class FileLogTarget : LogTarget
	{
		private FileStream _fileStream;
		private TextWriter _writer;
		public string Path { get; private set; }

		public FileLogTarget(string path)
		{
			Path = path;
			_fileStream = new FileStream(
				path,
				FileMode.OpenOrCreate,
				FileAccess.ReadWrite,
				FileShare.None);
			_writer = new StreamWriter(_fileStream);
		}

		public Task<string> GetAllLogs()
		{
			var reader = new StreamReader(_fileStream);
			reader.BaseStream.Seek(0, SeekOrigin.Begin);
			return reader.ReadToEndAsync();
		}

		public override void LogTemp(string message)
		{
			_writer.WriteLine(message);
			_writer.Flush();
		}

		public override void LogInfo(string message)
		{
			_writer.WriteLine(message);
			_writer.Flush();
		}

		public override void LogWarning(string message)
		{
			_writer.WriteLine(message);
			_writer.Flush();
		}

		public override void LogError(string message)
		{
			_writer.WriteLine(message);
			_writer.Flush();
		}
	}
}