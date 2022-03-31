using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text.RegularExpressions;

public class enemyConjurerScript : MonoBehaviour
{
    public GameObject summonedEnemy;
    public GameObject summonZone;
    public GameObject summonParticles;
    public GameObject summonEndParticles;
    public GameObject explosion;
    public GameObject destSphere;
    public Animator anim;
    public AudioSource audioSummon;
    public AudioSource audioSummonEnd;
    public AudioSource audioWalk;
    public Transform eye;
    [SerializeField] float health;

    NavMeshAgent agent;
    float walkRadius = 7f;
    float minWalkDistance = 3f;
    float maxWalkDistance = 7f;
    bool canSummon;
    int maxSpawnsPerSummoner = 5; //5? 10?
    bool dead;
    GameObject existingAttackVisual;
    GameObject existingParticles;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        canSummon = true;
        dead = false;
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

            bool possibleSpawnPoint = false;
            Vector3 spawnPosition = new Vector3(0f, 0f, 0f);
            NavMeshHit hitMesh;
            if (NavMesh.SamplePosition(transform.position + (transform.forward * 2f), out hitMesh, 2f, NavMesh.AllAreas))
            {
                possibleSpawnPoint = true;
                spawnPosition = hitMesh.position;
            }

            if (summoned.Length < totalMaxSummons && possibleSpawnPoint)
            {
                StartCoroutine(summon(spawnPosition));
            }
            else
            {
                StartCoroutine(onHold());
            }
        }
    }

    private void setNewDest()
    {
        bool pathIsValid = false;

        while (pathIsValid == false)
        {
            Vector3 newDest = new Vector3(
            transform.position.x + Random.Range(-walkRadius, walkRadius),
            transform.position.y,
            transform.position.z + Random.Range(-walkRadius, walkRadius));

            //drawray from eye to location
            bool destInSight = false;

            GameObject dest = Instantiate(destSphere, newDest, Quaternion.identity);
            Vector3 eyePos = eye.position;
            Vector3 vectorToDest = (eyePos - dest.transform.position) * -1;

            int enemyLayerIndex = LayerMask.NameToLayer("Enemy");
            int enemyLayerMask = (1 << enemyLayerIndex);
            enemyLayerMask = ~enemyLayerMask;

            RaycastHit hit; //= new RaycastHit()
            if (Physics.Raycast(eyePos, vectorToDest, out hit, walkRadius, enemyLayerMask, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.gameObject.tag == "Destination")
                {
                    destInSight = true;
                }
            }

            Destroy(dest);

            float distToDest = Vector3.Distance(transform.position, newDest);

            agent.SetDestination(newDest);

            if (destInSight == true && distToDest > minWalkDistance)
            {
                pathIsValid = true;

                anim.SetBool("Walk_Anim", true);
                if (!audioWalk.isPlaying)
                    audioWalk.Play();
                audioWalk.mute = false;
            }
        }

        /*
        Vector3 newDest = new Vector3(
            transform.position.x + Random.Range(-walkRadius, walkRadius),
            transform.position.y,
            transform.position.z + Random.Range(-walkRadius, walkRadius));

        float distToDest = Vector3.Distance(transform.position, newDest);

        if (distToDest > minWalkDistance)
        {
            agent.SetDestination(newDest);

            anim.SetBool("Walk_Anim", true);
            if (!audioWalk.isPlaying)
                audioWalk.Play();
            audioWalk.mute = false;
        }
        */
    }

    private float getPathLength(NavMeshPath path)
    {
        float length = 0.0f;

        if (path.status != NavMeshPathStatus.PathInvalid)
        {
            Debug.Log("path corners: " + path.corners.Length);
            for (int i = 1; i < path.corners.Length; ++i)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }
        else
        {
            Debug.Log("path invalid?");
        }

        Debug.Log("path length: " + length);
        return length;
    }

    IEnumerator summon(Vector3 summonLocation)
    {
        canSummon = false;
        //when this starts, conjurer has reached destination
        agent.SetDestination(transform.position);

        anim.SetBool("Walk_Anim", false);
        if (!audioWalk.isPlaying)
            audioWalk.Play();
        audioWalk.mute = true;

        //starts conjuring, takes 2 seconds, put an effect where the summoning takes place
        audioSummon.Play();
        existingAttackVisual = Instantiate(summonZone, summonLocation, Quaternion.identity);
        existingParticles = Instantiate(summonParticles, summonLocation, Quaternion.identity);
        existingParticles.transform.Rotate(-90f, 0f, 0f, Space.Self);
        yield return new WaitForSeconds(2f);

        //conjures
        audioSummon.Stop();
        audioSummonEnd.Play();
        Destroy(existingAttackVisual);
        ParticleSystem particlesPSys = existingParticles.GetComponent<ParticleSystem>();
        particlesPSys.Stop();
        Instantiate(summonEndParticles, summonLocation, transform.rotation);
        Instantiate(summonedEnemy, summonLocation, transform.rotation);

        yield return new WaitForSeconds(1f);

        setNewDest();
        canSummon = true;
    }

    IEnumerator onHold()
    {
        anim.SetBool("Walk_Anim", false);
        if (!audioWalk.isPlaying)
            audioWalk.Play();
        audioWalk.mute = true;

        yield return new WaitForSeconds(1f);
        setNewDest();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet(Clone)")
        {
            TakeDamage(other.gameObject.GetComponent<BulletScript>().getTrueDamage());
            callForAid(GameObject.Find("Player").transform.position);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            dead = true;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;

            audioWalk.mute = true;

            if (existingAttackVisual != null)
            {
                Destroy(existingAttackVisual);
            }

            if (existingParticles != null)
            {
                Destroy(existingParticles);
            }

            Instantiate(explosion, transform.position, transform.rotation);

            ScoreManager scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
            scoreManager.addToScore(15);

            //Invoke(nameof(DestroyThis), 3f);
            DestroyThis();
        }
    }

    public void callForAid(Vector3 playerPos)
    {
        string enemyGameObjectRegex = "^Enemy(?!Bullet|SpawnManager)";
        foreach (GameObject GOinScene in FindObjectsOfType<GameObject>())
        {
            if (Regex.IsMatch(GOinScene.name, enemyGameObjectRegex)
                && Vector3.Distance(gameObject.transform.position, GOinScene.transform.position) <= 10f
                && GOinScene != gameObject)
            {
                switch (GOinScene.name)
                {
                    case "EnemyDash":
                    case "EnemyDash(Clone)":
                        GOinScene.GetComponent<Animator>().SetBool("Walk Forward", true);
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemyIllusionist":
                    case "EnemyIllusionist(Clone)":
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemyMelee":
                    case "EnemyMelee(Clone)":
                        GOinScene.GetComponent<Animation>().Play("Run");
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemyRifle":
                    case "EnemyRifle(Clone)":
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemyShotgun":
                    case "EnemyShotgun(Clone)":
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                    case "EnemySpider":
                    case "EnemySpider(Clone)":
                        GOinScene.GetComponent<Animator>().SetBool("running", true);
                        GOinScene.GetComponent<NavMeshAgent>().SetDestination(playerPos);
                        break;
                }
            }
        }
    }

    public bool checkIfDead()
    {
        return dead;
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
