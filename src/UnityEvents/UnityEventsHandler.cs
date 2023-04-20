using System;
using System.Collections.Generic;
using UnityEngine;

namespace npg.bindlessdi.UnityEvents
{
	internal sealed class UnityEventsHandler : IDisposable
	{
		private readonly List<IFixedTickable> _fixTickables = new List<IFixedTickable>();
		private readonly List<ITickable> _tickables = new List<ITickable>();
		private readonly List<ILateTickable> _lateTickables = new List<ILateTickable>();
		private readonly List<IPausable> _pausables = new List<IPausable>();
		private readonly List<IDisposable> _disposables = new List<IDisposable>();

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
			listener.OnPause += OnPause;
			listener.OnUnpause += OnUnpause;
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

			if (instance is IPausable pausable)
			{
				_pausables.Add(pausable);
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
		
		private void OnPause()
		{
			for (var i = _pausables.Count - 1; i >= 0; i--)
			{
				_pausables[i]?.Pause();
			}
		}

		private void OnUnpause()
		{
			for (var i = _pausables.Count - 1; i >= 0; i--)
			{
				_pausables[i]?.Unpause();
			}
		}

		private void OnDestroyed()
		{
			for (var i = _disposables.Count - 1; i >= 0; i--)
			{
				_disposables[i]?.Dispose();
			}
		}
	}
}