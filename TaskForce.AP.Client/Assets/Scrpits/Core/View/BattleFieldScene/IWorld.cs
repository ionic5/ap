using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IWorld
    {
        Vector2 GetPlayerUnitSpawnPosition();
        Vector2 GetWarpPoint();
        bool IsOutOfCameraView(Vector2 vector2);
    }
}
