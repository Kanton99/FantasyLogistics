using System.Collections.Generic;
using UnityEngine;

namespace FantasyLogistics
{
	public class RecipeManager : MonoBehaviour
	{
		public static RecipeManager Instance;

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
		private RecipeSO[] recipes;
		public Dictionary<string, RecipeSO> recipeMap = new Dictionary<string, RecipeSO>();
		public Dictionary<string, RecipeBlob> recipeBlobs = new Dictionary<string, RecipeBlob>();

		private void Start()
		{
			foreach (RecipeSO recipe in recipes)
			{
				recipeMap.TryAdd(recipe.recipeName, recipe);
				recipeBlobs.TryAdd(recipe.recipeName, recipe.CreateRecipeData());
			}

		}
	}
}
