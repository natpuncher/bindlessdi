using System;
using System.Collections.Generic;
using System.Reflection;

namespace npg.bindlessdi
{
	internal sealed class ConstructionValidator : IDisposable
	{
		private static readonly Type ContainerType = typeof(Container);
		
		private readonly Dictionary<Type, bool> _typeValidity = new Dictionary<Type, bool>();
		
		public bool IsTypeValid(Type type)
		{
			if (!_typeValidity.TryGetValue(type, out var isValid))
			{
				isValid = IsTypeValidInternal(type);
				_typeValidity[type] = isValid;
			}

			return isValid;
		}

		public ConstructorInfo GetValidConstructor(Type type)
		{
			var constructors = GetConstructors(type);
			if (constructors == null || constructors.Length == 0)
			{
				return null;
			}

			return constructors[0];
		}

		public void Dispose()
		{
			_typeValidity.Clear();
		}
		
		private ConstructorInfo[] GetConstructors(Type type)
		{
			return type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}

		private bool IsTypeValidInternal(Type type)
		{
			var isContainer = type == ContainerType;
			return !isContainer && 
			       !type.IsAbstract && 
			       !type.IsValueType;
		}
	}
}