using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static Transform PlayerPostion => player.transform;
    private static GameObject player = null;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player").First(player => player.name.Equals("Player"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
