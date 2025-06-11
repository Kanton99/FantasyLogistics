using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

namespace FantasyLogistics
{

	public struct GolemInvetory : IComponentData
	{
		public FixedString32Bytes itemName;
		public int amout;
	}

	public struct GolemMovement : IComponentData
	{
		public float3 target;
		public float speed;
		public Status status;
	}

	public struct GolemInvetoryFilter : IComponentData
	{
		public FixedString32Bytes filteredItem;
	}

	public enum Status
	{
		MOVING,
		PICKINGUP,
		PLACING,
		STOP,
	}
}

