using System.Net;
using System.Text;
using Autumn.IOC;
using Autumn.MVC;
using Autumn.Utils;
using Autumn.Utils.Logger;
using BitMEX;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

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

		[WebController(Method.ANY, "^/webhook/$", 0, ContentType.Text)]
		public object WebhookTest(
			[WebContent] string content,
			[WebContent(ContentType.Bin)] byte[] rawContent,
			HttpListenerRequest request)
		{
			Log.Temp($"Call /webhook/ calls: {++appManager.webhookCalls}");

			Log.Temp($"Content: {(rawContent==null?"null":rawContent.Length.ToString())} -> \"{content}\" {DataUtils.ByteToHex(rawContent)}");
			
			
			StringBuilder sb = new StringBuilder();
			foreach (var key in request.Headers.AllKeys)
				sb.AppendLine($"{key}: {request.Headers[key]}");

			appManager.LogWebhook($"{request.HttpMethod} {request.RawUrl}\n\nHEADERS:\n{sb}\n\nCONTENT:\n{content}"
				                     .EscapeHTML());

			_bitmex.PostOrders();

			return _bitmex.GetOrders();
		}

		[WebController(Method.ANY, "^/webhook/deleteAll/$", 0, ContentType.Text)]
		public object WebhookReset([WebContent] string content, HttpListenerRequest request)
		{
			Log.Temp($"/webhook/deleteAll/ calls: {++appManager.webhookCalls}");

			_bitmex.DeleteOrders();

			return _bitmex.GetOrders();
		}
	}
}