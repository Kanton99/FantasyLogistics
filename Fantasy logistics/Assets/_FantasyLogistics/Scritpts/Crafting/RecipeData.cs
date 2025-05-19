using Unity.Entities;
using Unity.Collections;

public struct RecipeData : IComponentData
{
	public BlobAssetReference<RecipeBlob> recipeBlob;
}

public struct RecipeState : IComponentData
{
	public float timeRemaining;
	public bool isProcessing;
}

public struct RecipeInputs : IComponentData
{
	public FixedList32Bytes<Entity> Value;
}

public struct RecipeOutput : IComponentData
{
	public Entity output;
}

public struct ItemComponent : IComponentData
{
	public FixedString64Bytes itemName;
	public int amount;
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
