using System;
using System.Collections.Generic;

namespace ThirdParty.npg.bindlessdi
{
	internal sealed class InstantiationPolicyRegistry : IDisposable
	{
		internal InstantiationPolicy DefaultPolicy = InstantiationPolicy.Single;
		
		private readonly Dictionary<Type, InstantiationPolicy> _policies = new();

		public void Register(Type type, InstantiationPolicy instantiationPolicy)
		{
			_policies[type] = instantiationPolicy;
		}

		public InstantiationPolicy GetPolicy(Type type)
		{
			if (!_policies.TryGetValue(type, out var instantiationPolicy))
			{
				instantiationPolicy = DefaultPolicy;
			}

			return instantiationPolicy;
		}

		public void Dispose()
		{
			_policies?.Clear();
		}
	}
}