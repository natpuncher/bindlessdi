using System;

namespace npg.bindlessdi.Contracts
{
	internal sealed class FactoryTypeResolver
	{
		private static readonly Type FactoryInterfaceType = typeof(IFactory<>);
		private static readonly Type FactoryType = typeof(Factory<>);

		public bool TryResolve(Type type, out Type factoryType)
		{
			factoryType = type;
			
			var isFactoryType = type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == FactoryInterfaceType;
			if (isFactoryType)
			{
				var arguments = type.GetGenericArguments();
				if (arguments.Length != 1)
				{
					isFactoryType = false;
				}
				else
				{
					factoryType = FactoryType.MakeGenericType(arguments);
				}
			}
			return isFactoryType;
		}
	}
}