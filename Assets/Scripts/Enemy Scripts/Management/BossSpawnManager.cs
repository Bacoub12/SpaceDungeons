using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class BossSpawnManager : MonoBehaviour
{
    public GameObject boss;
    public GameObject[] chests;
    public GameObject[] armorPieces;

    private string enemyGameObjectRegex;
    private float armorSpawnOdds, healthSpawnOdds, moneySpawnOdds;
    private bool everythingSpawned;

    // Start is called before the first frame update
    void Start()
    {
        armorSpawnOdds = 30f;
        healthSpawnOdds = 30f;
        moneySpawnOdds = 40f;
        //adds up to 100

        everythingSpawned = false;

        enemyGameObjectRegex = "^Enemy(?!Bullet|Spawn|SpiderNest)"; //check for enemy but not bullet/spawnmanager/spidernest

        spawnBoss();
        spawnChestsOnTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        if (everythingSpawned && getEnemyCount() <= 0)
        {
            GameObject.Find("TerrainGenerator").GetComponent<RandomTerrain>()
                .readyPortalHome(transform.parent.gameObject);
        }
    }

    public int getEnemyCount()
    {
        int enemyCount = 0;
        foreach (GameObject GOinScene in FindObjectsOfType<GameObject>())
        {
            if (Regex.IsMatch(GOinScene.name, enemyGameObjectRegex))
            {
                //Debug.Log(GOinScene.name);
                enemyCount++;
            }
        }
        //Debug.Log(enemyCount);
        return enemyCount;
    }

    public void spawnBoss()
    {
        Transform bossSpawnTransform = null;
        for (int i = 1; i <= 15; i++)
        {
            if (transform.parent.Find("Terrain" + i) != null)
            {
                GameObject terrain = transform.parent.Find("Terrain" + i).gameObject;
                bossSpawnTransform = terrain.transform.Find("BossSpawn");
                Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                Instantiate(boss, bossSpawnTransform.position, randomYRotation);
            }
        }
    }

    public void spawnChestsOnTerrain()
    {
        //Debug.Log("salut");

        Transform chestSpawnTransform = null;
        for (int i = 1; i <= 15; i++)
        {
            if (transform.parent.Find("Terrain" + i) != null)
            {
                GameObject terrain = transform.parent.Find("Terrain" + i).gameObject;
                chestSpawnTransform = terrain.transform.GetChild(terrain.transform.childCount - 1).gameObject.transform;
            }
        }

        if (chestSpawnTransform == null || chestSpawnTransform.gameObject.name != "ChestSpawn")
        {
            Debug.Log("aucun chest spawn trouvé");
        }
        else
        {
            spawnChestAtPosition(chestSpawnTransform.position, chestSpawnTransform.rotation);
        }
    }

    private void spawnChestAtPosition(Vector3 position, Quaternion rotation)
    {
        float rolledNumber = Random.Range(0f, 100f);
        float bracketMinimum = 0;
        float bracketMaximum = 0 + armorSpawnOdds;
        bool spawnDone = false;

        //armor
        if (rolledNumber >= bracketMinimum && rolledNumber < bracketMaximum)
        {
            GameObject armorChest = Instantiate(chests[0], position, rotation);
            float rolledNumberArmor = Random.Range(0f, 100f);
            if (rolledNumberArmor >= 0 && rolledNumberArmor < 25)
            {
                armorChest.GetComponent<ChestScript>().drop = armorPieces[0];
            }
            else if (rolledNumberArmor >= 25 && rolledNumberArmor < 50)
            {
                armorChest.GetComponent<ChestScript>().drop = armorPieces[1];
            }
            else if (rolledNumberArmor >= 50 && rolledNumberArmor < 75)
            {
                armorChest.GetComponent<ChestScript>().drop = armorPieces[2];
            }
            else if (rolledNumberArmor >= 75 && rolledNumberArmor <= 100)
            {
                armorChest.GetComponent<ChestScript>().drop = armorPieces[3];
            }
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
            Instantiate(chests[1], position, rotation);
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
            Instantiate(chests[2], position, rotation);
            spawnDone = true;
        }

        if (spawnDone == true)
        {
            //Debug.Log("rolled chest number = " + rolledNumber);
        }
        else
        {
            //Debug.Log("not supposed to be here. rolled chest number = " + rolledNumber);
        }

        everythingSpawned = true;
    }
}
