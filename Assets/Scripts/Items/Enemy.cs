using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] Collider2D enemyCollider;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float flashWait;
    [SerializeField] int flashTimes;
    
   
    
    WaitForSeconds flashWaitTimer;
    WaitForSeconds FlashWaitTimer
    {
        get
        {
            if (flashWaitTimer == null)
            {
                flashWaitTimer = new WaitForSeconds(flashWait);
            }
            return flashWaitTimer;
        }
    }

  
    
    void OnDestroy()
    {
        StopAllCoroutines();
    }
    

    public void Damage(int direction)
    {
        StartCoroutine(FlashDamage());
    }

    IEnumerator FlashDamage()
    {
        enemyCollider.enabled = false; //false

        for (int i = 0; i < flashTimes; i++)
        {
            spriteRenderer.color = Color.black;
            yield return FlashWaitTimer;
            spriteRenderer.color = Color.white;
            yield return FlashWaitTimer;
        }

        enemyCollider.enabled = true;
    }
}
