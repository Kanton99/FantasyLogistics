using Unity.Entities;

namespace FantasyLogistics
{
	public struct BuildingComponent : IComponentData
	{
		public RecipeBlob recipe;
		public Entity recipeEntity;
	}
}
