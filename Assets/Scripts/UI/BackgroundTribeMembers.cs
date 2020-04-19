using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundTribeMembers : MonoBehaviour
{
    [SerializeField] private Sprite populationSprite;
    [SerializeField] private Sprite soldierSprite;

    void Start()
    {
        Transform populationHolder = new GameObject("Population Holder").transform;
        populationHolder.SetParent(transform);

        Transform soldiersHolder = new GameObject("Soldiers Holder").transform;
        soldiersHolder.SetParent(transform);

        GameManager.Instance.AddResourceAmountChangeListener((resourceType, amount) =>
        {
            int currentAmount = (resourceType == ResourceType.TotalPopulation) ? populationHolder.childCount : soldiersHolder.childCount;
            int change = amount - currentAmount;

            if (amount <= 0 || change == 0 || (resourceType != ResourceType.TotalPopulation && resourceType != ResourceType.TotalSoldiers))
            {
                return;
            }

            bool isChangeNegative = change < 0;
            change = Mathf.Abs(change);

            for (int i = 0; i < change; ++i)
            {
                if(isChangeNegative)
                {
                    Destroy(((resourceType == ResourceType.TotalPopulation) ? populationHolder : soldiersHolder).GetChild(0));
                }
                else
                {
                    SpriteRenderer newSprite = new GameObject("Tribe Member").AddComponent<SpriteRenderer>();
                    newSprite.transform.localScale = new Vector3(0.65f, 0.65f, 1);
                    newSprite.sprite = (resourceType == ResourceType.TotalPopulation) ? populationSprite : soldierSprite;
                    newSprite.transform.SetParent((resourceType == ResourceType.TotalPopulation) ? populationHolder : soldiersHolder);
                    newSprite.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), 0));
                    newSprite.sortingOrder = -Mathf.FloorToInt(newSprite.transform.position.y * 100);
                }
            }
        });
    }
}
