using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autumn.IOC;

namespace Autumn.MVC
{
	public class McvRawFilesComponent : McvComponent
	{
		public override Task Handle(HttpListenerContext context, string rout, Match routMatch,
		                            Exception           exception = null)
		{
			var response = context.Response;

			if (rout.StartsWith("/"))
				rout = rout.Substring(1, rout.Length - 1);

			var filePath  = container.fileHierarchy.FindFile(rout);
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