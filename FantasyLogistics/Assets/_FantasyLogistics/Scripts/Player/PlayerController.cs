using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics;

namespace FantasyLogistics
{
	public class PlayerController : MonoBehaviour, PlayerActions_Actions.IPlayerActions
	{
		public float moveSpeed;

		public Camera mainCam;

		private Vector2 direction;
		private Rigidbody rb;
		private bool UIOpen = false;


		private PlayerActions_Actions m_Actions;                  // Source code representation of asset.
		private PlayerActions_Actions.PlayerActions m_Player;     // Source code representation of action map.
		void Awake()
		{
			m_Actions = new PlayerActions_Actions();              // Create asset object.
			m_Player = m_Actions.Player;                      // Extract action map object.
			m_Player.AddCallbacks(this);                      // Register callback interface IPlayerActions.
		}
		void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		void Update()
		{
			rb.linearVelocity = direction * moveSpeed * Time.deltaTime;
		}

		void OnDestroy()
		{
			m_Actions.Dispose();                              // Destroy asset object.
		}

		void OnEnable()
		{
			m_Player.Enable();                                // Enable all actions within map.
		}

		void OnDisable()
		{
			m_Player.Disable();                               // Disable all actions within map.
		}

		// Invoked when "Move" action is either started, performed or canceled.
		public void OnMove(InputAction.CallbackContext context)
		{
			direction = context.ReadValue<Vector2>();
			Debug.Log($"Moving {direction}");
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			if (!UIOpen && context.action.triggered && context.ReadValueAsButton())
			{
				var mousePosition = Mouse.current.position.ReadValue();
				var worldPos = mainCam.ScreenToWorldPoint((Vector3)mousePosition);
				worldPos.z = 0;

				Vector3 from = mainCam.transform.position;
				Vector3 direction = (worldPos - mainCam.transform.position) * 1.5f;

				rays.Add(new Vector3[] { from, direction });
				var hit = Utils.ECSPhysics.RayCast(from, direction);
				if (hit.HasValue)
				{
					Entity entity = hit.Value;
					EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
					if (entityManager.HasComponent<BuildingComponent>(entity))
					{
						// BuildingComponent building = entityManager.GetComponentData<BuildingComponent>(entity);
						UIManager.Instance.OpenBuildingUI(entity);
						UIOpen = true;
					}
					else if (entityManager.HasComponent<GolemInvetory>(entity))
					{
						UIManager.Instance.OpenGolemUI(entity);
						UIOpen = true;
					}
				}
			}
		}

		private List<Vector3[]> rays = new List<Vector3[]>();

		private void OnDrawGizmos()
		{
			float intensity = 1;
			foreach (Vector3[] pairs in rays)
			{
				Gizmos.color = Color.white * (intensity / rays.Count);
				Gizmos.DrawRay(pairs[0], pairs[1]);
				intensity++;
			}

		}

		public void OnToggleInventory(InputAction.CallbackContext context)
		{
			if (UIOpen)
			{
				UIManager.Instance.CloseAll();
				UIOpen = false;
			}
			// else:
			// 	UIManager.Instance.OpenPlayerINvetory()
		}
	}
}
