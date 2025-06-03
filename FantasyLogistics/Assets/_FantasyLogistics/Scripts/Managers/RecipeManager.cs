using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace FantasyLogistics
{
	public class RecipeManager : MonoBehaviour
	{
		public static RecipeManager Instance;
		private static BlobAssetStore blobAssetStore;

		private void Awake()
		{
			if (Instance && this != Instance)
			{
				DontDestroyOnLoad(this);
				return;
			}
			Instance = this;
			blobAssetStore = new BlobAssetStore(recipes.Length);

		}

		private void Start()
		{
			foreach (RecipeSO recipe in recipes)
			{
				recipeMap.TryAdd(recipe.recipeName, recipe);
			}
		}

		public BlobAssetReference<RecipeBlob> GetRecipeBlobAsset(RecipeSO recipe)
		{
			var hash = new Unity.Entities.Hash128((uint)recipe.GetHashCode(), 0, 0, 0);

			if (blobAssetStore.TryGet(hash, out BlobAssetReference<RecipeBlob> exisitngBlob)) return exisitngBlob;

			var newBlob = recipe.CreateRecipeData();
			blobAssetStore.TryAdd(hash, ref newBlob);
			return newBlob;
		}
		private void Dispose()
		{
			blobAssetStore.Dispose();
		}

		[SerializeField]
		private RecipeSO[] recipes;
		public Dictionary<string, RecipeSO> recipeMap = new Dictionary<string, RecipeSO>();
	}
}
