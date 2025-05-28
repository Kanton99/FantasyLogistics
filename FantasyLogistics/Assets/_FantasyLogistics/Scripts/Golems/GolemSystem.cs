using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

namespace FantasyLogistics
{
	public partial struct GolemMovementSystem : ISystem
	{
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
}
