using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace FantasyLogistics
{
	public struct RecipeData : IComponentData
	{
		public BlobAssetReference<RecipeBlob> recipeBlob;
	}

	public struct RecipeState : IComponentData
	{
		public float timeRemaining;
		public bool isProcessing;
		public float2 position;
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
		public FixedString32Bytes name;
	}

	public struct ItemBlob
	{
		public BlobString itemName;
		public int amount;
	}
}
