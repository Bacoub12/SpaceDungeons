using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class RandomTerrain : MonoBehaviour
{
    public GameObject[] layouts;
    public GameObject enemySpawnManager;
    public GameObject exitZone;
    public GameObject playerSpawnLocation;

    private List<string> usedLayoutNames;
    private Vector3 playerSpawnPosition;
    private int nbrReserveForces;

    // Start is called before the first frame update
    void Start()
    {
        nbrReserveForces = 0;
        int positionX = 0;
        int rndNombre;
        bool terrainHasBeenUsed;
        usedLayoutNames = new List<string>();

        for (int i = 0; i < 10; i++){

            do
            {
                rndNombre = Random.Range(1, 16);

                terrainHasBeenUsed = false;
                foreach (string name in usedLayoutNames)
                {
                    string tentativeTerrainName = "TemplateGameMap" + rndNombre + "(Clone)";
                    if (name == tentativeTerrainName)
                        terrainHasBeenUsed = true;
                }

            } while (terrainHasBeenUsed);

            GameObject spawnedTerrain = Instantiate(layouts[rndNombre - 1], new Vector3(positionX, 0, 0), Quaternion.identity);
            GameObject spawnManager = Instantiate(enemySpawnManager, spawnedTerrain.transform);

            if (i == 0) //if on first map, set spawner active
            {
                spawnManager.SetActive(true);
                spawnManager.GetComponent<EnemySpawnManager>().setReserveForces(nbrReserveForces);
            }

            if (i < 9) //if not on last map, create exit zone
            {
                GameObject spawnedExitZone = Instantiate(exitZone, spawnedTerrain.transform);
                spawnedExitZone.transform.position = spawnedExitZone.transform.position + new Vector3(0f, 0.5f, 15f);
            }

            //create player spawn location
            playerSpawnPosition = new Vector3(-13f, 0.09f, -95.8f);
            GameObject spawnedPlayerSpawnLocation = Instantiate(playerSpawnLocation, spawnedTerrain.transform);
            spawnedPlayerSpawnLocation.transform.position = spawnedPlayerSpawnLocation.transform.position + playerSpawnPosition;

            if (i == 0) //if on first map, set player location
                GameObject.Find("Player").transform.position = spawnedPlayerSpawnLocation.transform.position;

            usedLayoutNames.Add(spawnedTerrain.name);

            positionX += 200;
        }

        //générer navmesh
        NavMesh.RemoveAllNavMeshData();
        NavMeshSurface surface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void readyNextTerrain (GameObject currentTerrain)
    {
        //disable current spawnmanager
        currentTerrain.transform.Find("EnemySpawnManager(Clone)").gameObject.SetActive(false);

        //find next terrain
        GameObject nextTerrain = null;
        foreach (string terrainName in usedLayoutNames)
        {
            if (currentTerrain.name == terrainName)
            {
                int currentIndex = usedLayoutNames.IndexOf(terrainName);
                string nextTerrainName = usedLayoutNames[currentIndex + 1];
                nextTerrain = GameObject.Find(nextTerrainName);
            }
        }

        //start enemyspawnmanager in next terrain, bump up its reserve forces
        GameObject nextSpawnManager = nextTerrain.transform.Find("EnemySpawnManager(Clone)").gameObject;
        nextSpawnManager.SetActive(true);
        nbrReserveForces++;
        nextSpawnManager.GetComponent<EnemySpawnManager>().setReserveForces(nbrReserveForces);

        //set exit zone in current terrain active
        GameObject currentExitZone = currentTerrain.transform.Find("ExitZone(Clone)").gameObject;
        currentExitZone.SetActive(true);
        currentExitZone.GetComponent<ExitZoneScript>()
            .setDestination(nextTerrain.transform.Find("PlayerSpawnLocation(Clone)"));
    }
}
