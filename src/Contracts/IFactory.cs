using npg.bindlessdi.Instantiation;

namespace npg.bindlessdi.Contracts
{
	public interface IFactory<TType> where TType : class
	{
		TConcrete Resolve<TConcrete>() where TConcrete : TType;
		TConcrete Resolve<TConcrete>(InstantiationPolicy instantiationPolicy) where TConcrete : TType;
	}
}