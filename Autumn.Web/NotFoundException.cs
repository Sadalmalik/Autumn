using System;

namespace Autumn.MVC
{
	public class NotFoundException : Exception
	{
		public NotFoundException(string message = null) : base(message)
		{
		}
	}
}