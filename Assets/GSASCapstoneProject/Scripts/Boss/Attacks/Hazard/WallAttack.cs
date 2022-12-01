using Core.GlobalVariables;
using UnityEngine;

public class WallAttack : MonoBehaviour
{
    [SerializeField]
    private LayerMask _stopLayer;
    [SerializeField]
    private FloatVariable _thinWallSpeed;
    [SerializeField]
    private FloatVariable _enragedSpeedMultiplier;
    [SerializeField]
    private BoolVariable _isBossEnraged;

    private Rigidbody2D _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCombat player = collision.GetComponent<PlayerCombat>();
        if (player)
        {
            player.TakeDamage(true);
        }
        else if (_stopLayer == (_stopLayer | (1 << collision.gameObject.layer)))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        _rb.velocity = Vector2.right * _thinWallSpeed.Value;
        if (_isBossEnraged.Value)
        {
            _rb.velocity *= _enragedSpeedMultiplier.Value;
        }
    }
}
