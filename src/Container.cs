using System;
using System.Collections.Generic;

namespace ThirdParty.npg.bindlessdi
{
	public class Container : IDisposable
	{
		private readonly Resolver _resolver;
		private readonly ContractBinder _contractBinder;
		private readonly InstanceCache _instanceCache;
		private readonly InstantiationPolicyRegistry _instantiationPolicyRegistry;
		private readonly UnityObjectContainer _unityObjectContainer;

		public UnityObjectContainer UnityObjectContainer => _unityObjectContainer;

		public InstantiationPolicy DefaultInstantiationPolicy
		{
			get => _instantiationPolicyRegistry.DefaultPolicy;
			set => _instantiationPolicyRegistry.DefaultPolicy = value;
		}
		
		private static Container _instance;

		public static Container Initialize()
		{
			if (_instance == null)
			{
				_instance = new Container();
				_instance.BindInstance(_instance._unityObjectContainer);
				_instance.BindInstance(_instance._resolver);
				_instance.RegisterInstantiationPolicy<Container>(InstantiationPolicy.Single);
			}

			return _instance;
		}

		private Container()
		{
			_instantiationPolicyRegistry = new InstantiationPolicyRegistry();
			_contractBinder = new ContractBinder();
			_instanceCache = new InstanceCache();
			_resolver = new Resolver(_instantiationPolicyRegistry, _contractBinder, _instanceCache);
			_unityObjectContainer = new UnityObjectContainer();
		}

		public void BindImplementation<TInterface, TType>(InstantiationPolicy instantiationPolicy = InstantiationPolicy.Single) where TType : TInterface 
		{
			_contractBinder.Bind<TInterface, TType>();
			RegisterInstantiationPolicy<TInterface>(instantiationPolicy);
			RegisterInstantiationPolicy<TType>(instantiationPolicy);
		}

		public void BindInstance<TType>(TType instance)
		{
			_instanceCache.AddInstance(typeof(TType), instance);
		}

		public void BindInstances<TType>(IEnumerable<TType> instances)
		{
			foreach (var instance in instances)
			{
				_instanceCache.AddInstance(instance.GetType(), instance);
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
			_instance = null;
		}
	}
}