using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Burst;
using System;

namespace FantasyLogistics
{
	public partial struct GolemMovementSystem : ISystem
	{
		EntityQuery buildingsQuery;
		EntityManager entityManager;
		ComponentLookup<RecipeOutput> recipeOutputLookup;
		ComponentLookup<BuildingComponent> buildingLookup;
		ComponentLookup<LocalTransform> buildingTransforms;
		NativeParallelMultiHashMap<Entity, int> outputUpdates;
		public void OnCreate(ref SystemState state)
		{
			buildingsQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<BuildingComponent, LocalTransform>().Build(ref state);
			recipeOutputLookup = state.GetComponentLookup<RecipeOutput>(true);
			buildingLookup = state.GetComponentLookup<BuildingComponent>(true);
			buildingTransforms = state.GetComponentLookup<LocalTransform>();

		}
		public void OnUpdate(ref SystemState state)
		{
			recipeOutputLookup.Update(ref state);
			buildingLookup.Update(ref state);
			buildingTransforms.Update(ref state);

			if (outputUpdates.IsCreated && outputUpdates.Capacity < buildingsQuery.CalculateEntityCount())
				outputUpdates.Dispose();

			outputUpdates = new NativeParallelMultiHashMap<Entity, int>(buildingsQuery.CalculateEntityCount(), Allocator.Persistent);
			var jobHandles = new GolemJob
			{
				deltaTime = SystemAPI.Time.DeltaTime,
				buildings = buildingsQuery.ToEntityArray(Allocator.TempJob),
				outputs = recipeOutputLookup,
				buildingComponents = buildingLookup,
				buildingTransforms = buildingTransforms,
				outputUpdates = outputUpdates
			}.ScheduleParallel(state.Dependency);

			jobHandles.Complete();

			var keyValues = outputUpdates.GetKeyValueArrays(Allocator.Temp);
			for (int i = 0; i < keyValues.Length; i++)
			{
				var key = keyValues.Keys[i];
				var value = keyValues.Values[i];
				var output = state.EntityManager.GetComponentData<RecipeOutput>(key);
				output.amount += value;
				state.EntityManager.SetComponentData(key, output);
			}
		}

	}

	[BurstCompile]
	partial struct GolemJob : IJobEntity
	{
		public NativeArray<Entity> buildings;
		public float deltaTime;
		[ReadOnly]
		public ComponentLookup<RecipeOutput> outputs;
		[ReadOnly]
		public ComponentLookup<BuildingComponent> buildingComponents;
		[NativeDisableParallelForRestriction]
		public ComponentLookup<LocalTransform> buildingTransforms;
		[NativeDisableParallelForRestriction]
		public NativeParallelMultiHashMap<Entity, int> outputUpdates;

		void Execute(Entity golemEntity, ref GolemMovement golem, ref GolemInvetory invetory, ref GolemInvetoryFilter filter, ref PhysicsVelocity golemVelocity, ref GolemTargets targets)
		{
			LocalTransform golemTransform = buildingTransforms[golemEntity];
			switch (golem.status)
			{
				case Status.STOP:
					{
						break;
					}
				case Status.MOVING:
					{
						float3 target = invetory.amout > 0 ? targets.dropTarget : targets.pickupTarget;
						float3 direction = target - buildingTransforms[golemEntity].Position;
						float distance = math.length(direction);

						if (distance > 0.1f)
						{
							golemVelocity.Linear = math.normalize(direction) * golem.speed * deltaTime;
						}
						else
						{
							golemVelocity.Linear = float3.zero;
							golem.status = invetory.amout > 0 ? Status.PLACING : Status.PICKINGUP;
						}
						break;
					}
				case Status.PLACING:
					{
						break;
					}
				case Status.PICKINGUP:
					{
						break;
					}

			}
			buildingTransforms[golemEntity] = golemTransform;
		}
	}
}
