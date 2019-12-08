using System;
using System.Text;
using Autumn.IOC;
using Autumn.MVC;
using Autumn.Utils;
using Autumn.Utils.Logger;

// ReSharper disable UnusedMember.Global

namespace TestBot
{
	public class IndexCounter
	{
		public int indexCount;
		public int webhookCount;

		public WebhookLog[] webhooks;
	}

	public class AppFaceController : SharedObject
	{
#pragma warning disable CS0649
		[Inject] private AppManager appManager;
#pragma warning restore CS0649

		public event Action OnExit;

		[WebController("^/$", 0, ContentType.HTML, "index.html")]
		public IndexCounter Index()
		{
			Log.Temp($"Index calls: {++appManager.indexCalls}");

			return new IndexCounter
			{
				indexCount   = appManager.indexCalls,
				webhookCount = appManager.webhookCalls,
				webhooks     = appManager.webhooks.ToArray()
			};
		}

		[WebController("^/server/control/shutdown/$")]
		public object Shutdown(string signature)
		{
			Log.Temp($"Receive signal to /server/control/shutdown/ with signature {signature}");
			var expectedSignature = HashManager.Hmacsha256(
				appManager.config.ServerSecret,
				"/server/control/shutdown/");

			if (expectedSignature.Equals(signature))
			{
				Log.Temp("Good signature!");
				OnExit?.Invoke();
				return new {result = "success"};
			}

			Log.Temp($"Wrong signature! Expected: {expectedSignature}");
			return new {result = "fail", message = "Wrong signature!"};
		}
	}
}