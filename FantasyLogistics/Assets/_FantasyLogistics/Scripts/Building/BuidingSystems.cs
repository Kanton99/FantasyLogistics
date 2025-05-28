// using Unity.Entities;
// using Unity.Burst;
//
// namespace FantasyLogistics
// {
// 	public struct BuildingMethods
// 	{
// 		public static void SetRecipe(BuildingComponent building, BlobAssetReference<RecipeBlob> recipe, EntityManager entityManager)
// 		{
// 			if (entityManager.Exists(building.recipeEntity)) entityManager.DestroyEntity(building.recipeEntity);
// 			building.recipeEntity = CreateRecipeEntity(recipe, entityManager);
// 		}
//
// 		public static void InputItem(BuildingComponent building, ItemComponent item, EntityManager entityManager)
// 		{
// 			Entity recipeEntity = building.recipeEntity;
// 			if (entityManager.Exists(recipeEntity))
// 			{
// 				DynamicBuffer<RecipeInputs> recipeData = entityManager.GetBuffer<RecipeInputs>(recipeEntity);
// 				for (int i = 0; i < recipeData.Length; i++)
// 				{
// 					var recipeInput = recipeData[i];
// 					if (recipeInput.itemName == item.itemName)
// 					{
// 						recipeData[i] = new RecipeInputs
// 						{
// 							itemName = recipeInput.itemName,
// 							amount = recipeInput.amount + item.amount
// 						};
// 						break;
// 					}
// 				}
// 			}
// 		}
//
// 		public static Entity GetOutput(BuildingComponent building, int amount, EntityManager entityManager) //TODO Review creation of Item Entity
// 		{
// 			Entity recipeEntity = building.recipeEntity;
// 			if (entityManager.Exists(recipeEntity))
// 			{
// 				var outputData = entityManager.GetComponentData<RecipeOutput>(recipeEntity);
// 				int returnAmount = outputData.amount >= amount ? amount : outputData.amount;
// 				entityManager.SetComponentData(recipeEntity, new RecipeOutput
// 				{
// 					itemName = outputData.itemName,
// 					amount = outputData.amount - returnAmount
// 				});
//
// 				var returnEntity = entityManager.CreateEntity(
// 						typeof(ItemComponent)
// 						);
// 				entityManager.SetComponentData(recipeEntity, new ItemComponent
// 				{
// 					itemName = outputData.itemName,
// 					amount = returnAmount
// 				});
//
// 				return returnEntity;
//
// 			}
// 			throw new System.Exception("trying to get output from unset building");
// 		}
//
//
// 		private static Entity CreateRecipeEntity(BlobAssetReference<RecipeBlob> recipe, EntityManager entityManager)
// 		{
// 			// Create a new Recipe entity
// 			var recipeEntityArchetype = entityManager.CreateArchetype(
// 					typeof(RecipeData),
// 					typeof(RecipeState),
// 					typeof(RecipeInputs),
// 					typeof(RecipeOutput)
// 					);
//
// 			var itemEntityArchetype = entityManager.CreateArchetype(typeof(ItemComponent));
//
// 			var recipeEntity = entityManager.CreateEntity(recipeEntityArchetype);
//
// 			// Initialize with data
// 			entityManager.SetComponentData(recipeEntity, new RecipeData
// 			{
// 				recipeBlob = recipe
// 			});
//
// 			entityManager.SetComponentData(recipeEntity, new RecipeState
// 			{
// 				isProcessing = false,
// 				timeRemaining = 0f
// 			});
//
// 			entityManager.SetComponentData(recipeEntity, new RecipeOutput
// 			{
// 				itemName = recipe.Value.output.itemName.ToString(),
// 				amount = 0
// 			});
//
// 			DynamicBuffer<RecipeInputs> recipeInputs = entityManager.GetBuffer<RecipeInputs>(recipeEntity);
// 			recipeInputs.EnsureCapacity(recipe.Value.inputs.Length);
//
// 			for (int i = 0; i < recipe.Value.inputs.Length; i++)
// 			{
// 				var item = recipe.Value.inputs[i];
// 				recipeInputs.Add(new RecipeInputs
// 				{
// 					itemName = item.itemName.ToString(),
// 					amount = item.amount
// 				});
// 			}
//
// 			return recipeEntity;
// 		}
// 	}
// }
