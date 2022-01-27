using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyConjurerScript : MonoBehaviour
{
    public GameObject summonedEnemy;
    public GameObject summonZone;

    NavMeshAgent agent;
    float walkRadius = 7f;
    float minimumWalkDistance = 3f;
    float cooldownLength = 5f;
    bool canSummon;
    int maxSpawnsPerSummoner = 5;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        canSummon = true;
        setNewDest();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 1f && canSummon)
        {
            GameObject[] summoners = GameObject.FindGameObjectsWithTag("Summoner");
            GameObject[] summoned = GameObject.FindGameObjectsWithTag("Summoned");
            int totalMaxSummons = summoners.Length * maxSpawnsPerSummoner;

            if (summoned.Length < totalMaxSummons)
            {
                StartCoroutine(summon());
            }
            else
            {
                StartCoroutine(onHold());
            }
        }
    }

    private void setNewDest()
    {
        Vector3 newDest = new Vector3(Random.Range(-walkRadius, walkRadius), transform.position.y, Random.Range(-walkRadius, walkRadius));
        if (Vector3.Distance(transform.position, newDest) > minimumWalkDistance)
            agent.SetDestination(newDest);
    }

    IEnumerator summon()
    {
        canSummon = false;
        //when this starts, conjurer has reached destination
        agent.SetDestination(transform.position);

        //starts conjuring, takes 2 seconds, put an effect where the summoning takes place
        Vector3 downShift = new Vector3(0f, -1f, 0f);
        Vector3 summonLocation = transform.position + (transform.forward * 2f) + downShift;
        GameObject zone = Instantiate(summonZone, summonLocation, Quaternion.identity);
        yield return new WaitForSeconds(2f);

        //conjures
        Destroy(zone);
        Instantiate(summonedEnemy, summonLocation, transform.rotation);

        yield return new WaitForSeconds(1f);

        setNewDest();
        canSummon = true;
    }

    IEnumerator onHold()
    {
        yield return new WaitForSeconds(1f);
        setNewDest();
    }
}