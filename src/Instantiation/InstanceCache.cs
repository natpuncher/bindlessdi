using System;
using System.Collections.Generic;

namespace npg.bindlessdi.Instantiation
{
	internal sealed class InstanceCache : IDisposable
	{
		private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
		private readonly List<IDisposable> _disposables = new List<IDisposable>();

		public bool TryGetInstance(Type resultType, out object instance)
		{
			return _instances.TryGetValue(resultType, out instance);
		}

		public void AddInstance(Type type, object instance)
		{
			_instances[type] = instance;
			
			if (instance is IDisposable disposable)
			{
				_disposables.Add(disposable);
			}
		}

		public void Dispose()
		{
			foreach (var disposable in _disposables)
			{
				disposable?.Dispose();
			}
			
			_disposables.Clear();
			_instances.Clear();
		}
	}
}