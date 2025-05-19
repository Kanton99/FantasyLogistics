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
		
		void Destory()
		{
			_entityManager.DestroyEntity(recipeEntity);
		}

		public void SetRecipe(RecipeSO recipe)
		{
			Debug.Log($"new recipe set {recipe.name}");
			if(_entityManager.Exists(recipeEntity)) _entityManager.DestroyEntity(recipeEntity);
			recipeEntity = CreateRecipeEntity(recipe);
			this.recipe = recipe;
		}

		public void InputItem(Entity item)
		{
			if (recipeEntity != null)
			{
				var inputData = _entityManager.GetComponentData<ItemComponent>(item);
				var recipeData = _entityManager.GetComponentData<RecipeInputs>(recipeEntity);
				foreach (var recipeInput in recipeData.Value)
				{
					var invItem = _entityManager.GetComponentData<ItemComponent>(recipeInput);
					if (invItem.itemName == inputData.itemName)
					{
						invItem.amount += inputData.amount;
						_entityManager.SetComponentData(recipeInput, invItem);
						break;
					}
				}
			}
		}

		public Entity GetOutput(int amount)
		{
			if (_entityManager.Exists(recipeEntity))
			{
				var output = _entityManager.GetComponentData<RecipeOutput>(recipeEntity).output;
				var outputItem = _entityManager.GetComponentData<ItemComponent>(output);

				var returnEntity = _entityManager.Instantiate(output);
				var returnEntityComponent = _entityManager.GetComponentData<ItemComponent>(returnEntity);

				int returnAmount = outputItem.amount >= amount ? amount : outputItem.amount;
				outputItem.amount -= returnAmount;
				returnEntityComponent.amount = returnAmount;

				_entityManager.SetComponentData(returnEntity, returnEntityComponent);
				_entityManager.SetComponentData(output, outputItem);
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

		[ContextMenu("Recipe/Fill")]
		public void FillInputs()
		{
			var recipeInput = _entityManager.GetComponentData<RecipeInputs>(recipeEntity).Value;
			foreach(var input in recipeInput){
				var newItem = _entityManager.Instantiate(input);
				var inputItem = _entityManager.GetComponentData<ItemComponent>(newItem);
				inputItem.amount = 100;
				_entityManager.SetComponentData(newItem,inputItem);

				InputItem(newItem);
				_entityManager.DestroyEntity(newItem);
			}
		}
	}
}
