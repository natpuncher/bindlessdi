using System.Collections.Generic;
using UnityEngine;

namespace npg.bindlessdi
{
	public abstract class SceneContext : MonoBehaviour
	{
		public abstract IEnumerable<Object> GetObjects();
	}
}