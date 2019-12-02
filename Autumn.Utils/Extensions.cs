namespace Autumn.Utils
{
	public static class Extensions
	{
		public static string EscapeHTML(this string text)
		{
			return text
			      .Replace("<", "&lt;")
			      .Replace(">", "&gt;");
		}
	}
}