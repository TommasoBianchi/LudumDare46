using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ResourceUI : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private Transform resourceChangeEffectPrefab;

    private TextMeshProUGUI resourceAmountText;
    private int oldAmount;

    private void Start()
    {
        resourceAmountText = GetComponent<TextMeshProUGUI>();
        oldAmount = 0;

        GameManager.Instance.AddResourceAmountChangeListener(OnResourceAmountChange);
    }

    private void OnResourceAmountChange(ResourceType resourceType, int amount)
    {
        if (this.resourceType == resourceType)
        {
            resourceAmountText.text = amount.ToString();

            int change = amount - oldAmount;
            oldAmount = amount;
            if (resourceChangeEffectPrefab != null && change != 0)
            {
                Transform effect = Instantiate(resourceChangeEffectPrefab, transform);
                effect.GetComponent<TextMeshProUGUI>().text = ((change > 0) ? "+" : "") + change;
            }
        }
    }
}
