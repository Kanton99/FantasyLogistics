using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FantasyLogistics
{
	public partial struct GolemMovementSystem : ISystem
	{
		public void OnUpdate(ref SystemState state)
		{
			foreach (var (golem, golemTransform) in SystemAPI.Query<RefRW<GolemMovement>, RefRW<LocalTransform>>())
			{
				var target = golem.ValueRO.target;
				float3 direction = target - golemTransform.ValueRO.Position;
				if (golem.ValueRO.status == Status.MOVING && math.length(direction) > 0.1f)
				{
					float3 normDir = math.normalize(direction);
					golemTransform.ValueRW.Position += normDir * golem.ValueRO.speed * SystemAPI.Time.DeltaTime;
				}
			}
		}

	}
}
