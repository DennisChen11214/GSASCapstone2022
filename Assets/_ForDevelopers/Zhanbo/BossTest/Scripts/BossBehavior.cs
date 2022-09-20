using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    IEnumerator BasicAttack()
    {
        yield return new WaitForSeconds(1);
    }


}
