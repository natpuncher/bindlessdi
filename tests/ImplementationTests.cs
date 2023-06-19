using npg.bindlessdi.Instantiation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace npg.bindlessdi.tests
{
	public class ImplementationTests
	{
		[Test]
		public void ImplementationGuessTest()
		{
			var container = Container.Initialize(false);

			var d = container.Resolve<D>();
			LogAssert.Expect(LogType.Warning,
				"[bindlessdi] Implementation guessed npg.bindlessdi.tests.ImplementationTests+A : " +
				"npg.bindlessdi.tests.ImplementationTests+I1");
			
			Assert.AreEqual(typeof(A), d.I1.GetType());

			var e = container.Resolve<E>();
			LogAssert.Expect(LogType.Error, 
				"[bindlessdi] Ambiguous implementations found for npg.bindlessdi.tests.ImplementationTests+I2 -> " +
				"(npg.bindlessdi.tests.ImplementationTests+B | npg.bindlessdi.tests.ImplementationTests+C), " +
				"intended implementation should be binded!");
			
			Assert.NotNull(e.I2);

			container.Dispose();
		}

		[Test]
		public void BindImplementationTest()
		{
			var container = Container.Initialize(false);

			container.BindImplementation<I2, B>();
			var e = container.Resolve<E>();
			Assert.AreEqual(typeof(B), e.I2.GetType());

			container.Dispose();
		}

		[Test]
		public void ChangeImplementationTest()
		{
			var container = Container.Initialize(false);

			container.BindImplementation<I2, B>();
			var e = container.Resolve<E>();
			Assert.AreEqual(typeof(B), e.I2.GetType());
			
			container.BindImplementation<I2, C>();
			// passing InstantiationPolicy.Transient to create a new instance
			e = container.Resolve<E>(InstantiationPolicy.Transient);
			Assert.AreEqual(typeof(C), e.I2.GetType());

			container.Dispose();
		}

		public interface I1
		{
		}

		public interface I2
		{
		}

		public class A : I1
		{
		}

		public class B : I2
		{
		}

		public class C : I2
		{
		}

		public class D
		{
			public I1 I1 { get; }

			public D(I1 i1)
			{
				I1 = i1;
			}
		}

		public class E
		{
			public I2 I2 { get; }

			public E(I2 i2)
			{
				I2 = i2;
			}
		}
	}
}