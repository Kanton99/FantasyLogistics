using UnityEngine;
using Unity.Entities;
using Unity.Collections;

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
			if (recipe) SetRecipe(recipe);
		}

		public void SetRecipe(RecipeSO recipe)
		{
			Debug.Log($"new recipe set {recipe.name}");
			recipeEntity = CreateRecipeEntity(recipe);
			this.recipe = recipe;
		}
		private Entity CreateRecipeEntity(RecipeSO recipe)
		{
			// Create a new Recipe entity
			var recipeEntityArchetype = _entityManager.CreateArchetype(
				typeof(RecipeData),
				typeof(RecipeState),
				typeof(RecipeInputs),
				typeof(RecipeOutput)
			);

			var itemEntityArchetype = _entityManager.CreateArchetype(typeof(ItemComponent));

			var recipeEntity = _entityManager.CreateEntity(recipeEntityArchetype);

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

			var outputEntity = _entityManager.CreateEntity(itemEntityArchetype);
			_entityManager.SetComponentData(outputEntity, new ItemComponent
			{
				itemName = recipe.output.itemName,
				amount = 0
			});
			_entityManager.SetComponentData(recipeEntity, new RecipeOutput
			{
				output = outputEntity
			});

			FixedList32Bytes<Entity> inputs = new FixedList32Bytes<Entity>();
			foreach (var item in recipe.inputs)
			{
				var inputEntity = _entityManager.CreateEntity(itemEntityArchetype);
				_entityManager.SetComponentData(inputEntity, new ItemComponent
				{
					itemName = item.itemName,
					amount = 0
				});
				inputs.Add(inputEntity);
			}

			_entityManager.SetComponentData(recipeEntity, new RecipeInputs
			{
				Value = inputs
			});


			return recipeEntity;
		}
	}
}
