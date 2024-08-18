using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using System.Collections.Generic;
using ZamTools.Components;
using ZamTools.Utils;

namespace ZamTools.Services
{
    public class MainWindowComponent
    {
        public static List<Item> characterItems = Inventory.Items;
        private static ZamLogger logger = ZamLogger.GetLogger("MainWindowService");
        private static MainWindowComponent instance = null;


        private MainWindowComponent() 
        {
            logger.debug($"MainWindowComponent initialized!");
        }

        public static MainWindowComponent CreateInstance() 
        {
            if (instance == null)
                instance = new MainWindowComponent();

            return instance;
        }

        public static void DestroyInstance()
        {
            logger.info($"Destroying MainWindowComponent Services...");
            if (instance != null)
                instance = null;
        }

        public static void Render() 
        {
            ComponentManager.MAIN_WINDOW.Show(true);
            if (ComponentManager.MAIN_WINDOW.IsValid)
            {
                if (ComponentManager.MAIN_WINDOW.FindView("createBagManagerButton", out ButtonBase createBagManagerButton))
                {
                    createBagManagerButton.Clicked += CreateBagManagerButtonCallBack;
                }

                if (ComponentManager.MAIN_WINDOW.FindView("createMailBoxDuper", out ButtonBase createMailBoxDuper))
                {
                    createMailBoxDuper.Clicked += CreateMailBoxDuperButtonCallBack;
                }

                if (ComponentManager.MAIN_WINDOW.FindView("warpForwardButton", out ButtonBase warpForwardButton))
                {
                    warpForwardButton.Clicked += WarpManagerComponent.WarpForwardButtonCallBack;
                }


                if (ComponentManager.MAIN_WINDOW.FindView("warpToTargetButton", out ButtonBase warpToTargetButton))
                {
                    warpToTargetButton.Clicked += WarpManagerComponent.WarpToTargetButtonCallBack;
                }
            }
        }
        public static void Updates()
        {
            if (ComponentManager.MAIN_WINDOW.IsValid)
            {
                if (ComponentManager.MAIN_WINDOW.FindView("duperSelectionLabel", out TextView duperSelectionLabel))
                    duperSelectionLabel.Text = $"DupeManager Selection: {ComponentManager.DM_SELECTION}";

                if (ComponentManager.MAIN_WINDOW.FindView("stackerSelectionLabel", out TextView stackerSelectionLabel))
                    stackerSelectionLabel.Text = $"Stacker Selection: {ComponentManager.BM_SELECTION}";

                if (ComponentManager.MAIN_WINDOW.FindView("focusedBagLabel", out TextView focusedBagLabel))
                    focusedBagLabel.Text = $"BagManager Selection: {BagManagerComponent.GetStackBagId()}";

                if (ComponentManager.MAIN_WINDOW.FindView("stackerStateLabel", out TextView stackerStateLabel))
                    stackerStateLabel.Text = $"Stacker Status: {ComponentManager.BM_STACKER_STATE}";

                if (ComponentManager.MAIN_WINDOW.FindView("duperStateLabel", out TextView duperStateLabel))
                    duperStateLabel.Text = $"Duper Status: {ComponentManager.DUPER_STATE}";
                //
                if (ComponentManager.MAIN_WINDOW.FindView("infoLabel", out TextView infoLabel))
                    infoLabel.Text = $"Information:";

                if (ComponentManager.MAIN_WINDOW.FindView("versionLabel", out TextView versionLabel))
                    versionLabel.Text = $"ZamTools v1.0.0";

                if (ComponentManager.MAIN_WINDOW.FindView("welcomeLabel", out TextView welcomeLabel))
                    welcomeLabel.Text = $"I got bored again :D";
            }
        }

        public static void CreateBagManagerButtonCallBack(object sender, ButtonBase e) 
        {
            BagManagerComponent.Render();
        }

        public static void CreateMailBoxDuperButtonCallBack(object sender, ButtonBase e) 
        {
            DupeManagerComponenet.CreateInstance();
            DupeManagerComponenet.Render();
        }
    }
}
