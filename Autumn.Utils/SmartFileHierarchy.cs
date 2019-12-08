using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Autumn.Utils
{
	public class SmartFileHierarchy
	{
		private List<string>                     _pathRoots;
		private List<string>                     _pathsGeneral;
		private Dictionary<string, List<string>> _pathsExtensionSpecified;

		public int searchHeight = 15;

		public IEnumerable<string> SupportedExtensions => _pathsExtensionSpecified.Keys;

		public SmartFileHierarchy()
		{
			_pathRoots               = new List<string>();
			_pathsGeneral            = new List<string>();
			_pathsExtensionSpecified = new Dictionary<string, List<string>>();
		}

		public void AddDefaultRootPath()
		{
			_pathRoots.Add(FileUtils.CurrentPath);
		}
		
		public void AddRootPaths(params string[] paths)
		{
			_pathRoots.AddRange(paths.Where(x => !string.IsNullOrEmpty(x)).Distinct());
		}

		public void AddGeneralPaths(params string[] paths)
		{
			_pathsGeneral.AddRange(paths.Distinct());
		}

		public void AddExtensionPaths(string ext, params string[] paths)
		{
			List<string> _paths;
			if (!_pathsExtensionSpecified.TryGetValue(ext, out _paths))
				_pathsExtensionSpecified[ext] = _paths = new List<string>();
			_paths.AddRange(paths);
			_pathsExtensionSpecified[ext] = _paths.Distinct().ToList();
		}


		/// <summary>
		///		Итак.
		///		Есть определённые спецификаторы пути и их приоритеты.
		///		Порядок приоритетов примерно такой:
		///			root1/general/extension/file.ext
		///			root2/general/extension/file.ext
		///			root1/general/file.ext
		///			root2/general/file.ext
		///			root1/extension/file.ext
		///			root2/extension/file.ext
		///			root1/file.ext
		///			root2/file.ext
		///			../root1/general/extension/file.ext
		///			../root2/general/extension/file.ext
		///			..
		///		И так до достижения корня каталога или максимальной высоты
		///		Здесь
		///			root - перечисленные по очереди корневые директории, от которых идёт поиск
		///			general - перечисленные по порядку глобальные пути
		///			extension - перечисленные по порядку пути по расширению
		///
		///		Вообще я хотел сделать универсальный конфигурируемый поиск
		///		Потому как у меня уже была схожая задача для поиска файлов для Bombardo
		///		Но я пока не сообразил как ввести дополнительные приоритеты.
		///		Так что пока это черновой вариант.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public string FindFile(string filename, bool byExtension = true, bool debug = false)
		{
			if (_pathRoots.Count == 0)
				_pathRoots.Add(FileUtils.CurrentPath);

			var ext        = Path.GetExtension(filename);
			var extEnabled = _pathsExtensionSpecified.ContainsKey(ext);
			var extensions = extEnabled ? _pathsExtensionSpecified[ext] : null;

			byExtension &= extEnabled;

			foreach (string _root in _pathRoots)
			{
				string root = Path.GetFullPath(_root);
				for (int height = searchHeight; height >= 0; height--)
				{
					string path;
					foreach (var general in _pathsGeneral)
					{
						if (byExtension)
						{
							foreach (var extPath in extensions)
							{
								path = Path.Combine(root, general, extPath, filename);
								if (debug) Console.WriteLine($"Check path {path} = {File.Exists(path)}");
								if (File.Exists(path)) return path;
							}
						}

						path = Path.Combine(root, general, filename);
						if (debug) Console.WriteLine($"Check path {path} = {File.Exists(path)}");
						if (File.Exists(path)) return path;
					}

					if (byExtension)
					{
						foreach (var extPath in extensions)
						{
							path = Path.Combine(root, extPath, filename);
							if (debug) Console.WriteLine($"Check path {path} = {File.Exists(path)}");
							if (File.Exists(path)) return path;
						}
					}

					path = Path.Combine(root, filename);
					if (debug) Console.WriteLine($"Check path {path} = {File.Exists(path)}");
					if (File.Exists(path)) return path;

					var parent = Directory.GetParent(root);
					if (parent == null) break;
					root = parent.FullName;
				}
			}

			return null;
		}
	}
}