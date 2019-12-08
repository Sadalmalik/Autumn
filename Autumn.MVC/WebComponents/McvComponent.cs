using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Autumn.MVC
{
	public class McvComponent
	{
		public Method       method;
		public Regex        route;
		public int          priority;
		public MvcContainer container;

		public string[] routNames;

		public virtual McvComponent Setup(Method Method, string Rout, int Priority, MvcContainer Container)
		{
			method    = Method;
			route     = new Regex(Rout);
			priority  = Priority;
			container = Container;
			routNames = route.GetGroupNames();
			return this;
		}

		public virtual Task Handle(
			HttpListenerContext context,
			string              rout,
			Match               routMatch,
			Exception           exception = null,
			Method              method    = Method.UNKNOWN)
		{
			throw new NotImplementedException();
		}

		public static void SendContent(HttpListenerContext context, int status, ContentType type, byte[] buffer)
		{
			var response = context.Response;

			response.StatusCode = status;
			response.ContentType = WebUtils.AddToHeader(
				context.Response.ContentType,
				WebDefines.GetContentTypeByEnum(type),
				"charset=utf-8");

			if (buffer == null)
			{
				response.ContentLength64 = 0;
				response.OutputStream.Close();
			}
			else
			{
				response.ContentLength64 = buffer.Length;

				Stream output = response.OutputStream;
				output.Write(buffer, 0, buffer.Length);
				output.Close();
			}
		}
	}
}