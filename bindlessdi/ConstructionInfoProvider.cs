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
		
		private readonly ContractBinder _contractBinder;

		public ConstructionInfoProvider(ContractBinder contractBinder)
		{
			_contractBinder = contractBinder;
		}

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
			if (!_contractBinder.TryGetImplementation(type, out var resolveType))
			{
				resolveType = type;
			}

			if (_factoryTypeResolver.TryResolve(resolveType, out var factoryType))
			{
				resolveType = factoryType;
			}

			if (!_constructionValidator.IsTypeValid(resolveType))
			{
				Debug.LogError($"Can't create instantiation info for {resolveType.FullName}: type is invalid");
				return null;
			}
			
			var constructor = _constructionValidator.GetValidConstructor(resolveType);
			if (constructor == null)
			{
				Debug.LogError($"Can't create instantiation info for {resolveType.FullName}: no valid constructor found");
				return null;
			}

			var info = new ConstructionInfo(resolveType, constructor);
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

				info.Dependencies.Add(dependencyInfo);
			}

			return true;
		}
	}
}