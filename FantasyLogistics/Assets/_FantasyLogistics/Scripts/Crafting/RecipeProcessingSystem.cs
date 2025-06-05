using Unity.Burst;
using Unity.Entities;

namespace FantasyLogistics
{
	[BurstCompile]
	public partial struct RecipeProcessingSystem : ISystem
	{

		public void OnUpdate(ref SystemState state)
		{
			var deltaTime = SystemAPI.Time.DeltaTime;

			foreach (var (recipeData, recipeState, recipeInputs, recipeOutput) in SystemAPI.Query<RefRO<RecipeData>, RefRW<RecipeState>, DynamicBuffer<RecipeInputs>, RefRW<RecipeOutput>>())
			{
				var recipeBlob = recipeData.ValueRO.recipeBlob;

				if (recipeState.ValueRO.isProcessing)
				{
					recipeState.ValueRW.timeRemaining += deltaTime;
					if (recipeState.ValueRO.timeRemaining >= recipeData.ValueRO.recipeBlob.Value.craftingTime)
					{
						recipeState.ValueRW.isProcessing = false;
						recipeState.ValueRW.timeRemaining = 0;

						recipeOutput.ValueRW.amount += recipeData.ValueRO.recipeBlob.Value.output.amount;
					}
				}
				else
				{
					bool canStart = true;
					for (int i = 0; i < recipeInputs.Length; i++)
					{
						if (recipeInputs[i].amount < recipeBlob.Value.inputs[i].amount)
						{
							canStart = false;
							break;
						}
					}

					if (canStart)
					{
						for (int i = 0; i < recipeInputs.Length; i++)
						{
							var input = recipeInputs[i];
							input.amount -= recipeBlob.Value.inputs[i].amount;
							recipeInputs.ElementAt(i) = input;
						}
						recipeState.ValueRW.isProcessing = true;
						// recipeState.ValueRW.timeRemaining = recipeBlob.Value.craftingTime;
					}
					// else Debug.Log("Can't start recipe, not enough resources");
				}
			}
		}
	}
}
