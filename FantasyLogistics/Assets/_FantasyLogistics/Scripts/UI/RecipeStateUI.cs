using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using TMPro;

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

		public Building building { get; private set; }
		private EntityManager entityManager;

		public void SetupUI(Building building)
		{
			this.building = building;
			transform.parent.GetComponentInChildren<RecipeList>().activeBuilding = building;
			if (building.recipe)
			{
				recipeName.text = this.building.recipe.recipeName;
				recipeImage.sprite = this.building.recipe.GetSprite();
			}
		}

		public void SetBuildingRecipe(RecipeSO recipe)
		{
			building.SetRecipe(recipe);
		}

		private void Start()
		{
			entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		}

		private void Update()
		{
			var recipeState = entityManager.GetComponentData<RecipeState>(building.recipeEntity);
			progress.value = (building.recipe.cratingTime - recipeState.timeRemaining) / building.recipe.cratingTime;

			if (progress.value < 0.1f)
			{
				var inputs = entityManager.GetBuffer<RecipeInputs>(building.recipeEntity);
				var output = entityManager.GetComponentData<RecipeOutput>(building.recipeEntity);

				if (inputsSlotsParent.transform.childCount != inputs.Length)
				{
					foreach (Transform child in inputsSlotsParent.transform)
					{
						Destroy(child.gameObject);
					}
					for (int i = 0; i < inputs.Length; i++)
					{
						var newSlot = Instantiate(inputSlotPrefab, inputsSlotsParent.transform);
						newSlot.GetComponent<Image>().sprite = ItemManager.Instance.itemMap[inputs[i].itemName.ToString()].itemSprite;
						newSlot.GetComponentInChildren<TextMeshProUGUI>().text = inputs[i].amount.ToString();
					}
					outputSlot.GetComponent<Image>().sprite = ItemManager.Instance.itemMap[output.itemName.ToString()].itemSprite;
				}
				else
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
}
