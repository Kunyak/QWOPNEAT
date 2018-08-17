#region Using Statements
using NEATLibrary;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
#endregion

namespace QWOPNEAT
{
    public class GameplayScene : Scene
    {
        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GameplayScene);
            var asd = new TestClass();
        }
    }
}
