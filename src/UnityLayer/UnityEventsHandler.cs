using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace npg.bindlessdi.UnityLayer
{
	internal sealed class UnityEventsHandler : IDisposable
	{
		private readonly List<IFixedTickable> _fixTickables = new List<IFixedTickable>();
		private readonly List<ITickable> _tickables = new List<ITickable>();
		private readonly List<ILateTickable> _lateTickables = new List<ILateTickable>();
		private readonly List<IPausable> _pausables = new List<IPausable>();
		
		private readonly UnityEventsListener _unityEventsListener;

		internal event Action Destroyed;

		private int _fixTickablesCount;
		private int _tickablesCount;
		private int _lateTickablesCount;
		
		public UnityEventsHandler()
		{
			var listenerGameObject = new GameObject
			{
				name = "[bindlessdi] UnityEventsListener",
				hideFlags = HideFlags.HideInHierarchy
			};
			_unityEventsListener = listenerGameObject.AddComponent<UnityEventsListener>();
			Object.DontDestroyOnLoad(listenerGameObject);
			
			_unityEventsListener.OnFixUpdated += OnFixUpdated;
			_unityEventsListener.OnUpdated += OnUpdated;
			_unityEventsListener.OnLateUpdated += OnLateUpdated;
			_unityEventsListener.OnPause += OnPause;
			_unityEventsListener.OnUnpause += OnUnpause;
			_unityEventsListener.OnDestroyed += OnDestroyed;
		}

		public void TryRegisterInstance(object instance)
		{
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
			Destroyed = null;
			ClearEventListener();
			
			_fixTickables.Clear();
			_tickables.Clear();
			_lateTickables.Clear();
			
			_fixTickablesCount = 0;
			_tickablesCount = 0;
			_lateTickablesCount = 0;
		}

		private void ClearEventListener()
		{
			if (_unityEventsListener == null)
			{
				return;
			}
			
			_unityEventsListener.OnFixUpdated -= OnFixUpdated;
			_unityEventsListener.OnUpdated -= OnUpdated;
			_unityEventsListener.OnLateUpdated -= OnLateUpdated;
			_unityEventsListener.OnPause -= OnPause;
			_unityEventsListener.OnUnpause -= OnUnpause;
			_unityEventsListener.OnDestroyed -= OnDestroyed;
			
			Object.Destroy(_unityEventsListener.gameObject);
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
			Destroyed?.Invoke();
		}
	}
}