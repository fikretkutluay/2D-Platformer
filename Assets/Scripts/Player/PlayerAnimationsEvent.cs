using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsEvent : MonoBehaviour
{
    private Player player;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void FinishRespawn() => player.RespawnFinished(true);
}
