using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using TMPro;

namespace FantasyLogistics
{
	public class ItemList : MonoBehaviour
	{
		public ItemSO[] items;
		public GameObject itemUIPrefab;
		public Transform itemList;
		public GolemUI golemUI;

		public Entity activeGolem;

		private void Start()
		{
			foreach (var item in items)
			{
				GameObject itemement = Instantiate(itemUIPrefab, itemList);
				itemement.transform.position = Vector3.zero;
				var image = itemement.GetComponentInChildren<Image>();
				image.sprite = item.itemSprite;

				var name = itemement.GetComponentInChildren<TextMeshProUGUI>();
				name.text = item.itemName;

				var button = itemement.GetComponentInChildren<Button>();
				button.gameObject.SetActive(true);

				button.onClick.AddListener(delegate ()
				{
					golemUI.SetGolemFilter(item);
				});
			}
		}
	}
}
