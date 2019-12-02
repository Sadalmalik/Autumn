using System;
using Autumn.IOC;
using Autumn.MVC;

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

		[WebController("^/$", 0, ContentType.HTML, "index.html")]
		public IndexCounter Index()
		{
			Console.WriteLine($"Index calls: {++appManager.indexCalls}");

			return new IndexCounter
			{
				indexCount   = appManager.indexCalls,
				webhookCount = appManager.webhookCalls,
				webhooks     = appManager.webhooks.ToArray()
			};
		}
	}
}