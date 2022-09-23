using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20;
    public float speedInc = 30;
    public Vector2 dir;



    // Update is called once per frame
    void Update()
    {
        Vector3 v = dir * speed * Time.deltaTime;
        transform.position += v;
        speed += speedInc * Time.deltaTime;
    }



}
