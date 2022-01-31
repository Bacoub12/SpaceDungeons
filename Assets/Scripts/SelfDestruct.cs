using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    float timeBeforeDestroy = 1f;

    void Start()
    {
        Destroy(gameObject, timeBeforeDestroy);
    }
}
