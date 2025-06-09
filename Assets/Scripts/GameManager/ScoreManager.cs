using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddPoints(int amount)
    {
        score += amount;
        Debug.Log("Puntos actuales: " + score);
    }

    public int GetScore()
    {
        return score;
    }
}
