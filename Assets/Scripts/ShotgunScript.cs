using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class ShotgunScript : MonoBehaviour
{
    [SerializeField] public int pelletCount;
    [SerializeField] public float spreadAngle;
    [SerializeField] public GameObject pellet;
    [SerializeField] public Transform BarrelExit;
    [SerializeField] public float force = 800f;
    public List<Quaternion> pellets;

    public void Awake()
    {
        pellets = new List<Quaternion>(pelletCount);
        for (int i = 0; i < pelletCount; i++)
        {
            pellets.Add(Quaternion.Euler(Vector3.zero));
        }
    }

    public void Shotgun()
    {
        int j = 0;
        foreach (Quaternion quat in pellets)
        {
            pellets[j] = Random.rotation;
            GameObject p = Instantiate(pellet, BarrelExit.position, BarrelExit.rotation);
            p.transform.rotation = Quaternion.RotateTowards(p.transform.rotation, pellets[j], spreadAngle);
            p.GetComponent<Rigidbody>().AddForce(p.transform.right * force);
            j++;
        }
    }

}
