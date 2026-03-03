using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class DiedEventArgs : EventArgs
    {
        public readonly IMortal DiedTarget;

        public DiedEventArgs(IMortal diedTarget)
        {
            DiedTarget = diedTarget;
        }
    }
}
