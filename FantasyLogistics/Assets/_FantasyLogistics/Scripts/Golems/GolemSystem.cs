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
		EntityQuery recipeOutputsQuery;
		public void OnCreate(ref SystemState state)
		{
			recipeOutputsQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<BuildingComponent, LocalTransform>().Build(ref state);
		}
		public void OnUpdate(ref SystemState state)
		{
			new GolemJob
			{
				deltaTime = SystemAPI.Time.DeltaTime,
				recipeOutputs = recipeOutputsQuery.ToEntityArray(Allocator.TempJob)
			}.ScheduleParallel();
		}

	}

	// [BurstCompile]
	partial struct GolemJob : IJobEntity
	{
		public NativeArray<Entity> recipeOutputs;
		public float deltaTime;

		void Execute(ref GolemMovement golem, ref GolemInvetory invetory, ref GolemInvetoryFilter filter, ref LocalTransform golemTransform, ref PhysicsVelocity golemVelocity)
		{
			switch (golem.status)
			{
				case Status.STOP:
					{
						//TODO Search for next target
						if (!filter.filteredItem.IsEmpty)
						{
							if (math.length(golem.target - golemTransform.Position) < 0.1f)
							{
								if (invetory.amout > 0)
								{
									//TODO PLACE ITEM
									break;
								}
								else
								{
									//TODO Pickup item

									break;
								}
							}
							else
								golem.status = Status.STOP;
						}
						break;
					}
				case Status.MOVING:
					{
						float3 direction = golem.target - golemTransform.Position;
						if (math.length(direction) > 0f)
						{
							float3 normDir = math.normalize(direction);
							golemVelocity.Linear = normDir * golem.speed * deltaTime;
						}
						else
							golem.status = Status.STOP;
						break;
					}
				case Status.PICKINGUP:
					{
						Entity[] buildings = recipeOutputs.ToArray();
						string filteredItem = filter.filteredItem.ToString();
						buildings = Array.FindAll(buildings, building =>
								{
									var entMan = World.DefaultGameObjectInjectionWorld.EntityManager;
									var recipe = entMan.GetComponentData<BuildingComponent>(building).recipeEntity;
									var recipeOutput = entMan.GetComponentData<RecipeOutput>(recipe).itemName;
									return recipeOutput == filteredItem;
								});

						if (buildings.Length > 0)
						{
							golem.target = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(buildings[0]).Position;
							golem.status = Status.MOVING;
						}
						else
							golem.status = Status.STOP;
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
