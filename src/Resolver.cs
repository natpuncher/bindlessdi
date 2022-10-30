using System;

namespace ThirdParty.npg.bindlessdi
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
			bool handleUnityEvents)
		{
			_instantiationPolicyRegistry = instantiationPolicyRegistry;
			_contractBinder = contractBinder;
			_instanceCache = instanceCache;
			_constructionInfoProvider = new ConstructionInfoProvider(_instanceCache, _contractBinder);
			if (handleUnityEvents)
			{
				_unityEventsHandler = new UnityEventsHandler();
			}
		}

		public void Dispose()
		{
			_constructionInfoProvider?.Dispose();
			_unityEventsHandler?.Dispose();
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
						result = CreateInstance(targetType);
						_instanceCache.AddInstance(targetType, result);
					}
					break;
				}
				case InstantiationPolicy.Transient:
					result = CreateInstance(targetType);
					break;
			}

			return result;
		}

		private object CreateInstance(Type type)
		{
			var canBeResolved = _constructionInfoProvider.TryGetInfo(type, out var info);
			if (!canBeResolved)
			{
				return null;
			}
			
			var buffer = _instanceBufferPool.Get();
			foreach (var dependencyInfo in info.Dependencies)
			{
				buffer.Add(Resolve(dependencyInfo.TargetType));
			}

			var result = _instantiator.Construct(info, buffer);
			_unityEventsHandler?.TryRegisterInstance(result);
			_instanceBufferPool.Return(buffer);
			return result;
		}
	}
}