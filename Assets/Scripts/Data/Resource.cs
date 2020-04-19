using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ResourceType
{
    All,
    CurrentPopulation,
    TotalPopulation,
    CurrentFood,
    FoodToBeConsumed,
    FoodStorage,
    Faith,
    Wood,
    Stone,
    CurrentSoldiers,
    TotalSoldiers,
    SoldiersStorage
}

[CreateAssetMenu(menuName = "Game Data/Resource")]
public class Resource : ScriptableObject
{
    [SerializeField] private ResourceType type;
    public ResourceType Type { get { return type; } }

    [SerializeField] private Sprite icon;
    public Sprite Icon { get { return icon; } }
}

public static class ResourceLocator
{
    private static Dictionary<ResourceType, Resource> resourceDict;

    static ResourceLocator()
    {
        resourceDict = new Dictionary<ResourceType, Resource>();
        foreach (Resource resource in Resources.LoadAll<Resource>("Resources"))
        {
            if (resourceDict.ContainsKey(resource.Type))
            {
                Debug.LogWarning("There is more than one resource of type " + resource.Type);
            }

            resourceDict.Add(resource.Type, resource);
        }
    }

    public static Resource GetResource(ResourceType type)
    {
        if(!resourceDict.ContainsKey(type))
        {
            Debug.LogWarning("ResourceLocator does not know about " + type);
            return null;
        }

        return resourceDict[type];
    }

    public static ResourceType[] GetAllResourceTypes()
    {
        return resourceDict.Keys.ToArray();
    }
}
