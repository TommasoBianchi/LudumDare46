using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private Transform costsPanel;
    [SerializeField] private Transform imagePanel;
    [SerializeField] private Transform effectsPanel;

    [SerializeField] private Transform levelIconPrefab;
    [SerializeField] private Transform costUIPrefab;
    [SerializeField] private Transform effectUIPrefab;

    private CardData data;
    public CardData Data { get { return data; } }

    private Transform startDragParent;
    private Vector3 startDragPosition;
    private bool hasBeenDropped;

    private void Start()
    {
        hasBeenDropped = false;
    }

    public void SetData(CardData data)
    {
        this.data = data;

        UpdateGraphics();
    }
    
    public void UpdateGraphics()
    {
        if(data == null)
        {
            return;
        }

        nameText.text = data.Name;

        for(int i = levelPanel.childCount - 1; i >= 0; --i)
        {
            Destroy(levelPanel.GetChild(i).gameObject);
        }

        for (int i = costsPanel.childCount - 1; i >= 0; --i)
        {
            Destroy(costsPanel.GetChild(i).gameObject);
        }

        for (int i = effectsPanel.childCount - 1; i >= 0; --i)
        {
            Destroy(effectsPanel.GetChild(i).gameObject);
        }

        for(int i = 0; i < data.Level; ++i)
        {
            Instantiate(levelIconPrefab, levelPanel);
        }

        foreach(CardData.Cost cost in data.Costs)
        {
            Transform costTransform = Instantiate(costUIPrefab, costsPanel);
            costTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cost.Amount.ToString();
            costTransform.GetChild(1).GetComponent<Image>().sprite = ResourceLocator.GetResource(cost.ResourceType).Icon;
        }

        foreach (CardData.Effect effect in data.Effects)
        {
            Transform effectTransform = Instantiate(effectUIPrefab, effectsPanel);
            Transform onlyTextPanel = effectTransform.GetChild(0);
            Transform resourceEffectPanel = effectTransform.GetChild(1);

            switch (effect.EffectType)
            {
                case CardData.Effect.Type.AddResources:
                    resourceEffectPanel.GetChild(0).GetComponent<TextMeshProUGUI>().text = effect.ResourceAmount.ToString();
                    resourceEffectPanel.GetChild(1).GetComponent<Image>().sprite = ResourceLocator.GetResource(effect.ResourceType).Icon;
                    break;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startDragPosition = transform.position;
        startDragParent = transform.parent;
        //transform.SetParent(CardDraggingManager.Instance.transform);
        transform.localScale = Vector3.one;

        // Turn off raycasting against the card itself to make raycast needed for
        // the drop pass through
        this.GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!hasBeenDropped)
        {
            ResetDrag();
        }
    }

    public void DragInto(DragDestination destination)
    {
        destination.DragCard(this);

        hasBeenDropped = true;

        // Make the card non-interactable so that we can no longer drag it away
        this.GetComponent<Image>().raycastTarget = false;

        GameManager.Instance.ResolveCard(this, destination);
    }

    private void ResetDrag()
    {
        transform.position = startDragPosition;
        transform.SetParent(startDragParent);

        // Make the card interactable again
        this.GetComponent<Image>().raycastTarget = true;
    }
}
