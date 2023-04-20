using npg.bindlessdi.Instantiation;

namespace npg.bindlessdi.Contracts
{
	internal sealed class Factory<TType> : IFactory<TType> where TType : class
	{
		private readonly Resolver _resolver;

		public Factory(Resolver resolver)
		{
			_resolver = resolver;
		}

		public TConcrete Resolve<TConcrete>() where TConcrete : TType
		{
			return (TConcrete)_resolver.Resolve(typeof(TConcrete));
		}

		public TConcrete Resolve<TConcrete>(InstantiationPolicy instantiationPolicy) where TConcrete : TType
		{
			return (TConcrete)_resolver.Resolve(typeof(TConcrete), instantiationPolicy);
		}
	}
}