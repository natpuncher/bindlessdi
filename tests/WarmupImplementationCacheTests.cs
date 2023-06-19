using NUnit.Framework;

namespace npg.bindlessdi.tests
{
	public class WarmupImplementationCacheTests
	{
		[Test]
		public void OnDemandWarmupTest()
		{
			var container = Container.Initialize(false);
			
			Assert.IsFalse(container.IsImplementationCacheInitialized);
			var a = container.Resolve<A>();
			Assert.NotNull(a);
			Assert.IsFalse(container.IsImplementationCacheInitialized);
			
			var b = container.Resolve<B>();
			Assert.NotNull(b);
			Assert.IsFalse(container.IsImplementationCacheInitialized);

			var c = container.Resolve<C>();
			Assert.NotNull(c);
			Assert.NotNull(c.I);
			Assert.AreEqual(c.I, b);
			// guessing an implementation of I initializes the implementation cache
			Assert.IsTrue(container.IsImplementationCacheInitialized);
			
			container.Dispose();
		}

		[Test]
		public void ForceWarmupTest()
		{
			var container = Container.Initialize(false);
			
			Assert.IsFalse(container.IsImplementationCacheInitialized);
			var a = container.Resolve<A>();
			Assert.NotNull(a);
			Assert.IsFalse(container.IsImplementationCacheInitialized);
			
			container.WarmupImplementationCache();
			Assert.IsTrue(container.IsImplementationCacheInitialized);
			var b = container.Resolve<B>();
			Assert.NotNull(b);
			Assert.IsTrue(container.IsImplementationCacheInitialized);
			
			container.Dispose();
		}
		
		public interface I
		{
		}

		public class A
		{
		}

		public class B : I
		{
		}

		public class C
		{
			public I I { get; }

			public C(I i)
			{
				I = i;
			}
		}
	}
}