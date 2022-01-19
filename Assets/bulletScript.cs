using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    float timeBeforeDestroy = 5f;

    void Start()
    {
        Destroy(this, timeBeforeDestroy);
    }
}
