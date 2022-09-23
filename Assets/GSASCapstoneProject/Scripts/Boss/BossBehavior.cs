using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.GlobalVariables;

public class BossBehavior : MonoBehaviour
{
    public FloatVariable hp;
    public GameObject target;
    public GameObject bullet;

    // Player attacks must use the component "DamageDealer" to deal damage.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageDealer dd = collision.gameObject.GetComponent<DamageDealer>();
        if (dd != null && dd.CompareTag("PlayerAttack"))
        {
            hp.Value -= dd.damage;
            Debug.Log("Boss got hit");
        }
    }
    
    IEnumerator BulletAttack()
    {
        while(true)
        {
            Vector2 dir = target.transform.position - transform.position;
            dir = dir.normalized;
            if (bullet != null)
            {
                GameObject b = GameObject.Instantiate(bullet, transform.position, Quaternion.identity);
                b.GetComponent<Bullet>().dir = dir;
            }
            yield return new WaitForSeconds(3);
        }
       
    }

    private void Start()
    {
        StartCoroutine(BulletAttack());
    }
}
