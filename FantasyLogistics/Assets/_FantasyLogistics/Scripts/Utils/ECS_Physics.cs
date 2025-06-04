using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FantasyLogistics.Utils
{
	class ECSPhysics
	{
		public static Entity? RayCast(float3 from, float3 direction)
		{
			EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();

			EntityQuery singletonQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(builder);
			var collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

			singletonQuery.Dispose();

			RaycastInput input = new RaycastInput()
			{
				Start = from,
				End = from + direction,
				Filter = new CollisionFilter()
				{
					BelongsTo = ~0u,
					CollidesWith = 1 << 6,
					GroupIndex = 0
				}
			};

			Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();
			bool haveHit = collisionWorld.CastRay(input, out hit);

			if (haveHit)
			{
				return hit.Entity;
			}
			return null;
		}
	}
}
