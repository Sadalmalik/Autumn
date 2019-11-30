using System.Threading.Tasks;
using Autumn.IOC;
using Autumn.MVC;

namespace Experiments
{
	public class TestController : SharedObject
	{
		[WebController( "/main/", 0, ContentType.HTML, "Template01.html")]
		private TestPageData Index([WebContent(ContentType.Text)] string content)
		{
			return new TestPageData
			{
				title   = "My Title #01",
				header  = "My Header #01",
				content = $"{content?.Length} : {content}"
			};
		}
        
		[WebController( "/async/", 0, ContentType.HTML, "Template01.html")]
		private async Task<TestPageData> IndexAsync([WebContent(ContentType.Text)] string content)
		{
			await Task.Delay(500);
			return new TestPageData
			{
				title   = "My Title #01",
				header  = "My Header #01",
				content = $"{content?.Length} : {content}"
			};
		}
        
		//    http://localhost:8080/arguments/?index=4&meta=frau&percent=0.25
		[WebController( "/arguments/", 0, ContentType.HTML, "Template01.html")]
		private async Task<TestPageData> IndexAsyncB([WebContent(ContentType.Text)] string content, int index, string meta, double percent)
		{
			await Task.Delay(500);
			return new TestPageData
			{
				title   = "My Title #01",
				header  = "My Header #01",
				content = $"index = {index}, meta = {meta}, percent = {percent}"
			};
		}
	}
}