using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float lifetime;

    void Update()
    {
        lifetime -= Time.deltaTime;

        if(lifetime < 0)
        {
            Destroy(gameObject);
        }
    }
}
