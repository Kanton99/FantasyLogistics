using UnityEngine;

namespace FantasyLogistics
{
	[CreateAssetMenu(fileName="Item", menuName="Items/Item")]
	public class Item : ScriptableObject
	{
		public Sprite sprite;
	}

	[System.Serializable]
	public struct ResourceAmount{
		public Item resource;
		public int amount;
	}
}
