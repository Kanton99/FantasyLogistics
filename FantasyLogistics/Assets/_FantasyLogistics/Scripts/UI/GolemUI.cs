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
		private EntityManager entotyManager;

		public void SetGolem(Entity golem)
		{
			this.golem = golem;
			var golemInvetory = entotyManager.GetComponentData<GolemInvetory>(golem);
			Image golemInvetoryImage = inventorySlot.GetComponent<Image>();
			golemInvetoryImage.sprite = ItemManager.Instance.itemMap[golemInvetory.itemName.ToString()].itemSprite;
			inventorySlot.GetComponentInChildren<TextMeshProUGUI>().text = golemInvetory.amout.ToString();

			var golemFilter = entotyManager.GetComponentData<GolemInvetoryFilter>(golem);
			filterSlot.GetComponent<Image>().sprite = ItemManager.Instance.itemMap[golemFilter.filteredItem.ToString()].itemSprite;
		}
	}
}
