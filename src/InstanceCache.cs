using System;
using System.Collections.Generic;

namespace ThirdParty.npg.bindlessdi
{
	internal sealed class InstanceCache : IDisposable
	{
		private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

		public bool TryGetInstance(Type resultType, out object instance)
		{
			return _instances.TryGetValue(resultType, out instance);
		}

		public void AddInstance(Type type, object instance)
		{
			_instances[type] = instance;
		}

		public void Dispose()
		{
			_instances?.Clear();
		}
	}
}