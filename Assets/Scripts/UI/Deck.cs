using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Deck : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deckCountText;
    [SerializeField] private Transform cardsPanel;
    [SerializeField] private Card cardPrefab;
    [SerializeField, Range(0, 1)] private float cardGlobalScale;

    [SerializeField] private List<CardData> startingCards;

    private Queue<CardData> deck;
    private List<CardData> addedCards;

    public void Setup()
    {
        deck = new Queue<CardData>();
        addedCards = new List<CardData>();

        foreach (CardData cardData in startingCards)
        {
            deck.Enqueue(cardData);
        }

        Shuffle();

        deckCountText.text = deck.Count.ToString();
    }

    public void Shuffle()
    {
        CardData[] cards = deck.ToArray().Concat(addedCards).ToArray();
        deck.Clear();
        addedCards.Clear();
        
        for (int i = cards.Length - 1; i >= 1; --i)
        {
            int j = Random.Range(0, i + 1);

            CardData tmp = cards[i];
            cards[i] = cards[j];
            cards[j] = tmp;
        }

        foreach (CardData card in cards)
        {
            deck.Enqueue(card);
        }
    }

    public void AddCard(CardData card)
    {
        addedCards.Add(card);
    }

    public void DrawCard()
    {
        if (deck.Count < 1)
        {
            return;
        }

        CardData cardData = deck.Dequeue();
        Card card = Instantiate(cardPrefab);
        card.SetData(cardData);
        card.gameObject.SetActive(true);
        card.transform.SetParent(cardsPanel);

        RectTransform cardTransform = card.GetComponent<RectTransform>();
        cardTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cardTransform.sizeDelta.x * cardGlobalScale);
        cardTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cardTransform.sizeDelta.y * cardGlobalScale);
        card.transform.localScale = Vector3.one;

        deckCountText.text = deck.Count.ToString();
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; ++i)
        {
            DrawCard();
        }
    }
}