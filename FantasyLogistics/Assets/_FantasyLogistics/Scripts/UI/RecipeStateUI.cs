using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;

namespace FantasyLogistics
{
	public class RecipeStateUI : MonoBehaviour
	{
		public TMPro.TextMeshProUGUI recipeName;
		public Image recipeImage;

		public Slider progress;

		public Building building { get; private set; }
		EntityManager entityManager;

		public void SetupUI(Building building)
		{
			this.building = building;
			if (building.recipe)
			{
				recipeName.text = this.building.recipe.recipeName;
				recipeImage.sprite = this.building.recipe.GetSprite();
			}
			transform.parent.GetComponentInChildren<RecipeList>().activeBuilding = building;
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
		}
	}
}
