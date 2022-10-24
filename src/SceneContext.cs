using System.Collections.Generic;
using UnityEngine;

namespace ThirdParty.npg.bindlessdi
{
	public abstract class SceneContext : MonoBehaviour
	{
		public abstract IEnumerable<Object> GetObjects();
	}
}