using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidBody2D;
    [SerializeField] float speed;
    [SerializeField] float lifeTime;

    int direction = 1;

    void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    void FixedUpdate()
    {
        rigidBody2D.linearVelocityX = speed * direction;
    }

    public void SetDirection(int newDirection)
    {
        direction = newDirection;
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var damagable = other.GetComponent<IDamagable>();
        if (damagable != null)
        {
            damagable.Damage(direction);
        }
        Destroy(gameObject);
    }
}
