using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;

namespace FantasyLogistics
{
	public class GolemUI : MonoBehaviour
	{
		public GameObject inventorySlot;
		public GameObject filterSlot;

		private Entity golem;
		private EntityManager entityManager;

		private void Awake()
		{
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		}

		public void SetGolem(Entity golem)
		{
			this.golem = golem;
			var golemInvetory = entityManager.GetComponentData<GolemInvetory>(golem);
			Image golemInvetoryImage = inventorySlot.GetComponent<Image>();
			if (!golemInvetory.itemName.IsEmpty)
			{
				golemInvetoryImage.sprite = ItemManager.Instance.itemMap[golemInvetory.itemName.ToString()].itemSprite;
				inventorySlot.GetComponentInChildren<TextMeshProUGUI>().text = golemInvetory.amout.ToString();
			}

			var golemFilter = entityManager.GetComponentData<GolemInvetoryFilter>(golem);
			if (!golemFilter.filteredItem.IsEmpty)
				filterSlot.GetComponent<Image>().sprite = ItemManager.Instance.itemMap[golemFilter.filteredItem.ToString()].itemSprite;
		}

		public void SetGolemFilter(ItemSO newFilter)
		{
			var golemFilter = entityManager.GetComponentData<GolemInvetoryFilter>(golem);
			golemFilter.filteredItem = newFilter.itemName;
			entityManager.SetComponentData<GolemInvetoryFilter>(golem, golemFilter);

			var golemInvetory = entityManager.GetComponentData<GolemInvetory>(golem);
			if (!golemInvetory.itemName.IsEmpty)
			{
				golemInvetory.amout = 0;
				entityManager.SetComponentData<GolemInvetory>(golem, golemInvetory);
			}

			SetGolem(golem);
		}
	}
}
