using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;



    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManagar.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManagar.Instance.OnRecipeComplated += DeliveryManager_OnRecipeComplated;


        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeComplated(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container )
        {
            if(child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }


        foreach(RecipeSO recipeSO in DeliveryManagar.Instance.GetWaitingRecipeSOList())
        {
            Transform recipeTransform = Instantiate(recipeTemplate,container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }
}
