using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] chests;

    private float conjurerSpawnOdds, dashSpawnOdds, illusionistSpawnOdds, 
        rifleSpawnOdds, shotgunSpawnOdds, spiderSpawnOdds;
    private float armorSpawnOdds, healthSpawnOdds, moneySpawnOdds;

    // Start is called before the first frame update
    void Start()
    {
        conjurerSpawnOdds = 10f;
        dashSpawnOdds = 10f;
        illusionistSpawnOdds = 10f;
        rifleSpawnOdds = 25f;
        shotgunSpawnOdds = 25f;
        spiderSpawnOdds = 10f;
        //adds up to 100

        armorSpawnOdds = 30f;
        healthSpawnOdds = 30f;
        moneySpawnOdds = 40f;
        //adds up to 100

        //à enlever (et gérer par GameManager général) dans le jeu final
        spawnEnemiesOnTerrain();
        spawnChestsOnTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnEnemiesOnTerrain()
    {
        //Debug.Log("salut");

        GameObject enemySpawns = null;
        for (int i = 1; i <= 15; i++)
        {
            if (GameObject.Find("Terrain" + i) != null) {
                GameObject terrain = GameObject.Find("Terrain" + i);
                enemySpawns = terrain.transform.GetChild(terrain.transform.childCount - 2).gameObject;
            }
        }

        if (enemySpawns == null)
        {
            Debug.Log("aucun terrain trouvé");
        } 
        else
        {
            foreach (Transform t in enemySpawns.transform)
            {
                //Debug.Log("yes");
                spawnEnemyAtPosition(t.position);
            }
        }
    }

    public void spawnChestsOnTerrain()
    {
        //Debug.Log("salut");

        Transform chestSpawnTransform = null;
        for (int i = 1; i <= 15; i++)
        {
            if (GameObject.Find("Terrain" + i) != null)
            {
                GameObject terrain = GameObject.Find("Terrain" + i);
                chestSpawnTransform = terrain.transform.GetChild(terrain.transform.childCount - 1).gameObject.transform;
            }
        }

        if (chestSpawnTransform == null || chestSpawnTransform.gameObject.name != "ChestSpawn")
        {
            Debug.Log("aucun chest spawn trouvé");
        }
        else
        {
            spawnChestAtPosition(chestSpawnTransform.position);
        }
    }

    private void spawnEnemyAtPosition(Vector3 position)
    {
        //roll from 0 to 100, split that into 'brackets', spawn based on what 'bracket' the number fell in
        float rolledNumber = Random.Range(0f, 100f);
        float bracketMinimum = 0;
        float bracketMaximum = 0 + conjurerSpawnOdds;
        bool spawnDone = false;

        //CRINGE INCOMING!! CRINGE!! CRINGE!!

        //conjurer
        if (rolledNumber >= bracketMinimum && rolledNumber < bracketMaximum)
        {
            Instantiate(enemies[0], position, Quaternion.identity);
            spawnDone = true;
        }
        else
        {
            bracketMinimum = bracketMaximum;
            bracketMaximum = bracketMinimum + dashSpawnOdds;
        }

        //dash
        if (spawnDone == false && rolledNumber >= bracketMinimum && rolledNumber < bracketMaximum)
        {
            Instantiate(enemies[1], position, Quaternion.identity);
            spawnDone = true;
        }
        else
        {
            bracketMinimum = bracketMaximum;
            bracketMaximum = bracketMinimum + illusionistSpawnOdds;
        }

        //illusionist
        if (spawnDone == false && rolledNumber >= bracketMinimum && rolledNumber < bracketMaximum)
        {
            Instantiate(enemies[2], position, Quaternion.identity);
            spawnDone = true;
        }
        else
        {
            bracketMinimum = bracketMaximum;
            bracketMaximum = bracketMinimum + rifleSpawnOdds;
        }

        //rifle
        if (spawnDone == false && rolledNumber >= bracketMinimum && rolledNumber < bracketMaximum)
        {
            Instantiate(enemies[3], position, Quaternion.identity);
            spawnDone = true;
        }
        else
        {
            bracketMinimum = bracketMaximum;
            bracketMaximum = bracketMinimum + shotgunSpawnOdds;
        }

        //shotgun
        if (spawnDone == false && rolledNumber >= bracketMinimum && rolledNumber < bracketMaximum)
        {
            Instantiate(enemies[4], position, Quaternion.identity);
            spawnDone = true;
        }
        else
        {
            bracketMinimum = bracketMaximum;
            bracketMaximum = bracketMinimum + spiderSpawnOdds;
        }

        //spider
        if (spawnDone == false && rolledNumber >= bracketMinimum && rolledNumber <= bracketMaximum)
        {
            Instantiate(enemies[5], position, Quaternion.identity);
            spawnDone = true;
        }
        else
        {
            Debug.Log("not supposed to be here. rolled number = " + rolledNumber);
        }

    }

    private void spawnChestAtPosition(Vector3 position)
    {
        float rolledNumber = Random.Range(0f, 100f);
        float bracketMinimum = 0;
        float bracketMaximum = 0 + armorSpawnOdds;
        bool spawnDone = false;

        //armor
        if (rolledNumber >= bracketMinimum && rolledNumber < bracketMaximum)
        {
            Instantiate(chests[0], position, Quaternion.identity);
            spawnDone = true;
        }
        else
        {
            bracketMinimum = bracketMaximum;
            bracketMaximum = bracketMinimum + healthSpawnOdds;
        }

        //health
        if (spawnDone == false && rolledNumber >= bracketMinimum && rolledNumber < bracketMaximum)
        {
            Instantiate(chests[1], position, Quaternion.identity);
            spawnDone = true;
        }
        else
        {
            bracketMinimum = bracketMaximum;
            bracketMaximum = bracketMinimum + moneySpawnOdds;
        }

        //money
        if (spawnDone == false && rolledNumber >= bracketMinimum && rolledNumber <= bracketMaximum)
        {
            Instantiate(chests[2], position, Quaternion.identity);
            spawnDone = true;
        }
        else
        {
            Debug.Log("not supposed to be here. rolled number = " + rolledNumber);
        }
    }
}
