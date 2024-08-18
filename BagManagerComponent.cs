using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZamTools.Models;
using ZamTools.Services;
using ZamTools.Utils;

namespace ZamTools
{
    public class BagManagerComponent
    {
        private static string filePath = "";
        private static Dictionary<string, bool> bagList = new Dictionary<string, bool>();
        private static ZamLogger logger = ZamLogger.GetLogger("BagManager");
        private static BagManagerComponent instance = null;

        private BagManagerComponent() {
            filePath = $"{Main.PLUGIN_DIR}\\ContainerDictionary-{DynelManager.LocalPlayer.Name}.txt";
            InitBags();
            logger.debug($"BagManagerComponent initialized!");
        }

        public static BagManagerComponent CreateInstance() 
        {
            if (instance == null) 
                instance = new BagManagerComponent();

            return instance;
        }

        public static void DestroyInstance()
        {
            logger.info($"Destroying BagManager Services...");
            if (instance != null)
                instance = null;
        }

        /// <summary>
        /// The bags list is persisted in a csv file where the bag instance id is the key and the value is a bool
        /// if the bool is true this is our "stack" bag.
        /// </summary>
        private static void InitBags()
        {
            bool LOADED = false;
            try
            {
                //itterate the persisted file if it exist, and look for a bag with a true value
                string[] persisted = File.ReadAllLines(filePath);
                logger.debug($"Config loaded!");
                foreach (string line in persisted)
                {
                    string key = line.Split(',')[0];
                    string value = line.Split(',')[1];
                    if (Boolean.Parse(value).Equals(true))
                    {
                        logger.debug($"Current Focused Bag: {key} : {value}");
                        ComponentManager.BM_FOCUS = key.ToString();
                    }

                    bagList.Add(key, Boolean.Parse(value));
                }
                LOADED = true;
            }
            catch (Exception e)
            {
                logger.debug($"{e.Message}");
            }

            if (LOADED == false)
            {
                logger.debug($"No Container Config found, creating one.");
                ResetBags();
            }

            logger.debug($"Bags Found: {bagList.Count}");

        }

        public static void Updates() 
        {

            if (ComponentManager.BM_WINDOW.IsValid)
            {
                if (ComponentManager.BM_WINDOW.FindView("selectedValue", out TextView selectedValue))
                    selectedValue.Text = $"Slot Selection: {GetSelection()}";

                if (ComponentManager.BM_WINDOW.FindView("currentSelectionInput", out TextInputView currentSelectionInput))
                {
                    if (ComponentManager.BM_WINDOW.FindView("currentSelectionLabel", out TextView currentSelectionLabel))
                    {
                        try
                        {
                            Inventory.Items.ForEach(item =>
                            {
                                if (Enum.IsDefined(typeof(EquipSlot), item.Slot.Instance))
                                {
                                    if (item.Name.Contains(currentSelectionInput.Text) && currentSelectionInput.Text.Length > 0)
                                    {
                                        ComponentManager.BM_SELECTION = (EquipSlot)item.Slot.Instance;
                                    }
                                }
                                                               
                            });
                        }

                        catch (Exception ex)
                        {
                            logger.debug($"Error when looking for item, check the spelling.");
                            logger.debug($"{ex.Message}");
                        }

                        currentSelectionLabel.Text = $"{ComponentManager.BM_SELECTION}";
                        ComponentManager.BM_ITEM_STRING = currentSelectionInput.Text;
                    }
                }
            }

            if (ComponentManager.BM_STACKER_STATE == SimpleState.ENABLED)
                StatStacker.Stack(ComponentManager.BM_SELECTION);
        }

        public static void Render() 
        {
            ComponentManager.BM_WINDOW.Show(true);
            if (ComponentManager.BM_WINDOW.IsValid)
            {
                //LABELS
                if (ComponentManager.BM_WINDOW.FindView("selectionLabel", out TextView selectionLabel))
                {
                    selectionLabel.Text = $"Select ONE slot.";
                }
                //BUTTONS
                if (ComponentManager.BM_WINDOW.FindView("setStackBagButton", out ButtonBase setStackBagButton))
                {
                    setStackBagButton.Clicked += SetStackBagButtonCallBack;
                }
                if (ComponentManager.BM_WINDOW.FindView("stackButton", out ButtonBase stackButton))
                {
                    stackButton.Clicked += StackButtonCallBack;
                }
                if (ComponentManager.BM_WINDOW.FindView("forceBagButton", out ButtonBase forceBagButton))
                {
                    forceBagButton.Clicked += ForceBagButtonCallBack;
                }
                if (ComponentManager.BM_WINDOW.FindView("resetBagsButton", out ButtonBase resetBagsButton))
                {
                    resetBagsButton.Clicked += ResetBagsButtonCallBack;
                }
            }
        }

        public static void SetSelection(EquipSlot selection) 
        {
            ComponentManager.BM_SELECTION = selection;
        }

        public static EquipSlot GetSelection()
        {
            return ComponentManager.BM_SELECTION;
        }

        public static string GetStackBagId()
        {
            return ComponentManager.BM_FOCUS;
        }


        /// <summary>
        /// Set the given bag's value to true.
        /// </summary>
        /// <param name="bagIdentity"></param>
        public static bool SetStackBag(string bagIdentity, int numberOfOpenBags) {

            if (numberOfOpenBags>1) {
                logger.info($"Too many bags open, zone and try again.");
                return false;
            }

            if (bagList.ContainsKey(bagIdentity)) {
                bagList[bagIdentity] = true;
                ComponentManager.BM_FOCUS = bagIdentity;

                logger.info($"{bagIdentity} set to focus bag.");
                //Update the persisted copy.
                PersistContainers();
                return true;
            }

            logger.info($"{bagIdentity} could not be found in your baglist.");
            logger.info($"Try resetting your bag list and trying again.");
            return false;
        }

        /// <summary>
        /// Resets the current bagList and updates the persisted version
        /// </summary>
        public static void ResetBags()
        {
            bagList = new Dictionary<string, bool>();
            ComponentManager.BM_FOCUS = "<Not Focused>";
            logger.debug($"Updating bag list.");
            foreach (Backpack container in Inventory.Backpacks)
            {
                string key = container.Identity.ToString().Substring(11, 7);
                bool value = false;
                //logger.debug($"{key} : {value}");
                bagList.Add(key, value);
            }
            PersistContainers();
        }

        /// <summary>
        /// Writes the container dict to a txt file in csv format to load on inject.
        /// </summary>
        private static void PersistContainers()
        {
            logger.debug($"Writing List to file.");
            string stuff = string.Join("\n", bagList.Select(kv => kv.Key + "," + kv.Value).ToArray());
            File.WriteAllText(filePath, stuff);
        }

        public static void StackButtonCallBack(object sender, ButtonBase e)
        {
            if (ComponentManager.BM_STACKER_STATE == SimpleState.DISABLED)
            {
                logger.info($"Stacking Turned on!");
                ComponentManager.BM_STACKER_STATE = SimpleState.ENABLED;
            }
            else
            {
                logger.info($"Stacking Turned off!");
                ComponentManager.BM_STACKER_STATE = SimpleState.DISABLED;
            }
        }

        public static void ForceBagButtonCallBack(object sender, ButtonBase e)
        {
            NoDropTraderComponent.SetBags();
        }

        /// <summary>
        /// Would be kind of nice if this was just managed by a piece of state, such that any time we add/remove a bag to inv it updates...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ResetBagsButtonCallBack(object sender, ButtonBase e)
        {
            logger.info($"Reseting your bag focus.");
            ResetBags();
        }

        /// <summary>
        /// Set the currently opened bag to the "stack bag". By persisting this we dont have to worry about quarky work arounds each time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void SetStackBagButtonCallBack(object sender, ButtonBase e)
        {
            int openedContainers = 0;
            string bagId = "";
            foreach (Container container in Inventory.Backpacks)
            {
                if (container.IsOpen)
                {
                    openedContainers++;
                    bagId = container.Identity.ToString().Substring(11, 7);
                }
            }
            SetStackBag(bagId, openedContainers);
        }
    }

}
