using System;
using UnityEngine;

namespace npg.bindlessdi.UnityLayer
{
	[AddComponentMenu("")]
	internal sealed class UnityEventsListener : MonoBehaviour
	{
		internal Action OnDestroyed;
		internal Action OnFixUpdated;
		internal Action OnUpdated;
		internal Action OnLateUpdated;
		internal Action OnPause;
		internal Action OnUnpause;

		private void FixedUpdate()
		{
			OnFixUpdated?.Invoke();
		}

		private void Update()
		{
			OnUpdated?.Invoke();
		}

		private void LateUpdate()
		{
			OnLateUpdated?.Invoke();
		}
		
#if !UNITY_EDITOR
		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				OnUnpause?.Invoke();
			}
			else
			{
				OnPause?.Invoke();
			}
		}
#else
		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				OnPause?.Invoke();
			}
			else
			{
				OnUnpause?.Invoke();
			}
		}
#endif
		
		private void OnDestroy()
		{
			OnDestroyed?.Invoke();
		}
	}
}