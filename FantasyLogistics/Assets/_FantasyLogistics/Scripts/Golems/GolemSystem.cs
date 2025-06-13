using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Burst;

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

		Unity.Mathematics.Random randomGenerator;
		public void OnCreate(ref SystemState state)
		{
			buildingsQuery = new EntityQueryBuilder(Allocator.Persistent).WithAll<BuildingComponent, LocalTransform>().Build(ref state);
			recipeOutputLookup = state.GetComponentLookup<RecipeOutput>();
			buildingLookup = state.GetComponentLookup<BuildingComponent>(true);
			buildingTransforms = state.GetComponentLookup<LocalTransform>();

			randomGenerator = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
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
				outputUpdates = outputUpdates,
				randomGenerator = randomGenerator
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
		public Unity.Mathematics.Random randomGenerator;

		void Execute(Entity golemEntity, ref GolemMovement golem, ref GolemInvetory invetory, ref GolemInvetoryFilter filter, ref PhysicsVelocity golemVelocity, ref GolemTargets targets)
		{
			LocalTransform golemTransform = buildingTransforms[golemEntity];
			switch (golem.status)
			{
				case Status.STOP:
					{
						if (!filter.filteredItem.IsEmpty)
						{

							//GET PICKUP LOCATION
							var _this = this;
							FixedString32Bytes filteredItem = filter.filteredItem;
							NativeList<Entity> pickupBuildings = new NativeList<Entity>(buildings.Length / 2, Allocator.Temp);
							foreach (var building in buildings)
							{
								var buildingComponent = _this.buildingComponents[building];
								if (outputs.EntityExists(buildingComponent.recipeEntity))
								{
									var output = _this.outputs[buildingComponent.recipeEntity];
									if (output.itemName.Equals(filteredItem))
										pickupBuildings.Add(building);
								}
							}

							int size = pickupBuildings.Length;
							Entity targetPickup = buildings[randomGenerator.NextInt(size)];
							targets.pickupTargetBuilding = targetPickup;
							targets.pickupTarget = buildingTransforms[targetPickup].Position;


							golem.status = Status.MOVING;
						}
						break;
					}
				case Status.MOVING:
					{
						if (filter.filteredItem.IsEmpty)
						{
							golem.status = Status.STOP;
							break;
						}

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
						Entity recipe = buildingComponents[targets.pickupTargetBuilding].recipeEntity;
						var outputInvetory = outputs[recipe];
						int getAmount = math.min(16, outputInvetory.amount);
						if (outputUpdates.ContainsKey(recipe))
						{
							var recipeUpdatesIT = outputUpdates.GetValuesForKey(recipe);
							int sum = recipeUpdatesIT.Current;
							while (recipeUpdatesIT.MoveNext())
							{
								sum += recipeUpdatesIT.Current;
							}

							getAmount = math.min(getAmount, outputInvetory.amount + sum);
						}
						if (getAmount > 0)
						{
							outputUpdates.AsParallelWriter().Add(recipe, getAmount);
							invetory.amout += getAmount;
							invetory.itemName = filter.filteredItem;
						}

						golem.status = Status.MOVING;
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
