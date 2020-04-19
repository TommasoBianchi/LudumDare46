using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUnlockOverlay : MonoBehaviour
{
    [SerializeField] private Transform rootOverlay;
    [SerializeField] private int maxUnlocksPerTurn;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private Button unlockButtonPrefab;

    public void Activate(List<CardData> unlockCandidates)
    {
        if (unlockCandidates.Count < 1)
        {
            return;
        }

        List<Card> possibleCards = new List<Card>();
        while(possibleCards.Count < maxUnlocksPerTurn && unlockCandidates.Count > 0)
        {
            int index = Random.Range(0, unlockCandidates.Count);

            Card card = Instantiate(cardPrefab);
            card.SetData(unlockCandidates[index]);

            Button unlockButton = Instantiate(unlockButtonPrefab, transform);
            card.transform.SetParent(unlockButton.transform);
            unlockButton.onClick.AddListener(() => UnlockCard(card));

            card.transform.localScale = Vector3.one * 1.5f;

            possibleCards.Add(card);
            unlockCandidates.RemoveAt(index);
        }

        rootOverlay.gameObject.SetActive(true);
    }

    public void UnlockCard(Card card)
    {
        GameManager.Instance.UnlockCard(card);

        for (int i = transform.childCount - 1; i >= 0 ; --i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        rootOverlay.gameObject.SetActive(false);
    }

    public void SkipUnlock()
    {
        UnlockCard(null);
    }
}
