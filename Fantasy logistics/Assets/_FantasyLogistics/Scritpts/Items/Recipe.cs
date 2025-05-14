using UnityEngine;

namespace FantasyLogistics
{
	[CreateAssetMenu(fileName = "Recipe", menuName = "Items/Recipe")]
	public class Recipe : ScriptableObject
	{
		public ResourceAmount[] input;
		public ResourceAmount[] output;
		public float time;
		public float manaCost;

		[SerializeField]
		Sprite customSprite;

		public Sprite getSprite()
		{
			return customSprite ? customSprite : output[0].resource.sprite;
		}
	}
}
