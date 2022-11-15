public interface iState
{
    public abstract void OnEnter();
    public abstract void OnUpdate(float dt);
    public abstract void OnExit();
}