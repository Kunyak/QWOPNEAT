#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Services;

#endregion

namespace QWOPNEAT
{
    public class Game : WaveEngine.Framework.Game
    {

        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            this.Load(WaveContent.GameInfo);

            ScreenContext screenContext = new ScreenContext(new GameplayScene());
            WaveServices.ScreenContextManager.To(screenContext);
        }

    }
}

