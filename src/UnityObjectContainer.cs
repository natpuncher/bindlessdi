using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace ThirdParty.npg.bindlessdi
{
	public class UnityObjectContainer : IDisposable
	{
		private readonly Dictionary<Type, Object> _data = new Dictionary<Type, Object>();

		private List<Type> _buffer = new List<Type>(32);

		public UnityObjectContainer()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			ProcessScene(SceneManager.GetActiveScene());
		}

		public void Bind(Object data)
		{
			if (data == null)
			{
				return;
			}
			
			var type = data.GetType();
			_data[type] = data;
		}
		
		public bool TryGetObject<TObject>(out TObject data) where TObject : Object
		{
			var type = typeof(TObject);
			if (_data.TryGetValue(type, out var result))
			{
				data = (TObject)result;
				return true;
			}

			data = null;
			return false;
		}

		public void Dispose()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			_data.Clear();
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			ProcessScene(scene);
		}

		private void ProcessScene(Scene scene)
		{
			foreach (var gameObject in scene.GetRootGameObjects())
			{
				var sceneContext = gameObject.GetComponent<SceneContext>();
				if (sceneContext == null)
				{
					continue;
				}

				BindSceneContext(sceneContext);
			}
			
			CleanUnused();
		}

		private void BindSceneContext(SceneContext sceneContext)
		{
			foreach (var data in sceneContext.GetObjects())
			{
				Bind(data);
			}
		}

		private void CleanUnused()
		{
			_buffer.Clear();
			foreach (var dataPair in _data)
			{
				if (dataPair.Value == null)
				{
					_buffer.Add(dataPair.Key);
				}
			}

			foreach (var type in _buffer)
			{
				_data.Remove(type);
			}
			_buffer.Clear();
		}
	}
}