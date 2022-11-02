using System;
using System.Collections.Generic;

namespace npg.bindlessdi
{
	public class Container : IDisposable
	{
		private readonly Resolver _resolver;
		private readonly ContractBinder _contractBinder;
		private readonly InstanceCache _instanceCache;
		private readonly InstantiationPolicyRegistry _instantiationPolicyRegistry;
		private readonly UnityObjectContainer _unityObjectContainer;
		private readonly UnityEventsHandler _unityEventsHandler;

		public InstantiationPolicy DefaultInstantiationPolicy
		{
			get => _instantiationPolicyRegistry.DefaultPolicy;
			set => _instantiationPolicyRegistry.DefaultPolicy = value;
		}
		
		private static Container _instance;

		public static Container Initialize(bool handleUnityEvents = true)
		{
			if (_instance == null)
			{
				_instance = new Container(handleUnityEvents);
			}
			return _instance;
		}

		private Container(bool handleUnityEvents)
		{
			_instantiationPolicyRegistry = new InstantiationPolicyRegistry();
			_contractBinder = new ContractBinder();
			_instanceCache = new InstanceCache();
			
			if (handleUnityEvents)
			{
				_unityEventsHandler = new UnityEventsHandler();
				_unityEventsHandler?.TryRegisterInstance(this);
			}
			
			_resolver = new Resolver(_instantiationPolicyRegistry, _contractBinder, _instanceCache, _unityEventsHandler);
			BindInstance(_resolver);
			
			_unityObjectContainer = new UnityObjectContainer();
			BindInstance(_unityObjectContainer);
		}

		public void BindImplementation<TInterface, TType>(InstantiationPolicy instantiationPolicy = InstantiationPolicy.Single) where TType : TInterface 
		{
			_contractBinder.Bind<TInterface, TType>();
			RegisterInstantiationPolicy<TType>(instantiationPolicy);
		}

		public void BindInstance<TType>(TType instance)
		{
			if (instance == null)
			{
				return;
			}
			
			_instanceCache.AddInstance(typeof(TType), instance);
			_unityEventsHandler?.TryRegisterInstance(instance);
		}

		public void BindInstances<TType>(IEnumerable<TType> instances)
		{
			foreach (var instance in instances)
			{
				if (instance == null)
				{
					continue;
				}
				
				_instanceCache.AddInstance(instance.GetType(), instance);
				_unityEventsHandler?.TryRegisterInstance(instance);
			}
		}

		public void RegisterInstantiationPolicy<TType>(InstantiationPolicy instantiationPolicy)
		{
			_instantiationPolicyRegistry.Register(typeof(TType), instantiationPolicy);
		}

		public TType Resolve<TType>()
		{
			return (TType)Resolve(typeof(TType));
		}

		public TType Resolve<TType>(InstantiationPolicy instantiationPolicy)
		{
			return (TType)Resolve(typeof(TType), instantiationPolicy);
		}

		public object Resolve(Type type)
		{
			return _resolver.Resolve(type);
		}

		public object Resolve(Type type, InstantiationPolicy instantiationPolicy)
		{
			return _resolver.Resolve(type, instantiationPolicy);
		}

		public void Dispose()
		{
			_resolver?.Dispose();
			_instantiationPolicyRegistry?.Dispose();
			_contractBinder?.Dispose();
			_instanceCache?.Dispose();
			_unityObjectContainer?.Dispose();
			_unityEventsHandler?.Dispose();
			_instance = null;
		}
	}
}