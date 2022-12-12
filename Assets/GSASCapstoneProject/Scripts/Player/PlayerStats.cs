///
/// Created by Dennis Chen
///

using Core.GlobalVariables;
using UnityEngine;
using TMPro;

//Used at the end of a game to display how much damage each player dealt, took, and how many times they died
public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private FloatVariable _damageDealt;
    [SerializeField]
    private FloatVariable _damageTaken;
    [SerializeField]
    private IntVariable _numDeaths;
    [SerializeField]
    private TMP_Text  _damageDealtText;
    [SerializeField]
    private TMP_Text _damageTakenText;
    [SerializeField]
    private TMP_Text _numDeathsText;

    private void Start()
    {
        _damageDealtText.text = "" + Mathf.RoundToInt(_damageDealt.Value);
        _damageTakenText.text = "" + Mathf.RoundToInt(_damageTaken.Value);
        _numDeathsText.text = "" + _numDeaths.Value;
    }
}
