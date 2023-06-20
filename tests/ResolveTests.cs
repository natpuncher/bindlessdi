using NUnit.Framework;

namespace npg.bindlessdi.tests
{
	public class ResolveTests
	{
		[Test]
		public void ResolveTest()
		{
			var container = Container.Initialize(false);

			var c = container.Resolve<C>();
			Assert.NotNull(c);
			Assert.NotNull(c.A);
			Assert.NotNull(c.B);

			var d = container.Resolve<D>();
			Assert.AreEqual(d.C, c);
			Assert.AreEqual(d.A, c.A);

			container.Dispose();
		}

		[Test]
		public void BindInstanceTest()
		{
			var container = Container.Initialize(false);

			var a = new A();
			container.BindInstance(a);
			var c = container.Resolve<C>();
			Assert.AreEqual(a, c.A);

			container.Dispose();
		}

		[Test]
		public void BindInstancesTest()
		{
			var container = Container.Initialize(false);

			var a = new A();
			var b = new B();

			var instances = new I[] { a, b };
			container.BindInstances(instances);

			var c = container.Resolve<C>();
			Assert.AreEqual(a, c.A);
			Assert.AreEqual(b, c.B);

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

		public class C
		{
			public A A { get; }
			public B B { get; }

			public C(A a, B b)
			{
				A = a;
				B = b;
			}
		}

		public class D
		{
			public C C { get; }
			public A A { get; }

			public D(C c, A a)
			{
				C = c;
				A = a;
			}
		}
	}
}