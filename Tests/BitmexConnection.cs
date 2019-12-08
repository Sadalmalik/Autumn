using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Autumn.Utils;
using Newtonsoft.Json;

namespace BitMEX
{
	public class BitmexConnection
	{
		public const string testDomain = "https://testnet.bitmex.com";

		public string domain = testDomain;
		public string apiKey;
		public string apiSecret;
		public int    rateLimit;
        
		public BitmexConnection(string bitmexKey = "", string bitmexSecret = "", int rateLimitTime = 5000)
		{
			apiKey    = bitmexKey;
			apiSecret = bitmexSecret;
			rateLimit = rateLimitTime;
		}
        
		public string GetTime()
		{
			return DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm");
		}
		
		public string GetTime(int offsetSeconds)
		{
			return DateTimeOffset.UtcNow.AddSeconds(offsetSeconds).ToString("yyyy-MM-dd HH:mm");
		}
		public long GetExpires()
		{
			return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600; // set expires one hour in the future
		}
        
		public string Query(string method, string function, Dictionary<string, string> param = null, bool auth = false, bool json = false)
		{
			string paramData = json ? JsonConvert.SerializeObject(param) : DataUtils.BuildQueryData(param);
			string url       = "/api/v1" + function + (method == "GET" && paramData != "" ? "?" + paramData : "");
			string postData  = method != "GET" ? paramData : "";

			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(domain + url);
			webRequest.Method = method;

			if (auth)
			{
				string expires         = GetExpires().ToString();
				string message         = method + url + expires + postData;
				byte[] signatureBytes  = HashManager.Hmacsha256(Encoding.UTF8.GetBytes(apiSecret), Encoding.UTF8.GetBytes(message));
				string signatureString = DataUtils.ByteToHex(signatureBytes);
                
				webRequest.Headers.Add("api-expires", expires);
				webRequest.Headers.Add("api-key", apiKey);
				webRequest.Headers.Add("api-signature", signatureString);
			}

			try
			{
				if (postData != "")
				{
					webRequest.ContentType = json ? "application/json" : "application/x-www-form-urlencoded";
					var data = Encoding.UTF8.GetBytes(postData);
					using (var stream = webRequest.GetRequestStream())
					{
						stream.Write(data, 0, data.Length);
					}
				}

				using (WebResponse webResponse = webRequest.GetResponse())
				using (Stream str = webResponse.GetResponseStream())
				using (StreamReader sr = new StreamReader(str))
				{
					return sr.ReadToEnd();
				}
			}
			catch (WebException wex)
			{
				using (HttpWebResponse response = (HttpWebResponse)wex.Response)
				{
					if (response == null)
						throw;

					using (Stream str = response.GetResponseStream())
					{
						using (StreamReader sr = new StreamReader(str))
						{
							return sr.ReadToEnd();
						}
					}
				}
			}
		}

#region RateLimiter

		private long   lastTicks = 0;
		private object thisLock  = new object();

		private void RateLimit()
		{
			lock (thisLock)
			{
				long elapsedTicks = DateTime.Now.Ticks - lastTicks;
				var  timespan     = new TimeSpan(elapsedTicks);
				if (timespan.TotalMilliseconds < rateLimit)
					Thread.Sleep(rateLimit - (int) timespan.TotalMilliseconds);
				lastTicks = DateTime.Now.Ticks;
			}
		}

#endregion RateLimiter
	}
}