using Unity.Entities;

public struct RecipeData : IComponentData
{
	public BlobAssetReference<RecipeBlob> recipeBlob;
}

public struct RecipeState : IComponentData
{
	public float timeRemaining;
	public bool isProcessing;
}

public struct RecipeBlob
{
	public BlobArray<ItemBlob> inputs;
	public ItemBlob output;
	public float craftingTime;
}

public struct ItemBlob
{
	public BlobString itemName;
	public int amount;
}
