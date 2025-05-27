using UnityEngine;

namespace FantasyLogistics
{
	public class UIManager : MonoBehaviour
	{
		public UIManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance && Instance != Instance)
			{
				DontDestroyOnLoad(this);
				return;
			}
			Instance = this;
		}

	}
}
