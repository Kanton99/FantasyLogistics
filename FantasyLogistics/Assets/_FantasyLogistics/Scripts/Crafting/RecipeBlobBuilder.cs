using UnityEngine;
using Unity.Entities;

namespace FantasyLogistics
{
	public class RecipeBlobBuilder : MonoBehaviour
	{
		public RecipeSO[] recipes;
	}

	public class RecipeBaker : Baker<RecipeBlobBuilder>
	{
		public override void Bake(RecipeBlobBuilder authoring)
		{
			foreach (var recipes in authoring.recipes)
			{

			}
		}
	}
}
