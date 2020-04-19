using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Card")]
public class CardData : ScriptableObject
{
    [SerializeField] private new string name;
    public string Name { get { return name; } }

    [SerializeField] private List<Cost> costs;
    public List<Cost> Costs { get { return costs; } }

    [SerializeField] private List<Effect> effects;
    public List<Effect> Effects { get { return effects; } }

    [SerializeField] private int unlockRequirement;
    public int UnlockRequirement { get { return unlockRequirement; } }

    [SerializeField] private int level = 1;
    public int Level { get { return level; } }

    [SerializeField] private bool isRitual = false;
    public bool IsRitual { get { return isRitual; } }

    [SerializeField] private CardData upgrade;
    public CardData Upgrade { get { return upgrade; } }

    [System.Serializable]
    public struct Cost
    {
        [SerializeField] private int amount;
        public int Amount { get { return amount; } }

        [SerializeField] private ResourceType resourceType;
        public ResourceType ResourceType { get { return resourceType; } }
    }

    [System.Serializable]
    public struct Effect
    {
        public enum Type
        {
            AddResources,
            DiscardCard,
            CloneCard,
            RemoveCard,
            UpgradeCard,
            ActivateRitual
        }

        [SerializeField] private Type effectType;
        public Type EffectType { get { return effectType; } }

        [SerializeField] private ResourceType resourceType;
        public ResourceType ResourceType { get { return resourceType; } }

        [SerializeField] private int resourceAmount;
        public int ResourceAmount { get { return resourceAmount; } }
    }
}
