using Unity.Entities;

namespace FantasyLogistics
{
	public struct Building : IComponentData
	{
		public Recipe recipe;
		public ResourceAmount[] inputInventory;
		public ResourceAmount[] outputInventory;
		public bool running;
		public float runningTime;
		public BuildingType type;
	}

	[System.Serializable]
	public enum BuildingType
	{
		Alchemy,
		Ritual
	}
}
