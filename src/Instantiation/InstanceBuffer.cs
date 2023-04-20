using System;
using System.Collections.Generic;

namespace npg.bindlessdi.Instantiation
{
	internal sealed class InstanceBuffer : IDisposable
	{
		private List<object> _instances = new List<object>(32);
		public List<object> Instances => _instances;
		
		public void Add(object instance)
		{
			_instances.Add(instance);
		}

		public void Dispose()
		{
			_instances.Clear();
		}
	}
}