using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed;

    void Update()
    {
        Vector3 position = transform.position;
        position.x = MyMoveTowards(position.x, target.position.x, speed * Time.deltaTime);
        transform.position = position;
    }

    float MyMoveTowards(float current, float target, float maxDelta)
    {
        float step = Mathf.Clamp(target - current,-maxDelta, maxDelta); 
        return current  + step; 
        //
    }
}
