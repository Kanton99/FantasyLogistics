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
			var inputLookup = SystemAPI.GetComponentLookup<ItemComponent>(isReadOnly: false);

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
					//TODO consume inputs
					bool canStart = true;
					int count = 0;
					foreach (var item in recipeInputs.ValueRO.Value)
					{
						var inputItem = inputLookup[item];
						if (inputItem.amount < recipeBlob.Value.inputs[count].amount)
							canStart = false;
						count++;
					}

					if (canStart)
					{
						count = 0;
						foreach (var item in recipeInputs.ValueRO.Value)
						{
							var inputItem = inputLookup[item];
							inputItem.amount -= recipeBlob.Value.inputs[count].amount;
							inputLookup[item] = inputItem;
							count++;
						}
						recipeState.ValueRW.isProcessing = true;
						recipeState.ValueRW.timeRemaining = recipeBlob.Value.craftingTime;
					}
					// else Debug.Log("Can't start recipe, not enough resources");
				}
			}
		}
	}
}
