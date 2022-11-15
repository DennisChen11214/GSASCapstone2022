using Core.GlobalVariables;
using UnityEngine;

public class WallAttack : MonoBehaviour
{
    [SerializeField]
    private LayerMask _playerLayer;
    [SerializeField]
    private LayerMask _stopLayer;
    [SerializeField]
    private FloatVariable _thinWallSpeed;

    private Rigidbody2D _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playerLayer == (_playerLayer | (1 << collision.gameObject.layer)))
        {
            collision.GetComponent<PlayerCombat>().TakeDamage();
        }
        else if (_stopLayer == (_stopLayer | (1 << collision.gameObject.layer)))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _rb.velocity = Vector2.right * _thinWallSpeed.Value;
    }
}
