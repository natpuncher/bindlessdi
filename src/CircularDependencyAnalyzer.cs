using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace npg.bindlessdi
{
	internal sealed class CircularDependencyAnalyzer : IDisposable
	{
		private readonly HashSet<Type> _hashSet = new HashSet<Type>();
		private readonly Stack<Type> _stack = new Stack<Type>();

		private readonly StringBuilder _stringBuilder = new StringBuilder();

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
			_stringBuilder.Clear();
			var count = _stack.Count;
			foreach (var type in _stack.Reverse())
			{
				count--;
				
				_stringBuilder.Append(type.FullName);
				if (count > 0)
				{
					_stringBuilder.Append(" -> ");
					_stringBuilder.AppendLine();
				}
			}

			return _stringBuilder.ToString();
		}

		public void Dispose()
		{
			_hashSet.Clear();
			_stack.Clear();
		}
	}
}