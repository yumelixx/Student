using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    int score;
    Dictionary<string, int> itemValues;

    void Awake()
    {
        itemValues = new Dictionary<string, int>();
        itemValues.Add("Key", 1);
        itemValues.Add("Gem", 3);
    }

    void OnEnable()
    {
        GameEvents.OnTargetDestroyed += AddScore;
    }

    void OnDisable()
    {
        GameEvents.OnTargetDestroyed -= AddScore;
    }

    void AddScore(GameItem gameItem)
    {
        int value = itemValues[gameItem.ToString()];
        score += value;
        Debug.Log($"Puntaje: {score}");
    }
}
