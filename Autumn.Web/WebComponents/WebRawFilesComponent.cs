using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autumn.IOC;

namespace Autumn.MVC
{
	public class WebRawFilesComponent : WebComponent
	{
		public override Task Handle(HttpListenerContext context, string rout, Match routMatch, Exception exception = null)
		{
			var response = context.Response;
			
			var filePath  = container.fileHierarchy.FindFile(rout, false);
			var extension = Path.GetExtension(rout);
			
			response.ContentType = WebUtils.AddToHeader(
				context.Response.ContentType,
				WebDefines.GetContentTypeByExtension(extension),
				"charset=utf-8");

			byte[] body = File.ReadAllBytes(filePath);
			
			response.ContentLength64 = body.Length;
			
			Stream output = response.OutputStream;
			output.Write(body, 0, body.Length);
			output.Close();
			
			return Task.FromResult<object>(null);
		}
	}
}