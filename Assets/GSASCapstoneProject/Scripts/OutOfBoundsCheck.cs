///
/// Created by Dennis Chen
///

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsCheck : MonoBehaviour
{
    [SerializeField] Transform _respawnPoint;
    public bool _on;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_on) return;
        PlayerCombat player = collision.gameObject.GetComponent<PlayerCombat>();
        if(player != null)
        {
            player.TakeDamageNoKnockback();
            player.gameObject.transform.position = _respawnPoint.position;
        }
    }
}
