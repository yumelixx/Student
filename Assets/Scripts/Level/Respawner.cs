using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] Gem gemPrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float respawnDelay;

    void OnEnable()
    {
        GameEvents.OnTargetDestroyed += TargetDestroyed;
    }

    void OnDisable() 
    {
        GameEvents.OnTargetDestroyed -= TargetDestroyed; 
    }
    
    void TargetDestroyed(GameItem gameItem)
    {
        if (gameItem == GameItem.Gem)
        {
            StartCoroutine( Respawn() );
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        Instantiate(gemPrefab, spawnPoint.position, spawnPoint.rotation);
        
    }
}
