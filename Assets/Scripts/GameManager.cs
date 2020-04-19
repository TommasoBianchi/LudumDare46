using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int startingPopulation;
    [SerializeField, Range(0, 1)] private float populationGrowthPercentage;

    [SerializeField] private TextMeshProUGUI turnCounterText;
    [SerializeField] private List<DragDestination> dragDestinationsToRecoverCardsFrom;
    [SerializeField] private List<DragDestination> dragDestinationsToDiscardCardsFrom;
    [SerializeField] private Transform cardsPanel;
    [SerializeField] private CardUnlockOverlay cardUnlockOverlay;
    [SerializeField] private Transform gameOverOverlay;

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    private int turnCounter;
    private float realPopulationCounter;
    private Dictionary<ResourceType, int> resourceAmounts;
    private Deck deck;

    private List<CardData> cardsToUnlock;

    private event System.Action<ResourceType, int> onResourceAmountChange;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("There should be only one GameManager");
            return;
        }

        turnCounter = 1;
        turnCounterText.text = "1";

        resourceAmounts = new Dictionary<ResourceType, int>();
        foreach(ResourceType resourceType in ResourceLocator.GetAllResourceTypes())
        {
            SetResourceAmount(resourceType, 0);
        }
        realPopulationCounter = startingPopulation;
        SetResourceAmount(ResourceType.CurrentPopulation, startingPopulation);
        SetResourceAmount(ResourceType.TotalPopulation, startingPopulation);
        SetResourceAmount(ResourceType.FoodToBeConsumed, -startingPopulation);

        deck = GameObject.FindObjectOfType<Deck>();

        cardsToUnlock = Resources.LoadAll<CardData>("Cards").Where(cardData => cardData.UnlockRequirement > 0 && cardData.Level == 1).ToList();

        deck.Setup();
    }

    public bool CanPlayCard(Card card, DragDestination destination)
    {
        bool canPlay = true;
        bool canPayCosts = true;

        foreach(CardData.Cost cost in card.Data.Costs)
        {
            canPayCosts = canPayCosts && (resourceAmounts[cost.ResourceType] >= cost.Amount);
        }

        canPlay = canPlay && canPayCosts;

        if (destination.DestinationType == DragDestination.Type.UpgradeCard)
        {
            canPlay = canPlay && (card.Data.Upgrade != null);
        }

        int gainTotalSoldiersAmount = card.Data.Effects
            .Where(effect => effect.EffectType == CardData.Effect.Type.AddResources && effect.ResourceType == ResourceType.TotalSoldiers)
            .Sum(effect => effect.ResourceAmount);
        canPlay = canPlay && (resourceAmounts[ResourceType.SoldiersStorage] >= gainTotalSoldiersAmount);

        return canPlay;
    }

    public void ResolveCard(Card card, DragDestination destination)
    {
        // Pay costs
        foreach(CardData.Cost cost in card.Data.Costs)
        {
            SetResourceAmount(cost.ResourceType, resourceAmounts[cost.ResourceType] - cost.Amount);
        }
        
        // Resolve effects
        switch(destination.DestinationType)
        {
            case DragDestination.Type.PlayCard:
                foreach (CardData.Effect effect in card.Data.Effects)
                {
                    switch(effect.EffectType)
                    {
                        case CardData.Effect.Type.AddResources:
                            SetResourceAmount(effect.ResourceType, resourceAmounts[effect.ResourceType] + effect.ResourceAmount);
                            break;
                    }
                }
                break;
            case DragDestination.Type.CloneCard:
                deck.AddCard(card.Data);
                break;
            case DragDestination.Type.RemoveCard:
                break;
            case DragDestination.Type.Ritual:
                break;
            case DragDestination.Type.UpgradeCard:
                deck.AddCard(card.Data.Upgrade);
                break;
        }

        // Draw a new card
        deck.DrawCard();
    }

    public void SetResourceAmount(ResourceType resourceType, int amount)
    {
        if(!resourceAmounts.ContainsKey(resourceType))
        {
            resourceAmounts.Add(resourceType, amount);
        }
        else
        {
            resourceAmounts[resourceType] = amount;
        }
       
        onResourceAmountChange?.Invoke(resourceType, amount);
    }

    public void AddResourceAmountChangeListener(System.Action<ResourceType, int> listener)
    {
        foreach(var entry in resourceAmounts)
        {
            listener.Invoke(entry.Key, entry.Value);
        }

        onResourceAmountChange += listener;
    }

    public void EndTurn()
    {
        ++turnCounter;
        turnCounterText.text = turnCounter.ToString();

        // Spend the food necessary to sustain the population
        int currentFood = resourceAmounts[ResourceType.CurrentFood];
        int foodToBeConsumed = -resourceAmounts[ResourceType.FoodToBeConsumed];

        if(currentFood < foodToBeConsumed)
        {
            SetResourceAmount(ResourceType.CurrentFood, 0);
            GameOver();
            return;
        }

        SetResourceAmount(ResourceType.CurrentFood, Mathf.Clamp(currentFood - foodToBeConsumed, 0, resourceAmounts[ResourceType.FoodStorage]));

        // Grow the population
        realPopulationCounter += Mathf.Max(1, populationGrowthPercentage * realPopulationCounter);
        int newTotalPopulation = Mathf.FloorToInt(realPopulationCounter);
        SetResourceAmount(ResourceType.CurrentPopulation, newTotalPopulation);
        SetResourceAmount(ResourceType.TotalPopulation, newTotalPopulation);
        SetResourceAmount(ResourceType.FoodToBeConsumed, -(newTotalPopulation + 2 * resourceAmounts[ResourceType.TotalSoldiers]));

        // Reset current soldiers to the total
        SetResourceAmount(ResourceType.CurrentSoldiers, resourceAmounts[ResourceType.TotalSoldiers]);

        // Reclaim cards from the board and put them either back in the deck or away
        foreach (DragDestination destination in dragDestinationsToRecoverCardsFrom)
        {
            Card[] cards = destination.Clear();

            foreach(Card card in cards)
            {
                deck.AddCard(card.Data);
                Destroy(card.gameObject);
            }
        }

        foreach (Card card in cardsPanel.GetComponentsInChildren<Card>())
        {
            deck.AddCard(card.Data);
            Destroy(card.gameObject);
        }

        foreach (DragDestination destination in dragDestinationsToDiscardCardsFrom)
        {
            Card[] cards = destination.Clear();

            foreach (Card card in cards)
            {
                Destroy(card.gameObject);
            }
        }

        List<CardData> candidatesCardsToUnlock = cardsToUnlock.Where(cardData => cardData.UnlockRequirement <= resourceAmounts[ResourceType.Faith]).ToList();
        if (candidatesCardsToUnlock.Count > 0)
        {
            cardUnlockOverlay.Activate(candidatesCardsToUnlock.ToArray().ToList());
        }
        else
        {
            StartTurn();
        }
    }

    public void StartTurn()
    {
        // Shuffle the deck and pick 5 new cards
        deck.Shuffle();
        deck.DrawCards(5);
    }

    private void GameOver()
    {
        gameOverOverlay.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UnlockCard(Card card)
    {
        if (card != null)
        {
            SetResourceAmount(ResourceType.Faith, resourceAmounts[ResourceType.Faith] - card.Data.UnlockRequirement);
            deck.AddCard(card.Data);
            cardsToUnlock.Remove(card.Data);
        }
        deck.Shuffle();
        StartTurn();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
