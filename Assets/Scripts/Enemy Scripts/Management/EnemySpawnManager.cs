using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Text.RegularExpressions;

public class EnemySpawnManager : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] chests;
    public GameObject[] armorPieces;
    public GameObject spawnEffect;
    public GameObject boss;

    private NavMeshSurface surface;

    private float conjurerSpawnOdds, dashSpawnOdds, illusionistSpawnOdds, 
        rifleSpawnOdds, shotgunSpawnOdds, spiderSpawnOdds, meleeSpawnOdds;
    private float armorSpawnOdds, healthSpawnOdds, moneySpawnOdds;
    private bool everythingSpawned, reserveForcesSet;
    private string terrainNameString;
    private string enemyGameObjectRegex;
    private int enemyReserveForces;

    // Start is called before the first frame update
    void Start()
    {
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
        //reserveForcesSet = false;

        enemyGameObjectRegex = "^Enemy(?!Bullet|Spawn|SpiderNest)"; //check for enemy but not bullet/spawnmanager/spidernest

        //enlever (et généer par GameManager général) dans le jeu final
        spawnEnemiesOnTerrain();
        spawnChestsOnTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        if (everythingSpawned && reserveForcesSet)
        {
            //Debug.Log(getEnemyCount() + " " + enemyReserveForces);
            if (getEnemyCount() <= 2 && enemyReserveForces > 0)
            {
                GameObject terrain = GameObject.Find(terrainNameString);
                GameObject enemySpawns = terrain.transform.GetChild(terrain.transform.childCount - 2).gameObject;

                int enemyLayerMask = 1 << 7;
                int playerLayerMask = 1 << 8; //ou plutot "gun" mais bon c'est équivalent pour nos besoins

                bool spawnCompleted = false;
                foreach (Transform spawnTransform in enemySpawns.transform)
                {
                    if (spawnCompleted == false)
                    {
                        if (!Physics.CheckSphere(spawnTransform.position, 5f, enemyLayerMask) && !Physics.CheckSphere(spawnTransform.position, 10f, playerLayerMask))
                        {
                            spawnEnemyAtPosition(spawnTransform.position);
                            enemyReserveForces--;
                            setAllEnemiesOnPlayer();
                            spawnCompleted = true;
                        }
                    }
                }
            }
            else if (getEnemyCount() <= 0 && enemyReserveForces <= 0)
            {
                GameObject.Find("TerrainGenerator").GetComponent<RandomTerrain>()
                    .readyNextTerrain(transform.parent.gameObject);
            }
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

    public void setReserveForces(int _enemyBenchedForces)
    {
        enemyReserveForces = _enemyBenchedForces;
        reserveForcesSet = true;
    }

    public void setAllEnemiesOnPlayer()
    {
        foreach (GameObject GOinScene in FindObjectsOfType<GameObject>())
        {
            if (Regex.IsMatch(GOinScene.name, enemyGameObjectRegex))
            {
                if (GOinScene.name != "EnemySpiderNest(Clone)")
                {
                    GOinScene.GetComponent<NavMeshAgent>()
                        .SetDestination(GameObject.Find("Player").transform.position);
                    switch (GOinScene.name)
                    {
                        case "EnemyBoss(Clone)":
                        case "EnemyDash(Clone)":
                            GOinScene.GetComponent<Animator>().SetBool("Walk Forward", true);
                            break;
                        case "EnemyMelee(Clone)":
                            GOinScene.GetComponent<Animation>().Play("Run");
                            break;
                        case "EnemySpider(Clone)":
                            GOinScene.GetComponent<Animator>().SetBool("running", true);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    public void spawnEnemiesOnTerrain()
    {
        //check for possible enemy spawns
        GameObject enemySpawns = null;
        for (int i = 1; i <= 15; i++)
        {
            if (transform.parent.Find("Terrain" + i) != null) {
                GameObject terrain = transform.parent.Find("Terrain" + i).gameObject;
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

        Instantiate(spawnEffect, position, Quaternion.identity);
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

    //after a few maps, say 3, boss spawn odds start going up by 10 each completed map. this should prolly be managed in a higher-scale gamemanager
    public void spawnBoss(float spawnOdds)
    {
        float rolledNumber = Random.Range(0f, 100f);
        if (rolledNumber <= spawnOdds)
        {
            //check for possible boss spawns
            GameObject bossSpawn = null;
            if (GameObject.Find("BossSpawn") != null)
            {
                bossSpawn = GameObject.Find("BossSpawn");
                Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                Instantiate(boss, bossSpawn.transform.position, randomYRotation);
            }
            else
            {
                Debug.Log("aucun boss spawn trouvé");
            }
        }
    }
}
