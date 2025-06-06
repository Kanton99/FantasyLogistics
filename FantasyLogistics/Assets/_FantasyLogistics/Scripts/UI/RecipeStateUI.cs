using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Collections;

namespace FantasyLogistics
{
	public class RecipeStateUI : MonoBehaviour
	{
		public TMPro.TextMeshProUGUI recipeName;
		public Image recipeImage;
		public Slider progress;

		public GameObject inputsSlotsParent;
		public GameObject inputSlotPrefab;
		public GameObject outputSlot;

		public Entity building { get; private set; }
		private Entity recipeEntity;
		private EntityManager entityManager;

		private TextMeshProUGUI outputAmount;
		private List<TextMeshProUGUI> inputsAmounts = new List<TextMeshProUGUI>();

		public void SetupUI(Entity building)
		{
			StartCoroutine(SetupUICoroutine(building));
		}
		public IEnumerator SetupUICoroutine(Entity building)
		{
			yield return null;
			ClearUI();
			this.building = building;
			var buildingComponent = entityManager.GetComponentData<BuildingComponent>(building);
			transform.parent.GetComponentInChildren<RecipeList>().activeBuilding = building;
			if (entityManager.Exists(entityManager.GetComponentData<BuildingComponent>(building).recipeEntity))
			{
				recipeEntity = entityManager.GetComponentData<BuildingComponent>(building).recipeEntity;
				recipeName.text = entityManager.GetComponentData<RecipeData>(entityManager.GetComponentData<BuildingComponent>(building).recipeEntity).recipeBlob.Value.name.ToString();
				recipeImage.sprite = RecipeManager.Instance.recipeMap[recipeName.text].GetSprite();


				var inputs = entityManager.GetBuffer<RecipeInputs>(recipeEntity);
				var output = entityManager.GetComponentData<RecipeOutput>(recipeEntity);

				foreach (Transform child in inputsSlotsParent.transform)
				{
					Destroy(child.gameObject);
				}
				for (int i = 0; i < inputs.Length; i++)
				{
					var newSlot = Instantiate(inputSlotPrefab, inputsSlotsParent.transform);
					newSlot.GetComponent<Image>().sprite = ItemManager.Instance.itemMap[inputs[i].itemName.ToString()].itemSprite;
					inputsAmounts.Add(newSlot.GetComponentInChildren<TextMeshProUGUI>());
				}
				outputSlot.GetComponent<Image>().sprite = ItemManager.Instance.itemMap[output.itemName.ToString()].itemSprite;
			}
		}



		public void SetBuildingRecipe(RecipeSO recipe)
		{
			BuildingComponent newBC = new BuildingComponent();
			if (entityManager.Exists(entityManager.GetComponentData<BuildingComponent>(building).recipeEntity))
				entityManager.DestroyEntity(entityManager.GetComponentData<BuildingComponent>(building).recipeEntity);

			newBC.recipeEntity = CreateRecipeEntity(recipe);

			entityManager.SetComponentData(building, newBC);
			SetupUI(building);
		}
		private void Awake()
		{
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		}

		private void Update()
		{
			if (entityManager.Exists(recipeEntity))
			{
				progress.value = (entityManager.GetComponentData<RecipeState>(recipeEntity).timeRemaining) / 
					entityManager.GetComponentData<RecipeData>(recipeEntity).recipeBlob.Value.craftingTime;

				if (progress.value < 0.1f)
				{
					var inputs = entityManager.GetBuffer<RecipeInputs>(recipeEntity);
					var output = entityManager.GetComponentData<RecipeOutput>(recipeEntity);
					{
						for (int i = 0; i < inputs.Length; i++)
						{
							Transform slot = inputsSlotsParent.transform.GetChild(i);
							var item = inputs[i];
							slot.GetComponentInChildren<TextMeshProUGUI>().text = item.amount.ToString();
						}
						outputSlot.GetComponentInChildren<TextMeshProUGUI>().text = output.amount.ToString();
					}
				}
			}
		}

		private void ClearUI()
		{
			recipeEntity = Entity.Null;
			recipeImage.sprite = null;
			recipeName.text = null;
			for(int i = 0;i<inputsSlotsParent.transform.childCount;i++)
			{
				Transform child = inputsSlotsParent.transform.GetChild(i);
				GameObject.Destroy(child.gameObject);
			}
			inputsAmounts.Clear();
			//GameObject.Destroy(outputSlot.transform.GetChild(0).gameObject);
			outputSlot.GetComponentInChildren<Image>().sprite = null;
			if(outputAmount) outputAmount.text = null;

		}

		private Entity CreateRecipeEntity(RecipeSO recipe)
		{
			// Create a new Recipe entity
			var recipeEntityArchetype = entityManager.CreateArchetype(
				typeof(RecipeData),
				typeof(RecipeState),
				typeof(RecipeInputs),
				typeof(RecipeOutput)
			);

			var itemEntityArchetype = entityManager.CreateArchetype(typeof(ItemComponent));

			var recipeEntity = entityManager.CreateEntity(recipeEntityArchetype);

			// Initialize with data
			entityManager.SetComponentData(recipeEntity, new RecipeData
			{
				recipeBlob = RecipeManager.Instance.GetRecipeBlobAsset(recipe),
			});

			entityManager.SetComponentData(recipeEntity, new RecipeState
			{
				isProcessing = false,
				timeRemaining = 0f,
				position = new float2(gameObject.transform.position.x, gameObject.transform.position.y)
			});

			entityManager.SetComponentData(recipeEntity, new RecipeOutput
			{
				itemName = recipe.output.itemName,
				amount = 0
			});

			DynamicBuffer<RecipeInputs> recipeInputs = entityManager.GetBuffer<RecipeInputs>(recipeEntity);
			recipeInputs.EnsureCapacity(recipe.inputs.Length);

			foreach (var item in recipe.inputs)
			{
				recipeInputs.Add(new RecipeInputs
				{
					itemName = item.itemName,
					amount = 0
				});
			}

			return recipeEntity;
		}
	}
}
