using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTerrain : MonoBehaviour
{
    public GameObject[] layouts;

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

            usedLayoutNames.Add(spawnedTerrain.name);

            positionX += 200;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
