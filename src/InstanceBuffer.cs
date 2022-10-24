using System;
using System.Collections.Generic;

namespace ThirdParty.npg.bindlessdi
{
	internal sealed class InstanceBuffer : IDisposable
	{
		private List<object> _instances = new(32);
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