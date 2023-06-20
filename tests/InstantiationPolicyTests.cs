using npg.bindlessdi.Instantiation;
using NUnit.Framework;

namespace npg.bindlessdi.tests
{
	public class InstantiationPolicyTests
	{
		[Test]
		public void DefaultInstantiationPolicyTest()
		{
			var container = Container.Initialize(false);
			
			container.DefaultInstantiationPolicy = InstantiationPolicy.Transient;
			
			var a = container.Resolve<A>();
			var a2 = container.Resolve<A>();
			
			Assert.AreEqual(a.GetType(), a2.GetType());
			Assert.AreNotEqual(a, a2);

			container.DefaultInstantiationPolicy = InstantiationPolicy.Single;
			
			a = container.Resolve<A>();
			a2 = container.Resolve<A>();
			
			Assert.AreEqual(a.GetType(), a2.GetType());
			Assert.AreEqual(a, a2);
			
			container.Dispose();
		}

		[Test]
		public void RegisterInstantiationPolicyOverrideTest()
		{
			var container = Container.Initialize(false);

			container.RegisterInstantiationPolicy<A>(InstantiationPolicy.Transient);
			
			var a = container.Resolve<A>();
			var a2 = container.Resolve<A>();
			
			Assert.AreEqual(a.GetType(), a2.GetType());
			Assert.AreNotSame(a, a2);
			
			container.RegisterInstantiationPolicy<A>(InstantiationPolicy.Single);
			
			a = container.Resolve<A>();
			a2 = container.Resolve<A>();
			
			Assert.AreEqual(a.GetType(), a2.GetType());
			Assert.AreEqual(a, a2);
			
			container.Dispose();
		}
		
		[Test]
		public void ResolveInstantiationPolicyOverrideTest()
		{
			var container = Container.Initialize(false);

			var a = container.Resolve<A>();
			var a2 = container.Resolve<A>();
			
			Assert.AreEqual(a.GetType(), a2.GetType());
			Assert.AreEqual(a, a2);
			
			a = container.Resolve<A>(InstantiationPolicy.Transient);
			a2 = container.Resolve<A>(InstantiationPolicy.Transient);
			
			Assert.AreEqual(a.GetType(), a2.GetType());
			Assert.AreNotEqual(a, a2);
			
			container.Dispose();
		}

		[Test]
		public void InstantiationPolicyByBaseTypeTest()
		{
			var container = Container.Initialize(false);
			
			container.RegisterInstantiationPolicy<I>(InstantiationPolicy.Transient);
			var b = container.Resolve<B>();
			var b2 = container.Resolve<B>();
			var b3 = container.Resolve<I>();
			
			Assert.AreNotEqual(b, b2);
			Assert.AreNotEqual(b2, b3);
			Assert.AreNotEqual(b3, b);
			
			container.Dispose();
		}

		public class A
		{
		}

		public interface I
		{
		}

		public class B : I
		{
		}
	}
}