using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Debug shortcuts for testing: K destroys the player, J destroys the next gem.
public class DebugKeys : MonoBehaviour
{
    [SerializeField] PlayerPhysics player;
    [SerializeField] List<Gem> gems;

    void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            KillPlayer();
        }

        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            DestroyNextGem();
        }
    }

    void KillPlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }
    }

    void DestroyNextGem()
    {
        for (int i = 0; i < gems.Count; i++)
        {
            if (gems[i] != null)
            {
                gems[i].Damage(1);
                return;
            }
        }
    }
}
