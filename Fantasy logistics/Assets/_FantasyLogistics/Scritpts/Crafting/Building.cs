using UnityEngine;
using Unity.Entities;

namespace FantasyLogistics
{
	public class Building : MonoBehaviour
	{
		public RecipeSO recipe;
		private Entity recipeEntity;
		private EntityManager _entityManager;

		void Start()
		{
			_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			if (!recipe) SetRecipe(recipe);
		}

		public void SetRecipe(RecipeSO recipe)
		{
			recipeEntity = CreateRecipeEntity(recipe);
			this.recipe = recipe;
		}
		private Entity CreateRecipeEntity(RecipeSO recipe)
		{
			// Create a new Recipe entity
			var entityArchetype = _entityManager.CreateArchetype(
				typeof(RecipeData),
				typeof(RecipeState)
			);

			var recipeEntity = _entityManager.CreateEntity(entityArchetype);

			var recipyBlob = recipe.CreateRecipeData();
			// Initialize with data
			_entityManager.SetComponentData(recipeEntity, new RecipeData
			{
				recipeBlob = recipyBlob
			});

			_entityManager.SetComponentData(recipeEntity, new RecipeState
			{
				isProcessing = false,
				timeRemaining = 0f
			});

			return recipeEntity;
		}
	}
}
