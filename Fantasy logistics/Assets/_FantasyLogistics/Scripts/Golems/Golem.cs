using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

namespace FantasyLogistics
{
	public class Golem : MonoBehaviour
	{
		public struct Invetory
		{
			public string itemName;
			public int amount;
		}
		public Invetory invetory;
		public ItemSO toMove;

		public float speed = 1f;
		private bool moveTo = false;
		private Vector3 targetPosition;
		private Status status = Status.MOVING;

		private Rigidbody2D rb;
		private Entity targetEntity;

		void Start()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		void Update()
		{
			switch (status)
			{
				case Status.MOVING:
					{
						if ((gameObject.transform.position - targetPosition).sqrMagnitude < 0.25)
						{
							rb.linearVelocity = Vector2.zero;
							status = moveTo ? Status.PLACING : Status.PICKINGUP;
						}
						else
						{
							rb.linearVelocity = (targetPosition - transform.position).normalized * speed * Time.deltaTime;
						}
						break;
					}

				case Status.PLACING:
					{
						EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
						DynamicBuffer<RecipeInputs> inputBuffer = entityManager.GetBuffer<RecipeInputs>(targetEntity);
						for (int i = 0; i < inputBuffer.Length; i++)
						{
							if (inputBuffer[i].itemName == invetory.itemName)
							{
								RecipeInputs item = inputBuffer[i];
								item.amount += invetory.amount;
								inputBuffer[i] = item;
								break;
							}
						}
						moveTo = false;
						status = Status.MOVING;
						targetPosition = FindFromPosition();
						break;
					}

				case Status.PICKINGUP:
					{
						EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
						RecipeOutput output = entityManager.GetComponentData<RecipeOutput>(targetEntity);
						int amountToGet = output.amount > 32 ? 32 : output.amount;
						output.amount -= amountToGet;
						entityManager.SetComponentData(targetEntity, output);
						invetory.amount += amountToGet;

						moveTo = true;
						status = Status.MOVING;
						targetPosition = FindToPosition();
						break;
					}
			}
		}

		Vector3 FindFromPosition()
		{
			EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			//TODO cache for performance
			EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<RecipeOutput>().Build(entityManager);
			NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
			List<Entity> entitiesWithOutputItems = new List<Entity>();

			foreach (var entity in entities)
			{
				var recipeOutputInv = entityManager.GetComponentData<RecipeOutput>(entity);
				if (recipeOutputInv.itemName == invetory.itemName && recipeOutputInv.amount > 0)
					entitiesWithOutputItems.Add(entity);
			}

			int idx = Random.Range(0, entitiesWithOutputItems.Count);

			targetEntity = entitiesWithOutputItems[idx];

			var position = entityManager.GetComponentData<RecipeState>(targetEntity).position;
			return new Vector3(position.x, position.y, 0);
		}

		Vector3 FindToPosition()
		{
			EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			//TODO cache for performance
			EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<RecipeInputs>().Build(entityManager);
			NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
			List<Entity> entitiesWithInputItems = new List<Entity>();
			foreach (var entity in entities)
			{
				var recipeOutputInv = entityManager.GetBuffer<RecipeInputs>(entity);
				for (int i = 0; i < recipeOutputInv.Length; i++)
				{
					var input = recipeOutputInv[i];
					if (input.itemName == invetory.itemName && input.amount < 32)
					{
						entitiesWithInputItems.Add(entity);
						break;
					}
				}
			}

			int idx = Random.Range(0, entitiesWithInputItems.Count);

			targetEntity = entitiesWithInputItems[idx];

			var position = entityManager.GetComponentData<RecipeState>(targetEntity).position;
			return new Vector3(position.x, position.y, 0);
		}

		enum Status
		{
			MOVING,
			PICKINGUP,
			PLACING,
			STOP,
		}
	}
}
