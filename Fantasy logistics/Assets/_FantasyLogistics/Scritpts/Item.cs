using UnityEngine;

namespace FantasyLogistics
{
	[CreateAssetMenu(fileName="Item", menuName="Items/Item")]
	class Item : ScriptableObject
	{
		public Sprite sprite;
	}

	[System.Serializable]
	struct ResourceAmount{
		public Item resource;
		public int amount;
	}
}
