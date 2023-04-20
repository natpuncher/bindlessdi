using System;
using System.Collections.Generic;

namespace npg.bindlessdi.Utils
{
	public class Pool<TType> : IDisposable where TType : IDisposable, new()
	{
		private readonly Queue<TType> _pool;

		public Pool(int capacity = 32)
		{
			_pool = new Queue<TType>(capacity);
		}

		public TType Get()
		{
			return _pool.Count == 0 ? new TType() : _pool.Dequeue();
		}

		public void Return(TType target)
		{
			target.Dispose();
			_pool.Enqueue(target);
		}

		public void Dispose()
		{
			_pool?.Clear();
		}
	}
}