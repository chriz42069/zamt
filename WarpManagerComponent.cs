using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZamTools.Services;

namespace ZamTools.Components
{
    public class WarpManagerComponent
    {
        private static ZamLogger logger = ZamLogger.GetLogger("WarpManagerComponent");
        private static WarpManagerComponent instance = null;

        private WarpManagerComponent() { logger.debug($"WarpManagerComponent initialized!"); }

        public static WarpManagerComponent CreateInstance() 
        {
            if (instance == null)
                instance = new WarpManagerComponent();

            return instance;
        }

        public static void DestroyInstance()
        {
            logger.info($"Destroying NoDropTraderComponent Services...");
            if (instance != null)
                instance = null;
        }

        public static void Run() 
        {
            
        }
        public static void Updates() 
        {
            if (ComponentManager.MAIN_WINDOW.FindView("playerPosLabel", out TextView playerPosLabel)) 
            {
                ComponentManager.WM_PLAYER_LOCATION = DynelManager.LocalPlayer.Position;
                playerPosLabel.Text = $"You: {ComponentManager.WM_PLAYER_LOCATION}";
            }

            if (ComponentManager.MAIN_WINDOW.FindView("targetPosLabel", out TextView targetPosLabel))
            {
                if (Targeting.TargetChar != null)
                {
                    ComponentManager.WM_TARGET_LOCATION = Targeting.TargetChar.Position;
                    targetPosLabel.Text = Targeting.TargetChar == null ? "Target: <No Target>" : "Target: " + Targeting.TargetChar.Position; ;
                }
            }

            if (ComponentManager.MAIN_WINDOW.FindView("playerRotLabel", out TextView playerRotLabel)) 
            {
                ComponentManager.WM_ROTATION = DynelManager.LocalPlayer.Rotation;
                playerRotLabel.Text = $"Rot: {ComponentManager.WM_ROTATION.Forward}";

            }
        }

        public static void WarpForwardButtonCallBack(object sender, ButtonBase e)
        {
            Vector3 dest = DynelManager.LocalPlayer.Position + (DynelManager.LocalPlayer.Rotation.Forward * 2);
            ComponentManager.WM_TARGET_LOCATION = dest;
            DynelManager.LocalPlayer.Position = dest;
        }


        public static void WarpToTargetButtonCallBack(object sender, ButtonBase e)
        {
            if (Targeting.TargetChar != null) 
            {
                Vector3 dest = new Vector3(Targeting.Target.Position.X, Targeting.Target.Position.Y, Targeting.Target.Position.Z);
                logger.info($" Warping to : x:{Targeting.Target.Position.X} y:{Targeting.Target.Position.Y} z:{Targeting.Target.Position.Z}");
                logger.info($"  Distance : {Targeting.TargetChar.GetLogicalRangeToTarget(DynelManager.LocalPlayer)}");
                ComponentManager.WM_TARGET_LOCATION = dest;
                DynelManager.LocalPlayer.Position = dest;
            }
        }

    }
}
