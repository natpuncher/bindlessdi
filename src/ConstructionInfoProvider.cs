using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace npg.bindlessdi
{
	internal sealed class ConstructionInfoProvider : IDisposable
	{
		private readonly InstanceCache _instanceCache;
		private readonly ContractBinder _contractBinder;
		private readonly ConstructionValidator _constructionValidator = new ConstructionValidator();
		private readonly Dictionary<Type, ConstructionInfo> _instantiationInfos = new Dictionary<Type, ConstructionInfo>();
		private readonly FactoryTypeResolver _factoryTypeResolver = new FactoryTypeResolver();
		private readonly CircularDependencyAnalyzer _circularDependencyAnalyzer = new CircularDependencyAnalyzer();

		public ConstructionInfoProvider(InstanceCache instanceCache, ContractBinder contractBinder)
		{
			_instanceCache = instanceCache;
			_contractBinder = contractBinder;
		}

		public bool TryGetInfo(Type type, out ConstructionInfo info)
		{
			_circularDependencyAnalyzer.Dispose();
			return TryGetInfoInternal(type, out info);
		}

		private bool TryGetInfoInternal(Type type, out ConstructionInfo info)
		{
			if (!_instantiationInfos.TryGetValue(type, out info))
			{
				if (!_circularDependencyAnalyzer.Validate(type))
				{
					Debug.LogError($"[bindlessdi] Circular dependency found!\n{_circularDependencyAnalyzer}");
					info = null;
					return false;
				}

				info = CreateInstantiationInfo(type);
				if (info == null)
				{
					return false;
				}

				_instantiationInfos[type] = info;
				_circularDependencyAnalyzer.ReleaseLast();
			}

			return true;
		}

		public void Dispose()
		{
			_instantiationInfos?.Clear();
			_constructionValidator?.Dispose();
			_circularDependencyAnalyzer?.Dispose();
		}

		private ConstructionInfo CreateInstantiationInfo(Type type)
		{
			if (!_contractBinder.TryGetTargetType(type, out var targetType))
			{
				targetType = type;
			}

			if (_factoryTypeResolver.TryResolve(targetType, out var factoryType))
			{
				targetType = factoryType;
			}

			if (!_constructionValidator.IsTypeValid(targetType))
			{
				Debug.LogError($"[bindlessdi] Can't create instantiation info for {targetType.FullName}: type is invalid\n{_circularDependencyAnalyzer}");
				return null;
			}

			var constructor = _constructionValidator.GetValidConstructor(targetType);
			if (constructor == null)
			{
				Debug.LogError(
					$"[bindlessdi] Can't create instantiation info for {targetType.FullName}: no valid constructor found\n{_circularDependencyAnalyzer}");
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
			var parameters = constructor.GetParameters();
			var length = parameters.Length;
			for (var index = 0; index < length; index++)
			{
				var parameter = parameters[index];
				var parameterType = parameter.ParameterType;
				info.Dependencies.Add(parameterType);
				var hasInstance = _instanceCache.TryGetInstance(parameterType, out _);
				if (hasInstance)
				{
					continue;
				}
				
				if (!TryGetInfoInternal(parameterType, out _))
				{
					return false;
				}
			}

			return true;
		}
	}
}