using System;
using System.Collections.Generic;

namespace npg.bindlessdi
{
	internal sealed class ContractBinder : IDisposable
	{
		private readonly Dictionary<Type, Type> _bindings = new Dictionary<Type, Type>();

		public void Bind<TContract, TTarget>()
		{
			_bindings[typeof(TContract)] = typeof(TTarget);
		}

		public bool TryGetTargetType(Type bindType, out Type resultType)
		{
			return _bindings.TryGetValue(bindType, out resultType);
		}

		public void Dispose()
		{
			_bindings?.Clear();
		}
	}
}