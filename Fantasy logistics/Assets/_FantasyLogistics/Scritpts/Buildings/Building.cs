using Unity.Entities;

namespace FantasyLogistics{
	public struct Building : IComponentData
	{
		public Recipe recipe;
		public ResourceAmount[] inventory;
		public bool running;
	}
}
