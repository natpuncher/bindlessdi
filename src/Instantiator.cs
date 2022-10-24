
namespace ThirdParty.npg.bindlessdi
{
	internal class Instantiator
	{
		public object Construct(ConstructionInfo info, InstanceBuffer instanceBuffer)
		{
			return info.ConstructorInfo.Invoke(instanceBuffer.Instances.ToArray());
		}
	}
}