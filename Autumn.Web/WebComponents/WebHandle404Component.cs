using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Scriban;

namespace Autumn.MVC
{
	public class WebHandle404Component : WebComponent
	{
		private Template _template;
		public  string   templateFile { get; private set; }

		public WebHandle404Component(string templateFile = null)
		{
			this.templateFile = templateFile;
		}

		public override WebComponent Setup(Method Method, string Rout, int Priority, WebContainer Container)
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

		public override Task Handle(HttpListenerContext context, string rout, Match routMatch,
		                            Exception           exception = null)
		{
			var model = new DefaultPageModel
			{
				code        = 404,
				header      = "Page not found!",
				description = "Could not find the specified resource!",
				stacktrace  = null
			};
			string body   = _template.Render(model);
			byte[] result = Encoding.UTF8.GetBytes(body);

			SendContent(context, 404, ContentType.HTML, result);

			return Task.FromResult<object>(null);
		}
	}
}