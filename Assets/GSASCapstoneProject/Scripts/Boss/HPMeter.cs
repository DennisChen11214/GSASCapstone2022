using Core.GlobalVariables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPMeter : MonoBehaviour
{
    public FloatVariable hp;
    [SerializeField] Scrollbar HpBar;
    [SerializeField] float fullHp; 

    private void Start()
    {
        if (HpBar == null)
        {
            HpBar = gameObject.GetComponent<Scrollbar>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HpBar.size = hp.Value / fullHp;
    }
}
