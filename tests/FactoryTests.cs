using npg.bindlessdi.Contracts;
using npg.bindlessdi.Instantiation;
using NUnit.Framework;

namespace npg.bindlessdi.tests
{
	public class FactoryTests
	{
		[Test]
		public void ResolveTest()
		{
			var container = Container.Initialize(false);

			var spawner = container.Resolve<Spawner>();
			var a = container.Resolve<A>();
			var a2 = spawner.SpawnA();
			Assert.AreEqual(a, a2);
			
			container.Dispose();
		}

		[Test]
		public void MultipleResolveTest()
		{
			var container = Container.Initialize(false);

			var spawner = container.Resolve<Spawner>();
			var b = container.Resolve<B>();
			var b2 = spawner.SpawnB();
			var b3 = spawner.SpawnB();
			Assert.AreNotEqual(b, b2);
			Assert.AreNotEqual(b2, b3);
			
			container.Dispose();
		}
		
		public interface I
		{
		}

		public class A : I
		{
		}

		public class B : I
		{
		}

		public class Spawner
		{
			private readonly IFactory<I> _factory;

			public Spawner(IFactory<I> factory)
			{
				_factory = factory;
			}

			public I SpawnA()
			{
				return _factory.Resolve<A>();
			}

			public I SpawnB()
			{
				return _factory.Resolve<B>(InstantiationPolicy.Transient);
			}
		}
	}
}