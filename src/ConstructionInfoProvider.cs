using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ThirdParty.npg.bindlessdi
{
	internal class ConstructionInfoProvider : IDisposable
	{
		private readonly ConstructionValidator _constructionValidator = new();
		private readonly Dictionary<Type, ConstructionInfo> _instantiationInfos = new();
		private readonly FactoryTypeResolver _factoryTypeResolver = new();
		
		public bool TryGetInfo(Type type, out ConstructionInfo info)
		{
			if (!_instantiationInfos.TryGetValue(type, out info))
			{
				info = CreateInstantiationInfo(type);
				if (info == null)
				{
					return false;
				}
				
				_instantiationInfos[type] = info;
			}

			return true;
		}
		
		public void Dispose()
		{
			_instantiationInfos?.Clear();
			_constructionValidator?.Dispose();
		}

		private ConstructionInfo CreateInstantiationInfo(Type type)
		{
			var targetType = type;
			if (_factoryTypeResolver.TryResolve(targetType, out var factoryType))
			{
				targetType = factoryType;
			}

			if (!_constructionValidator.IsTypeValid(targetType))
			{
				Debug.LogError($"Can't create instantiation info for {targetType.FullName}: type is invalid");
				return null;
			}
			
			var constructor = _constructionValidator.GetValidConstructor(targetType);
			if (constructor == null)
			{
				Debug.LogError($"Can't create instantiation info for {targetType.FullName}: no valid constructor found");
				return null;
			}

			var info = new ConstructionInfo(targetType, constructor);
			if (!TryProcessParameters(constructor, info))
			{
				return null;
			}

			return info;
		}

		private bool TryProcessParameters(ConstructorInfo constructor, ConstructionInfo info)
		{
			// todo add circular dependency check
			// todo add container check
			var parameters = constructor.GetParameters();
			var length = parameters.Length;
			for (var index = 0; index < length; index++)
			{
				var parameter = parameters[index];
				if (!TryGetInfo(parameter.ParameterType, out var dependencyInfo))
				{
					return false;
				}

				info.Dependencies.Add(dependencyInfo.TargetType);
			}

			return true;
		}
	}
}