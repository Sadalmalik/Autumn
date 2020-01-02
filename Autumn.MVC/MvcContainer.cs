using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Autumn.IOC;
using Autumn.Utils;
using Autumn.Utils.Logger;

namespace Autumn.MVC
{
	public class MvcContainer
	{
		public static readonly SmartFileHierarchy DefaultWebHierarchy = FileUtils.BuildWebHierarchy();

		private HttpListener            _listener;
		private Container               _innerContainer;
		private bool                    _work;
		private bool                    _isInner;
		private CancellationTokenSource _cancel;
		private List<object>            _sharedControllers;
		private List<McvComponent>      _components;

		public McvComponent Handler404Page;
		public McvComponent Handler500Page;

		public event Action<Method, string, HttpListenerContext> BeforeController;
		
		public SmartFileHierarchy fileHierarchy { get; private set; }

		public MvcContainer(Container          parentContainer      = null,
		                    bool               createInnerContainer = true,
		                    SmartFileHierarchy smartFileHierarchy   = null)
		{
			_work              = false;
			_listener          = new HttpListener();
			_sharedControllers = new List<object>();
			_components        = new List<McvComponent>();
			_isInner           = createInnerContainer;

			if (createInnerContainer)
			{
				_innerContainer = new Container(parentContainer);
			}
			else
			{
				_innerContainer = parentContainer;
				if (_innerContainer == null)
					throw new ArgumentException(
						"Wrong initial state: 'parentContainer' must be not null, or 'createInnerContainer' must be true!");
			}

			SetFileHierarchy(smartFileHierarchy);
		}

		public void SetFileHierarchy(SmartFileHierarchy smartFileHierarchy)
		{
			fileHierarchy = smartFileHierarchy ?? DefaultWebHierarchy;
		}

		public void AddPrefixes(params string[] prefixes)
		{
			foreach (var prefix in prefixes)
				_listener.Prefixes.Add(prefix);
		}

		public T Add<T>() where T : IShared, new()
		{
			var comp = _innerContainer.Add<T>();
			_sharedControllers.Add(comp);
			return comp;
		}

		public T Add<T>(T component) where T : IShared
		{
			var comp = _innerContainer.Add(component);
			_sharedControllers.Add(comp);
			return component;
		}

		public void AddDefaultFilesComponent()
		{
			_components.Add(
				new McvRawFilesComponent()
				   .Setup(Method.GET, "", int.MaxValue, this));
		}


		private void BuildComponents()
		{
			foreach (var shared in _sharedControllers)
			{
				var type    = shared.GetType();
				var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				foreach (var method in methods)
				{
					var controller = Attribute.GetCustomAttribute(method, typeof(WebController)) as WebController;
					if (controller == null) continue;

					var component = new McvControllerInvokeComponent();
					component.Setup(controller.method, controller.rout, controller.priority, this);
					component.SetConfiguration(controller);
					component.SetInvocationTarget(method, shared);
					_components.Add(component);
				}
			}

			if (_components.Count == 0)
				AddDefaultFilesComponent();

			if (Handler404Page == null)
				Handler404Page = new McvHandle404Component();

			if (Handler500Page == null)
				Handler500Page = new McvHandle500Component();

			Handler404Page.Setup(Method.ANY, ".*", 0, this);
			Handler500Page.Setup(Method.ANY, ".*", 0, this);
		}

		public async void Start()
		{
			_work   = true;
			_cancel = new CancellationTokenSource();

			await Task.Run(BuildComponents);

			if (_isInner)
				_innerContainer.Init();

			if (_listener.Prefixes.Count == 0)
				_listener.Prefixes.Add("http://localhost:8080/");

			_components.Sort((a, b) => a.priority.CompareTo(b.priority));

			_listener.Start();

			while (_work)
				HandleConnection(await _listener.GetContextAsync());
			
			_listener.Close();
			
			if (_isInner)
				_innerContainer.Dispose();
		}

		public void Stop()
		{
			_work = false;
			_cancel.Cancel();
		}

		private async void HandleConnection(HttpListenerContext context)
		{
			Method meshod = WebUtils.ParseMethod(context.Request.HttpMethod);

			string rout = context.Request.RawUrl.Split('?', 2)[0];

			BeforeController(meshod, rout, context);
			
			try
			{
				foreach (var comp in _components)
				{
					if (comp.method!=Method.ANY)
						if (meshod != comp.method)
							continue;

					Match match = comp.route.Match(rout);
					if (!match.Success) continue;

					await comp.Handle(context, rout, match);
					return;
				}
			}
			catch (NotFoundException notFoundException)
			{
				await Handler404Page.Handle(context, rout, null, notFoundException);
			}
			catch (Exception exception)
			{
				await Handler500Page.Handle(context, rout, null, exception);
			}
		}
	}
}