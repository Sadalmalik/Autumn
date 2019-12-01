using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scriban;

namespace Autumn.MVC
{
	public class McvControllerInvokeComponent : McvComponent
	{
		private readonly JsonSerializerSettings _jsonSettings;
		private          object                 _invocationTarget;
		private          MethodInfo             _invocationMethod;
		private          Template               _template;
		private          ParameterInfo[]        _parameters;
		private          ContentType            _type;
		private          string                 _view;
		private          bool                   _isAsync;

		public McvControllerInvokeComponent()
		{
			_template     = null;
			_jsonSettings = new JsonSerializerSettings();
		}

		public void SetConfiguration(WebController attribute)
		{
			_type = attribute.type;
			_view = attribute.view;

			if (_view == null)
			{
				if (_type == ContentType.HTML) throw new ArgumentException("For HTML type must be defined view name!");
			}
			else
			{
				var filepath    = container.fileHierarchy.FindFile(_view);
				var rawTemplate = File.ReadAllText(filepath);
				_template = Template.Parse(rawTemplate);
			}
		}

		public void SetInvokationTarget(MethodInfo method, object target)
		{
			_invocationMethod = method;
			_invocationTarget = target;

			_parameters = method.GetParameters();

			_isAsync = IsAsyncMethod(method);
		}

		private object ResolvaParameter(
			ParameterInfo              parametr,
			Dictionary<string, string> arguments,
			HttpListenerContext        context,
			byte[]                     rawContent)
		{
			var type = parametr.ParameterType;

			if (type == typeof(HttpListenerContext)) return context;
			if (type == typeof(HttpListenerRequest)) return context.Request;
			if (type == typeof(HttpListenerResponse)) return context.Response;

			var content = Attribute.GetCustomAttribute(type, typeof(WebContent)) as WebContent;
			if (content != null)
			{
				if (content.type == ContentType.Bin)
				{
					if (type == typeof(byte))
						return rawContent;
				}

				string finalContent = Encoding.UTF8.GetString(rawContent);
				if (content.type == ContentType.JSON)
					return JsonConvert.DeserializeObject(finalContent, type.GetType());
				if (type == typeof(string))
					return finalContent;
				throw new ArgumentException($"Can't transform input content to '{type.Name}' type!");
			}

			if (arguments.TryGetValue(parametr.Name, out var value))
			{
				if (type == typeof(bool)) return bool.Parse(value);
				if (type == typeof(char)) return char.Parse(value);
				if (type == typeof(byte)) return byte.Parse(value);
				if (type == typeof(sbyte)) return sbyte.Parse(value);
				if (type == typeof(short)) return short.Parse(value);
				if (type == typeof(ushort)) return ushort.Parse(value);
				if (type == typeof(int)) return int.Parse(value);
				if (type == typeof(uint)) return uint.Parse(value);
				if (type == typeof(long)) return long.Parse(value);
				if (type == typeof(ulong)) return ulong.Parse(value);
				if (type == typeof(float)) return float.Parse(value.Replace('.', ','));
				if (type == typeof(double)) return double.Parse(value.Replace('.', ','));
				if (type == typeof(string)) return value;
				if (type.IsEnum) return Enum.Parse(type, value);
			}

			if (type.IsValueType)
				return Activator.CreateInstance(type);
			return null;
		}

		public override async Task Handle(
			HttpListenerContext context,
			string              rout,
			Match               routMatch,
			Exception           exception = null)
		{
			var request = context.Request;

			byte[] content = new byte[request.ContentLength64];
			request.InputStream.Read(content, 0, content.Length);

			var arguments = BuildArgumentsFrom(context, routMatch);

			var invocationParameters =
				_parameters
				   .Select(par => ResolvaParameter(par, arguments, context, content))
				   .ToArray();

			object rawContent = null;
			if (_isAsync)
			{
				//	_invocationMethod.ReturnType.GenericTypeArguments[0]
				var task = (Task) _invocationMethod.Invoke(_invocationTarget, invocationParameters);
				await task;
				rawContent = task.GetType().GetProperty("Result").GetValue(task);
			}
			else
			{
				rawContent = _invocationMethod.Invoke(_invocationTarget, invocationParameters);
			}

			if (rawContent == null)
			{
				Console.Write("Aaa");
			}

			var buffer = BuildBody(rawContent);

			SendContent(context, 200, _type, buffer);
		}

		private static bool IsAsyncMethod(MethodInfo method)
		{
			Type attType = typeof(AsyncStateMachineAttribute);
			var  attrib  = (AsyncStateMachineAttribute) method.GetCustomAttribute(attType);
			return attrib != null;
		}


		private Dictionary<string, string> BuildArgumentsFrom(HttpListenerContext context, Match routMatch)
		{
			var request = context.Request;

			var groups    = routMatch.Groups;
			var arguments = new Dictionary<string, string>();
			foreach (var name in routNames)
				arguments[name] = groups[name].Value;
			foreach (var key in request.QueryString.AllKeys)
				arguments[key] = request.QueryString[key];

			return arguments;
		}

		private byte[] BuildBody(object rawContent)
		{
			var type = rawContent.GetType();
			switch (_type)
			{
				case ContentType.Bin:
				{
					CheckType(type, typeof(byte[]));
					return (byte[]) rawContent;
				}

				case ContentType.Text:
				{
					CheckType(type, typeof(string));
					string body = rawContent as string;
					return Encoding.UTF8.GetBytes(body);
				}

				case ContentType.HTML:
				{
					string body = _template.Render(rawContent, member => member.Name);
					return Encoding.UTF8.GetBytes(body);
				}

				case ContentType.JSON:
				{
					string body = JsonConvert.SerializeObject(rawContent, type, _jsonSettings);
					return Encoding.UTF8.GetBytes(body);
				}
			}

			return null;
		}

		private void CheckType(Type type, Type required)
		{
			if (type != required)
				throw new ArgumentException($"Content type mismatch: Expected {required.Name}, but get {type.Name}!");
		}
	}
}