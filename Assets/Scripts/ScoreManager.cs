using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        setScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getScore()
    {
        return score;
    }

    public void setScore(int _score)
    {
        score = _score;
    }

    public void addToScore(int _score)
    {
        score += _score;
    }
}
