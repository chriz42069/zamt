using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System;
using ZamTools.Components;
using ZamTools.Services;
using ZamTools.Utils;

namespace ZamTools
{
    public class Main : IAOPluginEntry
    {
        private static ZamLogger logger = ZamLogger.GetLogger("Main");
        public static string PLUGIN_DIR = "";
        public static Settings SETTINGS;

        public void Run(string pluginDir)
        {
            PLUGIN_DIR = pluginDir;

            try {

                logger.info($"Creating ComponentManager...");
                ComponentManager.CreateInstance();

                Game.OnUpdate += OnUpdate;

            } catch (Exception e) { 
                logger.error($"Failed to load the window");
                logger.error($"{e.Message}");
            }
        }

        private void OnUpdate(object s, float deltaTime)
        {
            if(ComponentManager.MAIN_WINDOW != null)
                ComponentManager.UIUpdate();
        }

        public void Teardown()
        {
            throw new NotImplementedException();
        }
    }
}


