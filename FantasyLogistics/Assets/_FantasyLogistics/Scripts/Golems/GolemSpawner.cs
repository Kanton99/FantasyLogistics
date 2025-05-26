using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
namespace FantasyLogistics
{
	public struct GolemInstantiator : IComponentData
	{
		public Entity prefab;
		public float3 position;
		public bool place;

	}

	class GolemInstantiatorBaker : Baker<GolemInstantiatorAuthoring>
	{
		public override void Bake(GolemInstantiatorAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new GolemInstantiator
			{
				prefab = GetEntity(authoring.golemEntityPrefab, TransformUsageFlags.Dynamic),
				position = authoring.position,
				place = authoring.place
			});
		}
	}

	[BurstCompile]
	public partial struct GolemInstantiateSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			foreach (var instantiator in SystemAPI.Query<RefRW<GolemInstantiator>>())
			{
				if (instantiator.ValueRO.place)
				{
					//TODO Use EntityCommandBuffer
					var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
					EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

					Entity newGolem = ecb.Instantiate(instantiator.ValueRO.prefab);
					ecb.AddComponent<GolemMovement>(newGolem);
					ecb.SetComponent(newGolem, LocalTransform.FromPosition(instantiator.ValueRO.position));
					ecb.SetComponent(newGolem, new GolemMovement
					{
						target = float3.zero,
						speed = 0.1f,
						status = Status.MOVING
					});


					instantiator.ValueRW.place = false;
				}
			}
		}
	}
}
