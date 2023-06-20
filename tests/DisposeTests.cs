using System;
using NUnit.Framework;

namespace npg.bindlessdi.tests
{
	public class DisposeTests
	{
		[Test]
		public void DisposeContainerTest()
		{
			var container = Container.Initialize(false);

			var a = container.Resolve<A>();
			Assert.IsNotNull(a);
			var b = container.Resolve<B>();
			Assert.IsNotNull(b);
			Assert.IsNotNull(b.A);
			
			Assert.IsFalse(a.IsDisposed);
			Assert.IsFalse(b.IsDisposed);
			
			container.Dispose();
			
			Assert.IsTrue(a.IsDisposed);
			Assert.IsTrue(b.IsDisposed);
		}
		
		public class A : IDisposable
		{
			public bool IsDisposed { get; private set; }
			
			public void Dispose()
			{
				IsDisposed = true;
			}
		}

		public class B : IDisposable
		{
			public A A { get; }
			public bool IsDisposed { get; private set; }

			public B(A a)
			{
				A = a;
			}
			
			public void Dispose()
			{
				Assert.IsTrue(A.IsDisposed);
				
				IsDisposed = true;
			}
		}
	}
}