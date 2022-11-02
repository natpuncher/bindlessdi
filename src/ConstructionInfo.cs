using System;
using System.Collections.Generic;
using System.Reflection;

namespace npg.bindlessdi
{
	internal sealed class ConstructionInfo
	{
		public Type TargetType { get; }
		public ConstructorInfo ConstructorInfo { get; }
		public List<Type> Dependencies { get; }

		public ConstructionInfo(Type targetType, ConstructorInfo constructorInfo)
		{
			TargetType = targetType;
			ConstructorInfo = constructorInfo;
			Dependencies = new List<Type>();
		}
	}
}