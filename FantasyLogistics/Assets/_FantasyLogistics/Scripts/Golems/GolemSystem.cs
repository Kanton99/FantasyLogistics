using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

namespace FantasyLogistics
{
	public partial struct GolemMovementSystem : ISystem
	{
		public void OnCreate(ref SystemState state)
		{
			var lookupRecipeOutput = SystemAPI.GetComponentLookup<RecipeOutput>();
		}
		public void OnUpdate(ref SystemState state)
		{
			foreach (var (golem, transform, velocity) in SystemAPI.Query<RefRW<GolemMovement>, RefRW<LocalTransform>, RefRW<PhysicsVelocity>>())
			{
				var target = golem.ValueRO.target;
				float3 direction = target - transform.ValueRO.Position;
				if (golem.ValueRO.status == Status.MOVING && math.length(direction) > 0f)
				{
					float3 normDir = math.normalize(direction);
					velocity.ValueRW.Linear = normDir * golem.ValueRO.speed * SystemAPI.Time.DeltaTime;
				}
			}
		}

	}

	partial struct GolemJob : IJobEntity
	{
		public EntityQuery recipeOutputs;

		void Execute(ref GolemMovement golem, ref GolemInvetory invetory, ref GolemInvetoryFilter filter, ref LocalTransform golemTransform, ref PhysicsVelocity golemVelocity)
		{
			if (!filter.filteredItem.IsEmpty)
			{
				switch (golem.status)
				{
					case Status.STOP:
						{
							//TODO Search for next target
							if (!filter.filteredItem.IsEmpty)
								golem.status = invetory.amout == 0 ? Status.PICKINGUP : Status.PLACING;
							break;
						}
					case Status.MOVING:
						{
							//TODO During moving logic
							break;
						}
					case Status.PICKINGUP:
						{
							foreach (var output in recipeOutputs.ToComponentDataArray(Allocator.Temp))
							{

							}
							break;
						}
					case Status.PLACING:
						{
							//TODO Transporting items
							break;
						}
				}
			}
		}
	}
}
