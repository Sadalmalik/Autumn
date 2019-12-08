using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Autumn.Utils
{
	public class DataUtils
	{
		public static readonly char[] symbols = {'0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f'};

		public static string ByteToHex(byte[] buffer)
		{
			if (buffer == null)
				return "";
			var len = buffer.Length;
			var s   = new char[len*2];
			for (int i = 0; i < len; i++)
			{
				var b = buffer[i];
				s[i * 2 + 0] = symbols[b >> 4 & 0xF];
				s[i * 2 + 1] = symbols[b >> 0 & 0xF];
			}
			return new string(s);
		}
        
		public static string BuildQueryData(Dictionary<string, string> param)
		{
			if (param == null)
				return "";
            
			bool          first    = true;
			StringBuilder sbuilder = new StringBuilder();
			foreach (var item in param)
			{
				if (!first)
					sbuilder.Append('&');
				sbuilder.Append(item.Key);
				sbuilder.Append('=');
				sbuilder.Append(WebUtility.UrlEncode(item.Value));
				first = false;
			}

			return sbuilder.ToString();
		}
	}
}