using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.System
{

    public abstract class PhaseState
    {
        protected static Dictionary<Constant.PhaseState, PhaseState> statePool = null;
        public static PhaseState GetFirstState()
        {
            if (null == statePool)
            {
                statePool = new Dictionary<Constant.PhaseState, PhaseState>
                {
                    { Constant.PhaseState.Start, new StartPhase() },
                    { Constant.PhaseState.Ready, new ReadyPhase() },
                    { Constant.PhaseState.Draw, new DrawPhase() },
                    { Constant.PhaseState.Build, new BuildPhase() },
                    { Constant.PhaseState.End, new EndPhase() }
                };
            }

            return statePool[Constant.PhaseState.Start];
        }

        public static async Task UpdateLoopAsync(StageController stage, CancellationToken token)
        {
            PhaseState currState = GetFirstState();
            PhaseState prevState = currState;

            await currState.EnterState();
            while (!token.IsCancellationRequested)
            {
                currState = await currState.UpdateStateAsync(stage);
                if (prevState != currState)
                {
                    await prevState.ExitState();
                    await currState.EnterState();
                    prevState = currState;
                }
            }
        }

        public abstract Task EnterState();
        public abstract Task<PhaseState> UpdateStateAsync(StageController stage);
        public abstract Task ExitState();
    }

    public class StartPhase : PhaseState
    {
        public override async Task EnterState()
        {
            await Task.Yield();
        }

        public override async Task ExitState()
        {
            await Task.Yield();
        }

        public override async Task<PhaseState> UpdateStateAsync(StageController stage)
        {
            await Task.Yield();

            return statePool[Constant.PhaseState.Ready];
        }
    }

    public class ReadyPhase : PhaseState
    {
        public override async Task EnterState()
        {
            await Task.Yield();
        }

        public override async Task ExitState()
        {
            await Task.Yield();
        }

        public override async Task<PhaseState> UpdateStateAsync(StageController stage)
        {
            await Task.Delay(100);

            return statePool[Constant.PhaseState.Draw];
        }
    }

    public class DrawPhase : PhaseState
    {
        public override async Task EnterState()
        {
            await Task.Yield();
        }

        public override async Task ExitState()
        {
            await Task.Yield();
        }

        public override async Task<PhaseState> UpdateStateAsync(StageController stage)
        {
            await stage.Draw();

            return statePool[Constant.PhaseState.Build];
        }
    }

    public class BuildPhase : PhaseState
    {
        public override async Task EnterState()
        {
            await Task.Yield();
        }

        public override async Task ExitState()
        {
            await Task.Yield();
        }

        public override async Task<PhaseState> UpdateStateAsync(StageController stage)
        {
            await Task.Yield();

            if (Input.GetMouseButtonDown(1))
            {
                stage.ResetHand();

                return statePool[Constant.PhaseState.End];
            }
            return statePool[Constant.PhaseState.Build];
        }
    }

    public class EndPhase : PhaseState
    {
        public override async Task EnterState()
        {
            await Task.Yield();
        }

        public override async Task ExitState()
        {
            await Task.Yield();
        }

        public override async Task<PhaseState> UpdateStateAsync(StageController stage)
        {
            await Task.Yield();
            await Task.Delay(500);

            return statePool[Constant.PhaseState.Ready];
        }
    }
}
