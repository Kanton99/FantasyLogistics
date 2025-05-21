using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

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

		void Destory()
		{
			_entityManager.DestroyEntity(recipeEntity);
		}

		public void SetRecipe(RecipeSO recipe)
		{
			// Debug.Log($"new recipe set {recipe.name}");
			if (_entityManager.Exists(recipeEntity)) _entityManager.DestroyEntity(recipeEntity);
			recipeEntity = CreateRecipeEntity(recipe);
			this.recipe = recipe;
		}

		public void InputItem(Entity item)
		{
			if (_entityManager.Exists(recipeEntity))
			{
				var inputData = _entityManager.GetComponentData<ItemComponent>(item);
				DynamicBuffer<RecipeInputs> recipeData = _entityManager.GetBuffer<RecipeInputs>(recipeEntity);
				for (int i = 0; i < recipeData.Length; i++)
				{
					var recipeInput = recipeData[i];
					if (recipeInput.itemName == inputData.itemName)
					{
						recipeData[i] = new RecipeInputs
						{
							itemName = recipeInput.itemName,
							amount = recipeInput.amount + inputData.amount
						};
						break;
					}
				}
			}
		}

		public Entity GetOutput(int amount)
		{
			if (_entityManager.Exists(recipeEntity))
			{
				var outputData = _entityManager.GetComponentData<RecipeOutput>(recipeEntity);
				int returnAmount = outputData.amount >= amount ? amount : outputData.amount;
				_entityManager.SetComponentData(recipeEntity, new RecipeOutput
				{
					itemName = outputData.itemName,
					amount = outputData.amount - returnAmount
				});

				var returnEntity = _entityManager.CreateEntity(
						typeof(ItemComponent)
						);
				_entityManager.SetComponentData(recipeEntity, new ItemComponent
				{
					itemName = outputData.itemName,
					amount = returnAmount
				});

				return returnEntity;

			}
			throw new System.Exception("trying to get output from unset building");
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
				timeRemaining = 0f,
				position = new float2(gameObject.transform.position.x, gameObject.transform.position.y)
			});

			_entityManager.SetComponentData(recipeEntity, new RecipeOutput
			{
				itemName = recipe.output.itemName,
				amount = 0
			});

			DynamicBuffer<RecipeInputs> recipeInputs = _entityManager.GetBuffer<RecipeInputs>(recipeEntity);
			recipeInputs.EnsureCapacity(recipe.inputs.Length);

			foreach (var item in recipe.inputs)
			{
				recipeInputs.Add(new RecipeInputs
				{
					itemName = item.itemName,
					amount = item.amout
				});
			}

			return recipeEntity;
		}

		[ContextMenu("Recipe/Fill")]
		public void FillInputs()
		{
			DynamicBuffer<RecipeInputs> recipeInputs = _entityManager.GetBuffer<RecipeInputs>(recipeEntity);
			for (int i = 0; i < recipeInputs.Length; i++)
			{
				recipeInputs[i] = new RecipeInputs
				{
					itemName = recipeInputs[i].itemName,
					amount = 100
				};
			}
		}
	}
}
