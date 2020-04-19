using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleCardDragDestination : DragDestination
{
    [SerializeField, Range(0, 1)] private float contentScale;

    private Card content;

    protected override bool CanDragCard(Card card)
    {
        return content == null;
    }

    public override void DragCard(Card card)
    {
        content = card;
        card.transform.SetParent(transform);
        card.transform.localScale = card.transform.localScale * contentScale;
    }

    public override Card[] Clear()
    {
        Card card = content;
        content = null;

        return (card == null) ? new Card[0] : new Card[] { card };
    }
}
