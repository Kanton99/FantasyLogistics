using UnityEngine;
using Unity.Entities;
using Unity.Collections;

namespace FantasyLogistics
{
	[CreateAssetMenu(fileName = "NewRecipe", menuName = "Item/Recipe")]
	public class RecipeSO : ScriptableObject
	{
		public string recipeName;
		public ItemSO[] inputs;
		public ItemSO output;
		public Sprite sprite;
		public float cratingTime = 1f;

		public ref RecipeBlob CreateRecipeData()
		{
			var builder = new BlobBuilder(Allocator.Temp);

			ref RecipeBlob recipeData = ref builder.ConstructRoot<RecipeBlob>();

			recipeData.craftingTime = cratingTime;
			recipeData.output = output.CreateItemData().Value;

			BlobBuilderArray<ItemBlob> arrayBuilder = builder.Allocate(
					ref recipeData.inputs,
					inputs.Length
					);

			for (int i = 0; i < inputs.Length; i++)
			{
				arrayBuilder[i] = inputs[i].CreateItemData().Value;
			}

			return ref recipeData;
		}

		public Sprite GetSprite()
		{
			if (sprite) return sprite;
			return output.itemSprite;
		}

	}

	[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item")]
	public class ItemSO : ScriptableObject
	{
		public string itemName;
		public Sprite itemSprite;
		public int amout = 1;

		public BlobAssetReference<ItemBlob> CreateItemData()
		{
			var builder = new BlobBuilder(Allocator.Temp);

			ref ItemBlob itemData = ref builder.ConstructRoot<ItemBlob>();

			builder.AllocateString(ref itemData.itemName, itemName);
			itemData.amount = amout;


			var result = builder.CreateBlobAssetReference<ItemBlob>(Allocator.Persistent);
			builder.Dispose();
			return result;
		}
	}
}
