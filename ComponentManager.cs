using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZamTools.Components;
using ZamTools.Models;
using ZamTools.Utils;

namespace ZamTools.Services
{
    public class ComponentManager
    {
        private static ZamLogger logger = ZamLogger.GetLogger("ComponentManager");
        private static ComponentManager instance = null;

        // UI VIEWS
        public static Window MAIN_WINDOW;

        public static Window BM_WINDOW;

        public static Window DM_WINDOW;

        // UI VARIABLES
        // - Bag manager stuff
        public static string BM_FOCUS { get; set; }
        public static EquipSlot BM_SELECTION { get; set; }
        public static SimpleState BM_STACKER_STATE { get; set; }
        public static string BM_ITEM_STRING { get; set; }


        // - Dupe Manager stuff
        public static EquipSlot DM_SELECTION { get; set; }
        public static SimpleState DUPER_STATE { get; set; }
        public static string DM_RECIPIENT { get; set; }
        public static string DM_ITEM_STRING { get; set; }
        public static int DUPE_QUANTITY { get; set; }
        public static int DUPE_COUNT { get; set; }
        public static bool DUPE_MANAGER_SEARCH_HIT { get; set; }

        // - Warp Manager stuff
        public static Quaternion WM_ROTATION { get; set; }
        public static Vector3 WM_PLAYER_LOCATION { get; set; }
        public static Vector3 WM_TARGET_LOCATION { get; set; }
        public static float WM_PLAYER_ANGLE_TO_TARGET { get; set; }


        public ComponentManager() {
            //Default view states
            SetDefaultState();
        }

        public static void DestroyInstance() {
            logger.info($"Destroying ComponentManager Services...");
            if (instance != null)
            {
                MainWindowComponent.DestroyInstance();
                BagManagerComponent.DestroyInstance();
                NoDropTraderComponent.DestroyInstance();
                WarpManagerComponent.DestroyInstance();
                instance = null;
            }
        }

        /// <summary>
        /// Set the defautl state for the UI
        /// </summary>
        private static void SetDefaultState() 
        {
            //Default value states (some of these will be set once the other service's have initialized)
            MAIN_WINDOW = Window.CreateFromXml("Main Window", $"{Main.PLUGIN_DIR}\\Views\\MainWindow.xml");
            BM_WINDOW = Window.CreateFromXml("Bag Manager", $"{Main.PLUGIN_DIR}\\Views\\BagManager.xml");
            DM_WINDOW = Window.CreateFromXml("Dupe Manager", $"{Main.PLUGIN_DIR}\\Views\\DupeManager.xml");

            //BM
            BM_FOCUS = Config.BAG_MANAGER_FOCUS;
            BM_SELECTION = Config.BAG_MANAGER_SELECTION;
            BM_STACKER_STATE = Config.STACKER_STATE;

            //DM
            DM_SELECTION = Config.DUPE_MANAGER_SELECTION;
            DUPE_MANAGER_SEARCH_HIT = Config.DUPE_MANAGER_SEARCH_HIT;
            DM_RECIPIENT = Config.RECIPIENT;
            DUPER_STATE = Config.DUPER_STATE;
            DUPE_QUANTITY = Config.DUPE_QUANTITY;
            DUPE_COUNT = Config.DUPE_COUNT;

            //WM
            WM_ROTATION = new Quaternion();
            WM_PLAYER_LOCATION = new Vector3();
            WM_TARGET_LOCATION = new Vector3();
            WM_PLAYER_ANGLE_TO_TARGET = 0;

            logger.debug($"ComponentManager initialized!");

        }

        /// <summary>
        /// Fetch an instance of the UIManager ( there can only be one!)
        /// </summary>
        /// <returns></returns>
        public static ComponentManager CreateInstance() 
        {
            if (instance != null)
                return instance;

            instance = new ComponentManager();
            
            logger.info($"Initilizing Services...");
            MainWindowComponent.CreateInstance();
            MainWindowComponent.Render();
            BagManagerComponent.CreateInstance();
            NoDropTraderComponent.CreateInstance();
            WarpManagerComponent.CreateInstance();

            return instance;
        }

        /// <summary>
        /// Nested within the plugin's OnUpdate, UI updates should happen here.
        /// </summary>
        public static void UIUpdate() 
        {
            if (MAIN_WINDOW.IsValid)
            {
                //Main window updates
                MainWindowComponent.Updates();

                //Bag Manager updates
                BagManagerComponent.Updates();

                //DM Updates
                DupeManagerComponenet.Updates();

                //WM Updates
                WarpManagerComponent.Updates();
            }
        }
    }
}
