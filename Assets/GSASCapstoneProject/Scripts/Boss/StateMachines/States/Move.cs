using System.Collections;
using TMPro.EditorUtilities;
using UnityEngine;


public class Move : iState
{
    private BossStateFSM _manager;
    private Vector3 _location;
    private Coroutine _moveCouroutine = null;
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

        // move the boss to that location
        _moveCouroutine = _manager.StartCoroutine(_Move());
        
        if (_manager.DEBUG) Debug.Log("Move Done OnEnter()");
    }

    public void OnUpdate(float dt)
    {
        
    }

    public void OnExit()
    {
        _manager.StopCoroutine(_moveCouroutine);
        _manager.canMove = false;
        _manager.StartCoroutine(MoveCoolDown());
        if (_manager.DEBUG) Debug.Log("Move Done OnExit()");
    }

    private IEnumerator _Move()
    {
        float reactionTime = 0.07f;
        // stop playing Idle animation before moving
        _manager.animator.enabled = false;
        yield return new WaitForSeconds(reactionTime);
        _manager.BossTransform.position = _location;
        yield return new WaitForSeconds(reactionTime);
        _manager.animator.enabled = true;

        _manager.TransitionToState(BossStateType.Idle, "Move");
        yield return null;
    }

    private IEnumerator MoveCoolDown()
    {
        yield return new WaitForSeconds(_manager.MoveCoolDown);
        _manager.canMove = true;
    }
}