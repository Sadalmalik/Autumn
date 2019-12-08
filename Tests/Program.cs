using System;
using System.Collections.Generic;
using BitMEX;
using Newtonsoft.Json;

namespace Tests
{
	public class Order
	{
		public string orderID;
	}
	
	class Program
	{
		public static string BitmexKey = "TvbPcwV0KaWUESULvJhwpCFM";
		public static string BitmexSecret = "jOlgND9wqO3bhbVixKMXnSeyP1g4Z6AL3w7QyRYLau7HFtGi";

		public static BitmexAPI API;
		
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			string result;
			API = new BitmexAPI(BitmexKey, BitmexSecret);

			Console.WriteLine("Get trade buckets:");
			result = API.GetTradeBuckets();
			
			Console.WriteLine();
			Console.WriteLine(result);
			Console.WriteLine();
			
			/*
			
			
			var param = new Dictionary<string, string>();
			param["symbol"] = "XBTUSD";
			
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Get Orders:");
			Console.WriteLine();
			ShowOrders();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Delete Orders:");
			
			Console.WriteLine();
			result = API.Query("DELETE", "/order/all", param, true, true);
			Console.WriteLine($"Response: {result}");
			
			Console.WriteLine();
			result = API.Query("DELETE", "/order/all", param, true, false);
			Console.WriteLine($"Response: {result}");

			Console.WriteLine();
			result = API.CloseAllOrders();
			Console.WriteLine($"Response: {result}");

			//Console.WriteLine($"Post sell order: {API.PostSellOrders(50)}");
			
			Console.WriteLine();
			ShowOrders();
			Console.WriteLine();
			//*
			Console.WriteLine();
			Console.WriteLine("Add one Order:");
			Console.WriteLine();
			API.PostOrders();
			ShowOrders();
			Console.WriteLine();
			Console.WriteLine();
			//*/
		}

		public static void ShowOrders()
		{
			string ordersRaw = API.GetOrders();
			Order[]orders = JsonConvert.DeserializeObject(ordersRaw, typeof(Order[])) as Order[];
			if (orders == null)
			{
				Console.WriteLine("Can't parse json!!!");
				Console.WriteLine();
				return;
			}

			int len = orders.Length;
			int lim = Math.Min(10, len);
			Console.WriteLine($"Orders: {len}");
			for(int i=0;i<lim;i++)
			{
				Console.WriteLine($" - {orders[i].orderID}");
			}
			Console.WriteLine();
		}

		public static void DeleteFirstOrders(int toDelete=3)
		{
			string  ordersRaw = API.GetOrders();
			Order[] orders    = JsonConvert.DeserializeObject(ordersRaw, typeof(Order[])) as Order[];
			if (orders == null)
			{
				Console.WriteLine("Can't parse json!!!");
				Console.WriteLine();
				return;
			}

			int len = orders.Length;
			int lim = Math.Min(len, toDelete);
			Console.WriteLine($"Delete {lim} first");
			for (int i = 0; i < lim; i++)
			{
				var ord = orders[i].orderID;
				Console.WriteLine($"Delete: {ord}");
				var res = API.DeleteOrder(ord);
				Console.WriteLine($"Result: {res}");
				Console.WriteLine();
			}

			Console.WriteLine();
		}
	}
}