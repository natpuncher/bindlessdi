using System;

namespace ThirdParty.npg.bindlessdi
{
	internal class Resolver : IDisposable
	{
		private readonly Instantiator _instantiator = new();
		
		private readonly InstantiationPolicyRegistry _instantiationPolicyRegistry;
		private readonly ConstructionInfoProvider _constructionInfoProvider;
		private readonly InstanceCache _instanceCache;
		private readonly UnityEventsHandler _unityEventsHandler;

		private readonly Pool<InstanceBuffer> _instanceBufferPool = new();

		public Resolver(InstantiationPolicyRegistry instantiationPolicyRegistry, ContractBinder contractBinder, InstanceCache instanceCache)
		{
			_instantiationPolicyRegistry = instantiationPolicyRegistry;
			_instanceCache = instanceCache;
			_constructionInfoProvider = new ConstructionInfoProvider(contractBinder);
			_unityEventsHandler = new UnityEventsHandler();
		}
		
		public void Dispose()
		{
			_constructionInfoProvider?.Dispose();
			_unityEventsHandler?.Dispose();
		}

		public object Resolve(Type type)
		{
			var canBeResolved = _constructionInfoProvider.TryGetInfo(type, out var info);
			if (!canBeResolved)
			{
				return null;
			}
			return GetInstance(info);
		}

		public object Resolve(Type type, InstantiationPolicy instantiationPolicy)
		{
			var canBeResolved = _constructionInfoProvider.TryGetInfo(type, out var info);
			if (!canBeResolved)
			{
				return null;
			}
			return GetInstance(info, instantiationPolicy);
		}

		private object GetInstance(ConstructionInfo info)
		{
			return GetInstance(info, _instantiationPolicyRegistry.GetPolicy(info.TargetType));
		}

		private object GetInstance(ConstructionInfo info, InstantiationPolicy instantiationPolicy)
		{
			object result = null;
			if (instantiationPolicy == InstantiationPolicy.Single && !_instanceCache.TryGetInstance(info.TargetType, out result))
			{
				result = CreateInstance(info);
				_instanceCache.AddInstance(info.TargetType, result);
			}

			if (instantiationPolicy == InstantiationPolicy.Transient)
			{
				result = CreateInstance(info);
			}

			return result;
		}

		private object CreateInstance(ConstructionInfo info)
		{
			var buffer = _instanceBufferPool.Get();
			foreach (var dependencyType in info.Dependencies)
			{
				buffer.Add(GetInstance(dependencyType));
			}
			
			var result = _instantiator.Construct(info, buffer);
			_unityEventsHandler.TryRegisterInstance(result);
			_instanceBufferPool.Return(buffer);
			return result;
		}
	}
}