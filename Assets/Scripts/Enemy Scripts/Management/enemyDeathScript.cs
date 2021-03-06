using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDeathScript : MonoBehaviour
{
    public string deathAnimName;
    public string animCompType;
    public string transitionType; //leave blank if animator
    public float timeBeforeDestroy;
    public GameObject deathParticles;

    Animator animator;
    Animation animation;

    // Start is called before the first frame update
    void Start()
    {
        switch (animCompType)
        {
            case "Animator":
                animator = gameObject.GetComponent<Animator>();

                switch (transitionType)
                {
                    case "Trigger":
                        animator.SetTrigger(deathAnimName);
                        break;
                    case "Bool":
                        animator.SetBool(deathAnimName, true);
                        break;
                    default:
                        Debug.Log("type de transition invalide!!!!");
                        break;
                }
                StartCoroutine(destruction(timeBeforeDestroy));
                break;

            case "Animation":
                animation = gameObject.GetComponent<Animation>();
                animation.Play(deathAnimName);
                StartCoroutine(destruction(timeBeforeDestroy));
                break;

            default:
                Debug.Log("type d'anim invalide!!!!");
                break;
        }
    }

    IEnumerator destruction(float time)
    {
        if (gameObject.name != "EnemyMeleeCorpse(Clone)")
            gameObject.GetComponent<moneyDropScript>().enabled = true;

        yield return new WaitForSeconds(time);

        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, transform.rotation);

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
