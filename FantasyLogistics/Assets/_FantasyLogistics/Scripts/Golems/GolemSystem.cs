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

	// [BurstCompile]
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

		void Execute(Entity golemEntity, ref GolemMovement golem, ref GolemInvetory invetory, ref GolemInvetoryFilter filter, ref PhysicsVelocity golemVelocity)
		{
			LocalTransform golemTransform = buildingTransforms[golemEntity];
			switch (golem.status)
			{
				case Status.STOP:
					{
						golemVelocity.Linear = float3.zero;
						//TODO Search for next target
						if (!filter.filteredItem.IsEmpty)
						{
							if (math.length(golem.target - golemTransform.Position) < 1f)
							{
								if (invetory.amout > 0)
								{
									//TODO PLACE ITEM
									break;
								}
								else
								{
									//TODO Pickup item
									var building = buildingComponents[golem.targetBuilding];
									var output = outputs[building.recipeEntity];
									int getAmount = 16;
									if (outputUpdates.CountValuesForKey(building.recipeEntity) > 0)
									{
										var changes = outputUpdates.GetValuesForKey(building.recipeEntity);
										var changesSum = changes.Current;
										while (changes.MoveNext())
										{
											changesSum += changes.Current;
										}
										getAmount = math.min(getAmount, output.amount + changesSum);
									}
									else
									{
										getAmount = math.min(16, output.amount);
									}
									if (getAmount > 0)
										outputUpdates.AsParallelWriter().Add(building.recipeEntity, -getAmount);
									invetory.itemName = filter.filteredItem;
									invetory.amout += getAmount;
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
						Entity[] buildingsArray = buildings.ToArray();
						string filteredItem = filter.filteredItem.ToString();
						var _this = this;
						buildingsArray = Array.FindAll(buildingsArray, building =>
								{
									var recipe = _this.buildingComponents[building].recipeEntity;
									if (_this.outputs.EntityExists(recipe))
									{
										var recipeOutput = _this.outputs[recipe].itemName;
										return recipeOutput == filteredItem;
									}
									return false;
								});

						if (buildings.Length > 0)
						{
							golem.target = buildingTransforms[buildingsArray[0]].Position;
							golem.targetBuilding = buildings[0];
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
			buildingTransforms[golemEntity] = golemTransform;
		}
	}
}
