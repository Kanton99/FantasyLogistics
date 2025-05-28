using UnityEngine;
using System.Collections.Generic;

namespace FantasyLogistics
{
	class ItemManager : MonoBehaviour
	{
		public static ItemManager Instance;

		private void Awake()
		{
			if (Instance && this != Instance)
			{
				DontDestroyOnLoad(this);
				return;
			}
			Instance = this;
		}

		[SerializeField]
		private ItemSO[] items;
		public Dictionary<string, ItemSO> itemMap = new Dictionary<string, ItemSO>();

		private void Start()
		{
			foreach (ItemSO item in items)
			{
				itemMap.TryAdd(item.itemName, item);
			}

		}
	}
}
