using System.Collections.Generic;

namespace Autumn.MVC
{
	public enum ContentType
	{
		Bin,
		Text,
		HTML,
		JSON
	}

	public enum Method
	{
		UNKNOWN,
		GET,
		POST,
		PUT,
		UPDATE,
		DELETE,
		ANY
	}

	public static class WebDefines
	{
		public static readonly string defaultCantentType = "text/plain";

		public static readonly string defaultPageTemplate = @"<!DOCTYPE html>
<html lang=""en"">
<head>
	<meta charset =""UTF-8"">
	<title>{{code}}{{header}}</title>
	<style>
	*	{
		overflow-x: auto;
		white-space: pre-wrap;
		white-space: -moz-pre-wrap;
		white-space: -pre-wrap;
		white-space: -o-pre-wrap;
		word-wrap: break-word;
		margin: 0px;
		}
	body{
		padding: 0px 80px;
		}
	</style>
</head>
<body>
	<h1>{{code}}</h1><h2>{{header}}</h2>
	<p>{{description}}</p>
	<plaintext>{{stacktrace}}</plaintext>
</body>
</html>";

		public static readonly Dictionary<string, string> contentTypeByExtension = new Dictionary<string, string>
		{
			{".html", "text/html"},
			{".js", "text/javascript"},
			{".json", "text/json"},
			{".txt", "text/plain"},
			{".css", "text/css"},
			{".csv", "text/csv"},
			{".xml", "text/xml"},

			{".gif", "image/gif"},
			{".jpg", "image/jpeg"},
			{".jpeg", "image/jpeg"},
			{".png", "image/png"},
			{".tiff", "image/tiff"}
		};

		public static readonly Dictionary<ContentType, string> contentTypeByEnum = new Dictionary<ContentType, string>
		{
			{ContentType.Text, "text/plain"},
			{ContentType.HTML, "text/html"},
			{ContentType.JSON, "text/json"}
		};

		public static string GetContentTypeByExtension(string extension)
		{
			return contentTypeByExtension.TryGetValue(extension, out var result) ? result : defaultCantentType;
		}

		public static string GetContentTypeByEnum(ContentType type)
		{
			return contentTypeByEnum.TryGetValue(type, out var result) ? result : defaultCantentType;
		}
	}
}