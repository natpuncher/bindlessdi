using System;
using System.Collections.Generic;

namespace ThirdParty.npg.bindlessdi
{
	internal class ContractBinder : IDisposable
	{
		private readonly Dictionary<Type, Type> _bindings = new();

		public void Bind<TInterface, TType>()
		{
			var bindType = typeof(TInterface);
			var resultType = typeof(TType);

			_bindings[bindType] = resultType;
		}

		public bool TryGetImplementation(Type bindType, out Type resultType)
		{
			return _bindings.TryGetValue(bindType, out resultType);
		}

		public void Dispose()
		{
			_bindings?.Clear();
		}
	}
}