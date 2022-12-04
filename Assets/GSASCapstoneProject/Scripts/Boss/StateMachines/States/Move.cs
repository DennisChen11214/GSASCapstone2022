using System.Collections;
using TMPro.EditorUtilities;
using UnityEngine;


public class Move : iState
{
    private BossStateFSM _manager;
    private Vector3 _location;
    public Move(BossStateFSM manager)
    {
        this._manager = manager;
    }

    public void OnEnter()
    {
        _manager.canMove = false;
        // choose a random different predefined location 
        int idx = Random.Range(0, _manager.locations.Length);
        _location = _manager.locations[idx].position;
        float diff = (_location - _manager.BossTransform.position).magnitude;
        if (diff < 0.1f)
        {
            idx = (idx + 1) % _manager.locations.Length;
            _location = _manager.locations[idx].position;
        }
        
        
        if (_manager.DEBUG) Debug.Log("Move Done OnEnter()");
    }

    public void OnUpdate(float dt)
    {
        // move the boss to the selected location
        if ((_location - _manager.BossTransform.position).magnitude < 1.0f)
          {
              _manager.TransitionToState(BossStateType.Idle, "Move");
          }
          _manager.BossTransform.position += _manager.moveSpeed * Time.deltaTime * (_location - _manager.BossTransform.position).normalized;
    }

    public void OnExit()
    {
        _manager.canMove = false;
        _manager.StartCoroutine(MoveCooldown());
        if (_manager.DEBUG) Debug.Log("Move Done OnExit()");
    }
    

    private IEnumerator MoveCooldown()
    {
        yield return new WaitForSeconds(_manager.MoveCooldown);
        _manager.canMove = true;
    }
}