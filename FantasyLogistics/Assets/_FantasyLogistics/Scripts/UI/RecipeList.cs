using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FantasyLogistics
{
	public class RecipeList : MonoBehaviour
	{
		public RecipeSO[] recipes;
		public GameObject recipeUIPrefab;
		public Transform recipeList;
		public RecipeStateUI recipeState;

		public Entity activeBuilding;

		private void Start()
		{
			foreach (var recipe in recipes)
			{
				GameObject recipeUIElement = Instantiate(recipeUIPrefab, recipeList);
				recipeUIElement.transform.position = Vector3.zero;
				var image = recipeUIElement.GetComponentInChildren<Image>();
				image.sprite = recipe.GetSprite();

				var name = recipeUIElement.GetComponentInChildren<TextMeshProUGUI>();
				name.text = recipe.recipeName;


				recipeUIElement.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
				{
					recipeState.SetBuildingRecipe(recipe);
				});
			}
		}
	}
}
