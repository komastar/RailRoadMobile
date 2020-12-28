namespace Assets.Foundation.Interface
{
    public interface ITutorialPhase
    {
        void Enter();
        ITutorialPhase Update(IActor actor);
        void Exit();
    }
}
