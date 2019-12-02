using System;

namespace Autumn.MVC
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public class WebController : Attribute
	{
		public Method      method;
		public string      rout;
		public int         priority;
		public ContentType type;
		public string      view;

		public WebController(
			Method      method,
			string      rout,
			int         priority = 0,
			ContentType type     = ContentType.JSON,
			string      view     = null)
		{
			this.method   = method;
			this.rout     = rout;
			this.priority = priority;
			this.type     = type;
			this.view     = view;
		}

		public WebController(
			string      rout,
			int         priority = 0,
			ContentType type     = ContentType.JSON,
			string      view     = null)
		{
			this.method   = Method.GET;
			this.rout     = rout;
			this.priority = priority;
			this.type     = type;
			this.view     = view;
		}
	}

	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
	public class WebContent : Attribute
	{
		public ContentType type;

		public WebContent(ContentType sourceType=ContentType.Text)
		{
			type = sourceType;
		}
	}
}