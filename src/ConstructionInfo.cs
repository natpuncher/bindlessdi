using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThirdParty.npg.bindlessdi
{
	internal class ConstructionInfo
	{
		public Type TargetType { get; }
		public ConstructorInfo ConstructorInfo { get; }
		public List<ConstructionInfo> Dependencies { get; }

		public ConstructionInfo(Type targetType, ConstructorInfo constructorInfo)
		{
			TargetType = targetType;
			ConstructorInfo = constructorInfo;
			Dependencies = new List<ConstructionInfo>();
		}
	}
}