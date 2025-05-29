using UnityEngine;

namespace FantasyLogistics
{
	public class BuildingSpawner : MonoBehaviour
	{
		public GameObject buildingPrefab;
		public Vector3 position;
		public bool spawn;

		private void Update()
		{
			if (spawn) spawn = false;
		}
	}

}
