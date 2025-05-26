using UnityEngine;
using UnityEngine.InputSystem;

namespace FantasyLogistics
{
	public class PlayerController : MonoBehaviour, PlayerActions_Actions.IPlayerActions
	{
		public float moveSpeed;

		private Vector2 direction;
		private Rigidbody rb;


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
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			Debug.Log("Interacted");
		}

	}
}
