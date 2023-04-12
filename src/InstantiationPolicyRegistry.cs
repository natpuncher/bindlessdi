using System;
using System.Collections.Generic;
using UnityEngine;

namespace npg.bindlessdi
{
	internal sealed class InstantiationPolicyRegistry : IDisposable
	{
		internal InstantiationPolicy DefaultPolicy = InstantiationPolicy.Single;

		private readonly Dictionary<Type, InstantiationPolicy> _contractPolicies = new Dictionary<Type, InstantiationPolicy>();
		private readonly Dictionary<Type, InstantiationPolicy> _basePolicies = new Dictionary<Type, InstantiationPolicy>();

		public void Register(Type type, InstantiationPolicy instantiationPolicy)
		{
			_contractPolicies[type] = instantiationPolicy;
		}

		public InstantiationPolicy GetPolicy(Type type)
		{
			if (_contractPolicies.TryGetValue(type, out var instantiationPolicy))
			{
				return instantiationPolicy;
			}

			if (TryGetBasePolicy(type, out instantiationPolicy))
			{
				return instantiationPolicy;
			}

			return DefaultPolicy;
		}

		public void Dispose()
		{
			DefaultPolicy = InstantiationPolicy.Single;
			_contractPolicies?.Clear();
			_basePolicies?.Clear();
		}

		private bool TryGetBasePolicy(Type type, out InstantiationPolicy instantiationPolicy)
		{
			if (_basePolicies.TryGetValue(type, out instantiationPolicy))
			{
				return true;
			}

			if (TryFindBasePolicy(type, out instantiationPolicy))
			{
				return true;
			}

			return false;
		}

		private bool TryFindBasePolicy(Type type, out InstantiationPolicy instantiationPolicy)
		{
			instantiationPolicy = DefaultPolicy;
			var result = false;
			
			foreach (var contractPolicy in _contractPolicies)
			{
				var contractType = contractPolicy.Key;
				if (!contractType.IsAssignableFrom(type))
				{
					continue;
				}

				if (_basePolicies.TryGetValue(type, out var basePolicy))
				{
					Debug.LogWarning(
						$"[bindlessdi] Ambiguous instantiation policy contract found for {type.FullName} -> {contractType.FullName}, " +
						$"InstantiationPolicy.{basePolicy} will be used!");
					continue;
				}

				instantiationPolicy = contractPolicy.Value;
				_basePolicies[type] = instantiationPolicy;
				result = true;
			}

			return result;
		}
	}
}