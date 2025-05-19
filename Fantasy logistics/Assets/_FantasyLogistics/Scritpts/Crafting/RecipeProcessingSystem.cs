using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace FantasyLogistics
{
	[BurstCompile]
	public partial struct RecipeProcessingSystem : ISystem
	{

		public void OnUpdate(ref SystemState state)
		{
			var deltaTime = SystemAPI.Time.DeltaTime;
			var outputLookup = SystemAPI.GetComponentLookup<ItemComponent>(isReadOnly: false);

			foreach (var (recipeData, recipeState, recipeInputs, recipeOutput) in SystemAPI.Query<RefRO<RecipeData>, RefRW<RecipeState>, RefRO<RecipeInputs>, RefRW<RecipeOutput>>())
			{
				var recipeBlob = recipeData.ValueRO.recipeBlob;

				if (recipeState.ValueRO.isProcessing)
				{
					recipeState.ValueRW.timeRemaining -= deltaTime;
					if (recipeState.ValueRO.timeRemaining <= 0)
					{
						recipeState.ValueRW.isProcessing = false;
						recipeState.ValueRW.timeRemaining = 0;

						if (outputLookup.HasComponent(recipeOutput.ValueRO.output))
						{
							var outputItem = outputLookup[recipeOutput.ValueRO.output];
							outputItem.amount++;
							outputLookup[recipeOutput.ValueRO.output] = outputItem;
							Debug.Log($"Finished a recipe, {outputItem.itemName}:{outputItem.amount}");
						}
						// outputItem.amount++;
					}
				}
				else
				{
					recipeState.ValueRW.isProcessing = true;
					recipeState.ValueRW.timeRemaining = recipeBlob.Value.craftingTime;
					//TODO consume inputs

				}
			}
		}
	}
}
