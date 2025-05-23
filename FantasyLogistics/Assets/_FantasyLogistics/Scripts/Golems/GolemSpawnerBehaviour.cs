using UnityEngine;
using Unity.Mathematics;

namespace FantasyLogistics
{
	class GolemInstantiatorAuthoring : MonoBehaviour
	{
		public GameObject golemEntityPrefab;
		public float3 position;
		public bool place;

		private void Update()
		{
			if (place) place = false;
		}
	}
}
