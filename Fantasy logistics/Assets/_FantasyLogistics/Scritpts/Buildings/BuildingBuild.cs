using Unity.Burst;
using Unity.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FantasyLogistics
{
	partial struct BuildingBuild : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{

		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			foreach (RefRW<Building> building in SystemAPI.Query<RefRW<Building>>())
			{
				if (!building.ValueRO.running)
				{
					ResourceAmount[] recInputs = building.ValueRO.recipe.input;
					ResourceAmount[] buildInput = building.ValueRO.inputInventory;

					if (HasAtLeast(buildInput, recInputs))
					{
						building.ValueRW.runningTime = 0;
						building.ValueRW.running = true;
					}
				}
				else
				{
					building.ValueRW.runningTime += SystemAPI.Time.DeltaTime;
					if (building.ValueRO.runningTime >= building.ValueRO.recipe.time)
					{
						//update inventory

						//Update output
					}
				}
			}
		}

		[BurstCompile]
		public void OnDestroy(ref SystemState state)
		{

		}

		private bool HasAtLeast(
			   IEnumerable<ResourceAmount> available,
			   IEnumerable<ResourceAmount> required)
		{
			// build lookup from Item → total amount available
			var availDict = available
				.GroupBy(r => r.resource)
				.ToDictionary(g => g.Key, g => g.Sum(r => r.amount));

			// verify each required resource
			foreach (var req in required)
			{
				if (!availDict.TryGetValue(req.resource, out var haveAmt)
					|| haveAmt < req.amount)
				{
					return false;
				}
			}
			return true;
		}
	}
}
