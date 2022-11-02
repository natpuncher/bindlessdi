using System;
using UnityEngine;

namespace npg.bindlessdi
{
	[AddComponentMenu("")]
	internal sealed class UnityEventsListener : MonoBehaviour
	{
		internal Action OnDestroyed;
		internal Action OnFixUpdated;
		internal Action OnUpdated;
		internal Action OnLateUpdated;

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

		private void OnDestroy()
		{
			OnDestroyed?.Invoke();
		}
	}
}