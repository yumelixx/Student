using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, IDamagable
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] List<Color> colors;

    int indexColor = 0;

    public void Damage(int direction)
    {
        if (indexColor >= colors.Count)
        {
            Destroy(gameObject);
            GameEvents.TargetDestroyed(GameItem.Gem);
            return;
        }

        spriteRenderer.color = colors[indexColor];
        indexColor++;
    }
}
