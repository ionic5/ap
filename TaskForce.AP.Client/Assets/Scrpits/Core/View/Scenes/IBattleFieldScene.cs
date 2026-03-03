namespace TaskForce.AP.Client.Core.View.Scenes
{
    public interface IBattleFieldScene : IDestroyable
    {
        void SetExp(int v);
        void SetLevel(string v);
        void SetRequireExp(int v);
    }
}
