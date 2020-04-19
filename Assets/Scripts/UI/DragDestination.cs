using UnityEngine.EventSystems;
using UnityEngine;

public abstract class DragDestination : MonoBehaviour, IDropHandler
{
    public enum Type
    {
        PlayCard,
        CloneCard,
        RemoveCard,
        UpgradeCard,
        Ritual
    }

    [SerializeField] private Type destinationType;
    public Type DestinationType { get { return destinationType; } }

    protected abstract bool CanDragCard(Card card);
    public abstract void DragCard(Card card);
    public abstract Card[] Clear();

    public void OnDrop(PointerEventData eventData)
    {
        Card draggedCard = eventData.pointerDrag.GetComponent<Card>();

        if(this.CanDragCard(draggedCard) && GameManager.Instance.CanPlayCard(draggedCard, this))
        {
            draggedCard.DragInto(this);
        }
    }
}
