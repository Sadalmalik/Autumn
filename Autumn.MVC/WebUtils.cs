using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Autumn.MVC
{
	public static class WebUtils
	{
		public static string AddToHeader(string header, params string[] values)
		{
			var newValues =
				(header == null
					? values
					: header.Split(";")
					        .Union(values))
			   .Select(x => x.Trim())
			   .Distinct();
			return string.Join("; ", newValues);
		}

		public static string RemoveFromHeader(string header, params string[] values)
		{
			if (header == null)
				return header;
			var newValues = header
			               .Split(";")
			               .Select(x => x.Trim())
			               .Where(x => !values.Contains(x))
			               .Distinct();
			return string.Join("; ", newValues);
		}

		public static Method ParseMethod(string meshod)
		{
			meshod = meshod.ToUpper();
			switch (meshod)
			{
				case "GET":
					return Method.GET;
				case "POST":
					return Method.POST;
				case "PUT":
					return Method.PUT;
				case "UPDATE":
					return Method.UPDATE;
				case "DELETE":
					return Method.DELETE;
				default:
					return Method.UNKNOWN;
			}
		}
	}
}