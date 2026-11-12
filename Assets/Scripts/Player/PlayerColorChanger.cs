using UnityEngine;

public class PlayerColorChanger : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        GameEvents.OnTargetDestroyed += TargetDestroyedHandler;
        
    }

    void OnDisable()
    {
        GameEvents.OnTargetDestroyed -= TargetDestroyedHandler;
    }

    void TargetDestroyedHandler(GameItem gameItem)
    {
        if (gameItem == GameItem.Gem)
        {
            spriteRenderer.color = Color.red;
        }
        else if (gameItem == GameItem.Key)
        {
            spriteRenderer.color = Color.blue;
        }
    } 
}
