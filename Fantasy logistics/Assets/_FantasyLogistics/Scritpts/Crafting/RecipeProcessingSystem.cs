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

			foreach (var (recipeData, recipeState) in SystemAPI.Query<RefRO<RecipeData>, RefRW<RecipeState>>())
			{
				var recipeBlob = recipeData.ValueRO.recipeBlob;

				if (recipeState.ValueRO.isProcessing)
				{
					recipeState.ValueRW.timeRemaining -= deltaTime;
					if (recipeState.ValueRO.timeRemaining <= 0)
					{
						recipeState.ValueRW.isProcessing = false;
						recipeState.ValueRW.timeRemaining = 0;

						Debug.Log($"Finished a recipe");
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
