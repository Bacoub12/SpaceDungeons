using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem particules;

    float timeBeforeDestroy = 3f;

    private float baseDamage, trueDamage;
    private bool damageUpgrade1, damageUpgrade2, damageUpgrade3;
    
    void Start()
    {
        Destroy(gameObject, timeBeforeDestroy);
        baseDamage = 10;
        damageUpgrade1 = false;
        damageUpgrade2 = false;
        damageUpgrade3 = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player" && other.gameObject.tag != "Gun" && other.gameObject.tag != "Bullet")
        {
            //Debug.Log("tag de  : " + other.gameObject.tag);
            Instantiate(particules, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);

        }
        else
        {
            //Debug.Log("tag else : " + other.gameObject.tag);
            //Destroy(gameObject);
        }
    }

    public void setDamageParams(float _baseDamage, bool _damageUpgrade1, bool _damageUpgrade2, bool _damageUpgrade3)
    {
        baseDamage = _baseDamage;
        damageUpgrade1 = _damageUpgrade1;
        damageUpgrade2 = _damageUpgrade2;
        damageUpgrade3 = _damageUpgrade3;
        calculateTrueDamage();
    }

    private void calculateTrueDamage()
    {
        trueDamage = baseDamage;
        if (damageUpgrade1)
            trueDamage *= 1.5f;
        if (damageUpgrade2)
            trueDamage *= 1.5f;
        if (damageUpgrade3)
            trueDamage *= 1.5f;
    }

    public float getTrueDamage()
    {
        return trueDamage;
    }
}
