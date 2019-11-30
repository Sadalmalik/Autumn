namespace Experiments
{
	/*
	public class WebHandler
	{
		public Regex route;

		public UserData userData;
		public Template template;

		public WebHandler()
		{
			userData = new UserData();
			var rand = new Random();
			var len  = rand.Next(3, 12);
			userData.name     = "Just some name";
			userData.messages = new MessageData[len];
			for (int i = 0; i < len; i++)
			{
				userData.messages[i] = new MessageData
				{
					title = "Message#" + i,
					text  = "Mesasge body " + i
				};
			}

			template = Template.Parse(
				@"<!doctype html>
<html>
<body>
    <h2>{{user.name}}</h2>
    <div>
{{for message in user.messages}}
        <p><h3>{{message.title}}</h3><br/>{{message.text}}</p>
{{end}}
    </div>
</body>
</html>");
		}

		public void Handle(HttpListenerContext context, Match routMatch)
		{
			var request  = context.Request;
			var response = context.Response;

			Console.WriteLine($"Handle request {request.Url} | {request.RawUrl}");
			Console.WriteLine("Arguments:");
			foreach (var key in request.QueryString.AllKeys)
				Console.WriteLine($"    {key} = {request.QueryString[key]}");
			Console.WriteLine("Matches:");
			foreach (var group in routMatch.Groups)
				Console.WriteLine($"    {group}");
			foreach (var name in route.GetGroupNames())
				Console.WriteLine($"    {name} = {routMatch.Groups[name]}");
			Console.WriteLine("Done");

			//request.QueryString

			string responseString = template.Render(Hash.FromAnonymousObject(userData));

			Console.WriteLine($"Result html:\n{responseString}\n");
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

			response.ContentLength64 = buffer.Length;
			response.ContentType     = "text/html; charset=utf-8";

			System.IO.Stream output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			output.Close();
		}
	}
	//*/
}