using UnityEngine;

public class Key : MonoBehaviour, IDamagable
{
    [SerializeField] float scaleFactor;
    [SerializeField] float minScale;

    public void Damage(int direction)
    {
        Vector3 scale = transform.localScale;
        scale -= Vector3.one * scaleFactor;
        transform.localScale = scale;

        if (transform.localScale.x < minScale)
        {
            Destroy(gameObject);
            GameEvents.TargetDestroyed(GameItem.Key);
        }
    }
}
