using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MultiRowDragDestination : DragDestination
{
    [SerializeField, Range(0, 1)] private float contentScale;
    [SerializeField] private List<Row> rows;

    private List<Card> content;
    private int totalSpace;

    private void Start()
    {
        content = new List<Card>();
        totalSpace = rows.Sum(row => row.maxCards);
    }

    protected override bool CanDragCard(Card card)
    {
        return content.Count < this.totalSpace;
    }

    public override void DragCard(Card card)
    {
        content.Add(card);

        foreach (Row row in rows)
        {
            if(row.container.childCount < row.maxCards)
            {
                card.transform.localScale = card.transform.localScale * contentScale;
                card.transform.SetParent(row.container);
                return;
            }
        }
    }

    public override Card[] Clear()
    {
        Card[] cards = content.ToArray();
        content.Clear();
        return cards;
    }

    [System.Serializable]
    public struct Row
    {
        public Transform container;
        public int maxCards;
    }
}
