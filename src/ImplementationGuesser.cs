using System;
using System.Collections.Generic;
using UnityEngine;

namespace npg.bindlessdi
{
	public class ImplementationGuesser : IDisposable
	{
		private Dictionary<Type, List<Type>> _foundImplementations = new Dictionary<Type, List<Type>>();

		public bool TryGuess(Type type, out Type implementationType)
		{
			if (!type.IsInterface)
			{
				implementationType = type;
				return false;
			}

			if (_foundImplementations.Count == 0)
			{
				InitializeImplementationCache();
			}

			if (_foundImplementations.TryGetValue(type, out var implementationTypes))
			{
				implementationType = implementationTypes[0];
				Debug.LogWarning($"[bindlessdi] Implementation guessed {implementationType.FullName} : {type.FullName}");
				
				if (implementationTypes.Count > 1)
				{
					Debug.LogError($"[bindlessdi] Ambiguous implementations found for {type.FullName} -> ({string.Join(" | ", implementationTypes)}), " +
					               $"intended implementation should be binded!");
				}

				return true;
			}

			Debug.LogError($"[bindlessdi] No implementation found for {type.FullName}!");
			implementationType = type;
			return false;
		}

		public void Dispose()
		{
			_foundImplementations.Clear();
		}

		private void InitializeImplementationCache()
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes())
				{
					if (type.IsInterface || type.IsAbstract)
					{
						continue;
					}

					FindImplementations(type);
				}
			}
		}

		private void FindImplementations(Type type)
		{
			foreach (var inter in type.GetInterfaces())
			{
				if (!_foundImplementations.TryGetValue(inter, out var list))
				{
					list = new List<Type>();
					_foundImplementations[inter] = list;
				}

				list.Add(type);
			}
		}
	}
}