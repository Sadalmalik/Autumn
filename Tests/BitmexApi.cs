//using ServiceStack.Text;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace BitMEX
{
	public class OrderBookItem
	{
		public string   Symbol    { get; set; }
		public int      Level     { get; set; }
		public int      BidSize   { get; set; }
		public decimal  BidPrice  { get; set; }
		public int      AskSize   { get; set; }
		public decimal  AskPrice  { get; set; }
		public DateTime Timestamp { get; set; }
	}
	
	public class BitmexAPI : BitmexConnection
	{
		public BitmexAPI(
			string bitmexKey    = "",
			string bitmexSecret = "",
			int    rateLimit    = 5000)
			: base(bitmexKey, bitmexSecret, rateLimit)
		{
		}
		
		//public List<OrderBookItem> GetOrderBook(string symbol, int depth)
		//{
		//    var param = new Dictionary<string, string>();
		//    param["symbol"] = symbol;
		//    param["depth"] = depth.ToString();
		//    string res = Query("GET", "/orderBook", param);
		//    return JsonSerializer.DeserializeFromString<List<OrderBookItem>>(res);
		//}

		public string GetTradeBuckets()
		{
			var param = new Dictionary<string, string>();
			param["binSize"] = "1m";
			param["symbol"] = "XBTUSD";
			param["startTime"] = GetTime(- 2*60*60);
			param["endTime"] = GetTime();
			return Query("GET", "/trade/bucketed", param, true);
		}
		
		public string CloseAllOrders()
		{
			var param = new Dictionary<string, string>();
			param["symbol"] = "XBTUSD";
			//param["filter"] = "{\"open\":true}";
			//param["columns"] = "";
			//param["count"] = 100.ToString();
			//param["start"] = 0.ToString();
			//param["reverse"] = false.ToString();
			//param["startTime"] = "";
			//param["endTime"] = "";
			return Query("DELETE", "/order/all", param, true);
		}

		public string GetOrders()
		{
			var param = new Dictionary<string, string>();
			param["symbol"] = "XBTUSD";
			//param["filter"] = "{\"open\":true}";
			//param["columns"] = "";
			//param["count"] = 100.ToString();
			//param["start"] = 0.ToString();
			//param["reverse"] = false.ToString();
			//param["startTime"] = "";
			//param["endTime"] = "";
			return Query("GET", "/order", param, true);
		}

		public string PostOrders()
		{
			var param = new Dictionary<string, string>();
			param["symbol"]   = "XBTUSD";
			param["side"]     = "Buy";
			param["orderQty"] = "1";
			param["ordType"]  = "Market";
			return Query("POST", "/order", param, true);
		}

		public string PostSellOrders(int count = 1)
		{
			var param = new Dictionary<string, string>();
			param["symbol"]   = "XBTUSD";
			param["side"]     = "Sell";
			param["orderQty"] = count.ToString();
			param["ordType"]  = "Market";
			return Query("POST", "/order", param, true);
		}

		public string DeleteOrders()
		{
			var param = new Dictionary<string, string>();
			return Query("DELETE", "/order/all", param, true, true);
		}

		public string DeleteOrder(string orderID)
		{
			var param = new Dictionary<string, string>();
			param["symbol"]  = "XBTUSD";
			param["orderID"] = orderID;
			return Query("DELETE", "/order", param, true, true);
		}

		private byte[] hmacsha256(byte[] keyByte, byte[] messageBytes)
		{
			using (var hash = new HMACSHA256(keyByte))
			{
				return hash.ComputeHash(messageBytes);
			}
		}
	}
}