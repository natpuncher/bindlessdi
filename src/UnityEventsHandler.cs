using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdParty.npg.bindlessdi
{
	internal class UnityEventsHandler : IDisposable
	{
		private readonly List<IFixedTickable> _fixTickables = new();
		private readonly List<ITickable> _tickables = new();
		private readonly List<ILateTickable> _lateTickables = new();
		private readonly List<IDisposable> _disposables = new();

		private int _fixTickablesCount;
		private int _tickablesCount;
		private int _lateTickablesCount;
		
		public UnityEventsHandler()
		{
			var listenerGameObject = new GameObject
			{
				name = "UnityEventsListener",
				hideFlags = HideFlags.HideInHierarchy
			};
			var listener = listenerGameObject.AddComponent<UnityEventsListener>();
			UnityEngine.Object.DontDestroyOnLoad(listenerGameObject);
			
			listener.OnFixUpdated += OnFixUpdated;
			listener.OnUpdated += OnUpdated;
			listener.OnLateUpdated += OnLateUpdated;
			listener.OnDestroyed += OnDestroyed;
		}

		public void TryRegisterInstance(object instance)
		{
			if (instance is IDisposable disposable)
			{
				_disposables.Add(disposable);
			}
			
			if (instance is IFixedTickable fixedTickable)
			{
				_fixTickables.Add(fixedTickable);
			}

			if (instance is ITickable tickable)
			{
				_tickables.Add(tickable);
			}
			
			if (instance is ILateTickable lateTickable)
			{
				_lateTickables.Add(lateTickable);
			}

			_fixTickablesCount = _fixTickables.Count;
			_tickablesCount = _tickables.Count;
			_lateTickablesCount = _lateTickables.Count;
		}

		public void Dispose()
		{
			_fixTickables.Clear();
			_tickables.Clear();
			_lateTickables.Clear();
			_disposables.Clear();
			
			_fixTickablesCount = 0;
			_tickablesCount = 0;
			_lateTickablesCount = 0;
		}

		private void OnFixUpdated()
		{
			for (var i = 0; i < _fixTickablesCount; i++)
			{
				_fixTickables[i]?.FixedTick();
			}
		}
		
		private void OnUpdated()
		{
			for (var i = 0; i < _tickablesCount; i++)
			{
				_tickables[i]?.Tick();
			}
		}
		
		private void OnLateUpdated()
		{
			for (var i = 0; i < _lateTickablesCount; i++)
			{
				_lateTickables[i]?.LateTick();
			}
		}

		private void OnDestroyed()
		{
			foreach (var disposable in _disposables)
			{
				disposable?.Dispose();
			}
		}
	}
}