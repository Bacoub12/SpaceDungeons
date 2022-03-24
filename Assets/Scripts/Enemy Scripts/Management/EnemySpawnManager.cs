using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using System.Text.RegularExpressions;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] chests;
    public GameObject[] armorPieces;

    private NavMeshSurface surface;

    private float conjurerSpawnOdds, dashSpawnOdds, illusionistSpawnOdds, 
        rifleSpawnOdds, shotgunSpawnOdds, spiderSpawnOdds, meleeSpawnOdds;
    private float armorSpawnOdds, healthSpawnOdds, moneySpawnOdds;
    private bool everythingSpawned;
    private string terrainNameString;
    private string enemyGameObjectRegex;

    // Start is called before the first frame update
    void Start()
    {
        //générer navmesh
        surface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();
        surface.RemoveData();
        surface.BuildNavMesh();

        conjurerSpawnOdds = 5f;
        dashSpawnOdds = 15f;
        illusionistSpawnOdds = 10f;
        rifleSpawnOdds = 25f;
        shotgunSpawnOdds = 20f;
        spiderSpawnOdds = 10f;
        meleeSpawnOdds = 15f;
        //adds up to 100

        armorSpawnOdds = 30f;
        healthSpawnOdds = 30f;
        moneySpawnOdds = 40f;
        //adds up to 100

        everythingSpawned = false;

        enemyGameObjectRegex = "^Enemy(?!Bullet|SpawnManager)"; //check for enemy but not bullet or spawnmanager

        //・enlever (et généer par GameManager général) dans le jeu final
        spawnEnemiesOnTerrain();
        spawnChestsOnTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        if (everythingSpawned && getEnemyCount() <= 2)
        {
            //check for all enemies dead
            GameObject terrain = GameObject.Find(terrainNameString);
            GameObject enemySpawns = terrain.transform.GetChild(terrain.transform.childCount - 2).gameObject;
            int enemyLayerMask = 1 << 7;
            int playerLayerMask = 1 << 8; //ou plutot "gun" mais bon c'est équivalent pour nos besoins
            foreach (Transform spawnTransform in enemySpawns.transform)
            {
                if (Physics.CheckSphere(spawnTransform.position, 5f, enemyLayerMask) || Physics.CheckSphere(spawnTransform.position, 10f, playerLayerMask))
                {
                    spawnEnemyAtPosition(spawnTransform.position);
                }
            }
        }
    }

    public int getEnemyCount()
    {
        int enemyCount = 0;
        foreach (GameObject GOinScene in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            if (Regex.IsMatch(GOinScene.name, enemyGameObjectRegex))
            {
                enemyCount++;
            }
        }
        return enemyCount;
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
                terrainNameString = terrain.name;
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
                //spawnEnemyAtPosition(t.position);
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
            spawnChestAtPosition(chestSpawnTransform.position, chestSpawnTransform.rotation);
        }
    }

    private void spawnEnemyAtPosition(Vector3 position)
    {
        //roll from 0 to 100, split that into 'brackets', spawn based on what 'bracket' the number fell in
        float rolledNumber = Random.Range(0f, 100f);
        float bracketMinimum = 0;
        float bracketMaximum = 0 + conjurerSpawnOdds;
        bool spawnDone = false;
        Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        //CRINGE INCOMING!! CRINGE!! CRINGE!!

        //conjurer
        if (rolledNumber >= bracketMinimum && rolledNumber < bracketMaximum)
        {
            //Debug.Log("spawning conjurer at " + position.x + " " + position.y + " " + position.z);
            Instantiate(enemies[0], position, randomYRotation);
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
            //Debug.Log("spawning dash at " + position.x + " " + position.y + " " + position.z);
            Instantiate(enemies[1], position, randomYRotation);
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
            //Debug.Log("spawning illusionist at " + position.x + " " + position.y + " " + position.z);
            Instantiate(enemies[2], position, randomYRotation);
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
            //Debug.Log("spawning rifle at " + position.x + " " + position.y + " " + position.z);
            Instantiate(enemies[3], position, randomYRotation);
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
            //Debug.Log("spawning shotgun at " + position.x + " " + position.y + " " + position.z);
            Instantiate(enemies[4], position, randomYRotation);
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
            //Debug.Log("spawning spider at " + position.x + " " + position.y + " " + position.z);
            Instantiate(enemies[5], position, randomYRotation);
            spawnDone = true;
        }
        else
        {
            bracketMinimum = bracketMaximum;
            bracketMaximum = bracketMinimum + meleeSpawnOdds;
        }

        //melee
        if (spawnDone == false && rolledNumber >= bracketMinimum && rolledNumber <= bracketMaximum)
        {
            //Debug.Log("spawning zombie at " + position.x + " " + position.y + " " + position.z);
            Instantiate(enemies[6], position, randomYRotation);
            spawnDone = true;
        }

        if (spawnDone == true)
        {
            //Debug.Log("rolled enemy number = " + rolledNumber);
        }
        else
        {
            //Debug.Log("not supposed to be here. rolled enemy number = " + rolledNumber);
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
