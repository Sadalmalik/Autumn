using System;
using System.Net;
using System.Text;
using Autumn.IOC;
using Autumn.MVC;
using Autumn.Utils;
using BitMEX;

namespace TestBot
{
	public class AppWebhookController : SharedObject
	{
#pragma warning disable CS0649
		[Inject] private AppManager appManager;
#pragma warning restore CS0649

		private BitMEXApi _bitmex;

		public override void Init()
		{
			_bitmex = new BitMEXApi(
				appManager.config.BitmexKey,
				appManager.config.BitmexSecret);
		}

		[WebController("^/webhook/$", 0, ContentType.Text)]
		public object WebhookTest([WebContent] string content, HttpListenerRequest request)
		{
			Console.WriteLine($"Webhook calls: {++appManager.webhookCalls}");

			StringBuilder sb = new StringBuilder();
			foreach (var key in request.Headers.AllKeys)
				sb.AppendLine($"{key}: {request.Headers[key]}");
			
			appManager.LogWebhook($"URL: {request.RawUrl}\n\nHEADERS:\n{sb}\n\nCONTENT:\n{content}".EscapeHTML());

			_bitmex.PostOrders();

			return _bitmex.GetOrders();
		}
		
		[WebController("^/webhook/deleteAll$", 0, ContentType.Text)]
		public object WebhookReset([WebContent] string content, HttpListenerRequest request)
		{
			Console.WriteLine($"Webhook calls: {++appManager.webhookCalls}");

			_bitmex.DeleteOrders();

			return _bitmex.GetOrders();
		}
	}
}