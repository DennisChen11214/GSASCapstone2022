///
/// Created by Dennis Chen
///
/// Created by Dennis Chen
///

using UnityEngine;

public class OutOfBoundsCheck : MonoBehaviour
{
    [SerializeField] Transform _respawnPoint;
    public bool _on;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If the player falls out of the map, respawn them at a given location and deal 1 damage to them
        if (!_on) return;
        PlayerCombat player = collision.gameObject.GetComponent<PlayerCombat>();
        if(player != null)
        {
            player.TakeDamageNoKnockback();
            player.gameObject.transform.position = _respawnPoint.position;
        }
    }
}
