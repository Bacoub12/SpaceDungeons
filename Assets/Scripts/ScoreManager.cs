using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private double score;
    private bool onStreak;

    // Start is called before the first frame update
    void Start()
    {
        setScore(0);
        onStreak = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public double getScore()
    {
        return score;
    }

    public void setScore(double _score)
    {
        score = _score;
    }

    public void addToScore(double _score)
    {
        double scoreMultiplier = 1f;
        if (onStreak)
            scoreMultiplier *= 1.5;
        score += _score * scoreMultiplier;
        Debug.Log("Score: " + score);
    }

    IEnumerator streakCoroutine()
    {
        onStreak = true;
        yield return new WaitForSeconds(3f);
        onStreak = false;
    }
}
