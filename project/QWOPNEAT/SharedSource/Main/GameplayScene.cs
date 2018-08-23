#region Using Statements
using NEATLibrary;
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Cameras;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Resources;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;
#endregion

namespace QWOPNEAT
{
    public class GameplayScene : Scene
    {

        private Color backgroundColor = new Color(163 / 255f, 178 / 255f, 97 / 255f);
        private Color darkColor = new Color(120 / 255f, 39 / 255f, 72 / 255f);
        private Color lightColor = new Color(157 / 255f, 73 / 255f, 133 / 255f);

        protected override void CreateScene()
        {
            this.Load(WaveContent.Scenes.GameplayScene);
            var asd = new TestClass();
           // CreateDebugMode();
        }

        private void CreateDebugMode()
        {
            ToggleSwitch debugMode = new ToggleSwitch()
            {
                OnText = "Debug On",
                OffText = "Debug Off",
                Margin = new Thickness(5),
                IsOn = true,
                Width = 200,
                Foreground = darkColor,
                Background = lightColor,
            };

            debugMode.Toggled += (s, o) =>
            {
                RenderManager.DebugLines = debugMode.IsOn;
            };

            EntityManager.Add(debugMode);
        }

    }
}
