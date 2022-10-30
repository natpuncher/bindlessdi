using System;
using System.Collections.Generic;
using System.Linq;

namespace ThirdParty.npg.bindlessdi
{
	internal sealed class CircularDependencyAnalyzer : IDisposable
	{
		private readonly HashSet<Type> _hashSet = new HashSet<Type>();
		private readonly Stack<Type> _stack = new Stack<Type>();

		public bool Validate(Type type)
		{
			if (_hashSet.Contains(type))
			{
				return false;
			}

			_hashSet.Add(type);
			_stack.Push(type);
			return true;
		}

		public void ReleaseLast()
		{
			if (_stack.Count == 0)
			{
				return;
			}

			var type = _stack.Pop();
			if (_hashSet.Contains(type))
			{
				_hashSet.Remove(type);
			}
		}

		public override string ToString()
		{
			return string.Join("\n", _stack.Reverse());
		}

		public void Dispose()
		{
			_hashSet.Clear();
			_stack.Clear();
		}
	}
}