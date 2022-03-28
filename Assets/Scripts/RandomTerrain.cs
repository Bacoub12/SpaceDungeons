using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTerrain : MonoBehaviour
{
    public GameObject[] layouts;
    public GameObject enemySpawnManager;
    public GameObject exitZone;

    private List<string> usedLayoutNames;

    // Start is called before the first frame update
    void Start()
    {
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

            if (i == 0) //if on first map
            {
                spawnManager.SetActive(true);
            }

            if (i < 9) //if not on last map
            {
                GameObject spawnedExitZone = Instantiate(exitZone, spawnedTerrain.transform);
                spawnedExitZone.transform.position = spawnedExitZone.transform.position + new Vector3(0f, 0.5f, 15f);
            }

            usedLayoutNames.Add(spawnedTerrain.name);

            positionX += 200;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void readyNextTerrain (GameObject currentTerrain)
    {
        //find next terrain
        GameObject nextTerrain = new GameObject();
        foreach (string terrainName in usedLayoutNames)
        {
            if (currentTerrain.name == terrainName)
            {
                int currentIndex = usedLayoutNames.IndexOf(terrainName);
                string nextTerrainName = usedLayoutNames[currentIndex + 1];
                nextTerrain = GameObject.Find(nextTerrainName);
            }
        }

        //start enemyspawnmanager in next terrain
        nextTerrain.transform.Find("EnemySpawnManager(Clone)").gameObject.SetActive(true);
    }
}
