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

[InternalBufferCapacity(4)]
public struct RecipeInputs : IBufferElementData
{
	public FixedString32Bytes itemName;
	public int amount;
}

public struct RecipeOutput : IComponentData
{
	public FixedString32Bytes itemName;
	public int amount;
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
