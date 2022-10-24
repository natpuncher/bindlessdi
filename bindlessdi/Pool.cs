using System;
using System.Collections.Generic;

namespace ThirdParty.npg.bindlessdi
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
			if (!_pool.TryDequeue(out var result))
			{
				result = new TType();
			}

			return result;
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