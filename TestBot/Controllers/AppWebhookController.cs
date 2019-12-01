using System;
using Autumn.IOC;
using Autumn.MVC;
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
		public object WebHookHandler()
		{
			Console.WriteLine($"Webhook calls: {++appManager.webhookCalls}");

			_bitmex.PostOrders();

			return _bitmex.GetOrders();
		}
	}
}