using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autumn.Utils;
using Scriban;

namespace Autumn.MVC
{
	public class McvHandle500Component : McvComponent
	{
		private Template _template;
		public  string   templateFile { get; private set; }

		public McvHandle500Component(string templateFile = null)
		{
			this.templateFile = templateFile;
		}

		public override McvComponent Setup(Method Method, string Rout, int Priority, MvcContainer Container)
		{
			base.Setup(Method, Rout, Priority, Container);

			if (templateFile == null)
			{
				_template = Template.Parse(WebDefines.defaultPageTemplate);
			}
			else
			{
				var filepath    = container.fileHierarchy.FindFile(templateFile);
				var rawTemplate = File.ReadAllText(filepath);
				_template = Template.Parse(rawTemplate);
			}

			return this;
		}

		public override Task Handle(
			HttpListenerContext context,
			string              rout,
			Match               routMatch,
			Exception           exception = null,
			Method              method    = Method.UNKNOWN)
		{
			var model = new DefaultPageModel
			{
				code        = 500,
				header      = "Internal server error!",
				description = "An error occurred while the server was running!",
				stacktrace  = exception?.ToString().EscapeHTML()
			};
			string body   = _template.Render(model);
			byte[] result = Encoding.UTF8.GetBytes(body);

			SendContent(context, 500, ContentType.HTML, result);

			return Task.FromResult<object>(null);
		}
	}
}