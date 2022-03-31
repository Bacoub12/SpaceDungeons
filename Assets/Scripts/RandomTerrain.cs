using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class RandomTerrain : MonoBehaviour
{
    public GameObject[] layouts;
    public GameObject enemySpawnManager;
    public GameObject bossSpawnManager;
    public GameObject exitZone;
    public GameObject finalExitZone;
    public GameObject playerSpawnLocation;

    public int floor;

    private List<string> usedLayoutNames;
    private Vector3 playerSpawnPosition;
    private int nbrReserveForces;
    private int nbrEnemyTerrains;
    private GameObject bossTerrain;

    // Start is called before the first frame update
    void Start()
    {
        floor = 1;

        nbrReserveForces = 0;
        nbrEnemyTerrains = 9;
        int positionX = 0;
        int rndNombre;
        bool terrainHasBeenUsed;
        usedLayoutNames = new List<string>();

        //generate normal enemy zones

        for (int i = 0; i < nbrEnemyTerrains; i++){

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

            GameObject spawnedTerrainE = Instantiate(layouts[rndNombre - 1], new Vector3(positionX, 0, 0), Quaternion.identity);
            GameObject spawnManagerE = Instantiate(enemySpawnManager, spawnedTerrainE.transform);

            if (i == 0) //if on first map, set spawner active
            {
                spawnManagerE.SetActive(true);
                spawnManagerE.GetComponent<EnemySpawnManager>().setReserveForces(nbrReserveForces);
            }

            //create exit zone
            GameObject spawnedExitZoneE = Instantiate(exitZone, spawnedTerrainE.transform);
            spawnedExitZoneE.transform.position = spawnedExitZoneE.transform.position + new Vector3(0f, 0.5f, 15f);

            //create player spawn location
            playerSpawnPosition = new Vector3(-13f, 0.09f, -95.8f);
            GameObject spawnedPlayerSpawnLocationE = Instantiate(playerSpawnLocation, spawnedTerrainE.transform);
            spawnedPlayerSpawnLocationE.transform.position = spawnedPlayerSpawnLocationE.transform.position + playerSpawnPosition;

            if (i == 0) //if on first map, set player location
                GameObject.Find("Player").transform.position = spawnedPlayerSpawnLocationE.transform.position;

            usedLayoutNames.Add(spawnedTerrainE.name);

            positionX += 200;
        }

        //generate boss zone
        rndNombre = Random.Range(1, 4); //1 à 3
        int terrainNo = 0;
        switch (rndNombre)
        {
            case 1: //get terrain 3
                terrainNo = 3;
                break;
            case 2: //get terrain 8
                terrainNo = 8;
                break;
            case 3: //get terrain 10
                terrainNo = 10;
                break;
        }
        bossTerrain = Instantiate(layouts[terrainNo - 1], new Vector3(positionX, 0, 0), Quaternion.identity);
        GameObject spawnManager = Instantiate(bossSpawnManager, bossTerrain.transform);

        GameObject spawnedExitZone = Instantiate(finalExitZone, bossTerrain.transform);
        spawnedExitZone.transform.position = spawnedExitZone.transform.position + new Vector3(0f, 0.5f, 15f);

        playerSpawnPosition = new Vector3(-13f, 0.09f, -95.8f);
        GameObject spawnedPlayerSpawnLocation = Instantiate(playerSpawnLocation, bossTerrain.transform);
        spawnedPlayerSpawnLocation.transform.position = spawnedPlayerSpawnLocation.transform.position + playerSpawnPosition;


        //generate navmesh
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
        if (usedLayoutNames.IndexOf(currentTerrain.name) < (nbrEnemyTerrains - 1)) //if not on last enemy terrain
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

            //start enemyspawnmanager (& bump up its reserve forces)
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
        else
        {
            //disable current spawnmanager
            currentTerrain.transform.Find("EnemySpawnManager(Clone)").gameObject.SetActive(false);

            //find next terrain
            GameObject nextTerrain = bossTerrain;

            //start bossspawnmanager
            GameObject nextSpawnManager = nextTerrain.transform.Find("BossSpawnManager(Clone)").gameObject;
            nextSpawnManager.SetActive(true);

            //set exit zone in current terrain active
            GameObject currentExitZone = currentTerrain.transform.Find("ExitZone(Clone)").gameObject;
            currentExitZone.SetActive(true);
            currentExitZone.GetComponent<ExitZoneScript>()
                .setDestination(nextTerrain.transform.Find("PlayerSpawnLocation(Clone)"));
        }
    }

    public void readyPortalHome(GameObject currentTerrain)
    {
        //set exit zone in current terrain active
        GameObject currentExitZone = currentTerrain.transform.Find("ExitZoneFinal(Clone)").gameObject;
        currentExitZone.SetActive(true);
        currentExitZone.GetComponent<ExitZoneScript>()
            .setBackToLobby(true);
    }
}
