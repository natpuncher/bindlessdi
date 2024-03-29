using System;
using npg.bindlessdi.Instantiation;
using npg.bindlessdi.UnityLayer;
using npg.bindlessdi.Utils;

namespace npg.bindlessdi.Contracts
{
	internal sealed class Resolver : IDisposable
	{
		private readonly InstantiationPolicyRegistry _instantiationPolicyRegistry;
		private readonly ContractBinder _contractBinder;
		private readonly ConstructionInfoProvider _constructionInfoProvider;
		private readonly InstanceCache _instanceCache;
		private readonly UnityEventsHandler _unityEventsHandler;
		private readonly Instantiator _instantiator = new Instantiator();
		private readonly Pool<InstanceBuffer> _instanceBufferPool = new Pool<InstanceBuffer>();

		public Resolver(InstantiationPolicyRegistry instantiationPolicyRegistry, ContractBinder contractBinder, InstanceCache instanceCache,
			UnityEventsHandler unityEventsHandler, ImplementationGuesser implementationGuesser)
		{
			_instantiationPolicyRegistry = instantiationPolicyRegistry;
			_contractBinder = contractBinder;
			_instanceCache = instanceCache;
			_constructionInfoProvider = new ConstructionInfoProvider(_instanceCache, _contractBinder, implementationGuesser);
			_unityEventsHandler = unityEventsHandler;
		}

		public void Dispose()
		{
			_constructionInfoProvider?.Dispose();
			_instantiator?.Dispose();
		}

		public object Resolve(Type type)
		{
			if (!_contractBinder.TryGetTargetType(type, out var targetType))
			{
				targetType = type;
			}

			return GetInstance(targetType, _instantiationPolicyRegistry.GetPolicy(targetType));
		}

		public object Resolve(Type type, InstantiationPolicy instantiationPolicy)
		{
			if (!_contractBinder.TryGetTargetType(type, out var targetType))
			{
				targetType = type;
			}
			
			return GetInstance(targetType, instantiationPolicy);
		}

		private object GetInstance(Type targetType, InstantiationPolicy instantiationPolicy)
		{
			object result = null;
			switch (instantiationPolicy)
			{
				case InstantiationPolicy.Single:
				{
					if (!_instanceCache.TryGetInstance(targetType, out result))
					{
						result = CreateInstance(targetType, instantiationPolicy);
						_instanceCache.AddInstance(targetType, result);
					}
					break;
				}
				case InstantiationPolicy.Transient:
					result = CreateInstance(targetType, instantiationPolicy);
					break;
			}

			return result;
		}

		private object CreateInstance(Type type, InstantiationPolicy instantiationPolicy)
		{
			var canBeResolved = _constructionInfoProvider.TryGetInfo(type, out var info);
			if (!canBeResolved)
			{
				return null;
			}

			if (instantiationPolicy == InstantiationPolicy.Single &&
			    _instanceCache.TryGetInstance(info.TargetType, out var result))
			{
				return result;
			}
			
			var buffer = _instanceBufferPool.Get();
			foreach (var dependencyType in info.Dependencies)
			{
				buffer.Add(Resolve(dependencyType));
			}

			result = _instantiator.Construct(info, buffer);
			_unityEventsHandler?.TryRegisterInstance(result);
			_instanceBufferPool.Return(buffer);
			return result;
		}
	}
}