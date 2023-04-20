using System;
using System.Collections.Generic;

namespace npg.bindlessdi.Instantiation
{
	internal sealed class Instantiator : IDisposable
	{
		private Dictionary<int, object[]> _cache = new Dictionary<int, object[]>();

		public object Construct(ConstructionInfo info, InstanceBuffer instanceBuffer)
		{
			return info.ConstructorInfo.Invoke(PrepareParameters(instanceBuffer));
		}

		public void Dispose()
		{
			_cache.Clear();
		}

		private object[] PrepareParameters(InstanceBuffer instanceBuffer)
		{
			var instanceCount = instanceBuffer.Instances.Count;
			if (!_cache.TryGetValue(instanceCount, out var buffer))
			{
				buffer = new object[instanceCount];
				_cache[instanceCount] = buffer;
			}

			for (var index = 0; index < instanceCount; index++)
			{
				buffer[index] = instanceBuffer.Instances[index];
			}

			return buffer;
		}
	}
}