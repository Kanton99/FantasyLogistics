using UnityEngine;

namespace FantasyLogistics
{
	class InvetorySlot : MonoBehaviour
	{
		public ItemSO item;
		public int amount;

		private SpriteRenderer sprite;

		private void Start()
		{
			sprite.GetComponent<SpriteRenderer>();
		}

		public void UpdateItem(ItemSO item, int changeAmout)
		{
			if (item != this.item) return;

			Mathf.Clamp(this.amount + changeAmout, 0, int.MaxValue);
		}
	}
}
