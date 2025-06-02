using UnityEngine;
using Unity.Entities;

namespace FantasyLogistics
{
	public class UIManager : MonoBehaviour
	{
		public static UIManager Instance { get; private set; }

		private void Awake()
		{
			if (Instance && Instance != Instance)
			{
				DontDestroyOnLoad(this);
				return;
			}
			Instance = this;
		}

		[Header("UI Panels")]
		public GameObject GolemUI;
		public GameObject BuildingUI;
		public GameObject PlayerInventory;

		public void CloseAll()
		{
			GolemUI.SetActive(false);
			PlayerInventory.SetActive(false);
			BuildingUI.SetActive(false);
		}

		public void OpenBuildingUI(Entity building)
		{
			GolemUI.SetActive(false);
			PlayerInventory.SetActive(false);
			BuildingUI.SetActive(true);
			BuildingUI.GetComponentInChildren<RecipeStateUI>().SetupUI(building);
		}
	}
}
