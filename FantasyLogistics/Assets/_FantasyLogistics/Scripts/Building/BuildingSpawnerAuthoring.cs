using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FantasyLogistics
{
	public class BuildingSpawnerBaker : Baker<BuildingSpawner>
	{
		public override void Bake(BuildingSpawner authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new BuildingSpawnerComponent
			{
				buildingPrefab = GetEntity(authoring.buildingPrefab, TransformUsageFlags.Renderable),
				position = authoring.position,
				spawn = authoring.spawn
			});
		}
	}
	public struct BuildingSpawnerComponent : IComponentData
	{
		public Entity buildingPrefab;
		public float3 position;
		public bool spawn;
	}

	[BurstCompile]
	public partial struct BuildingSystemSpawner : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var buildingSpawner = SystemAPI.GetSingleton<BuildingSpawnerComponent>();

			if (buildingSpawner.spawn)
			{
				var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
				EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

				Entity newBuilding = ecb.Instantiate(buildingSpawner.buildingPrefab);
				ecb.SetComponent(newBuilding, LocalTransform.FromPosition(buildingSpawner.position));
				ecb.AddComponent(newBuilding, new BuildingComponent());


				buildingSpawner.spawn = false;
			}
		}


	}
}
