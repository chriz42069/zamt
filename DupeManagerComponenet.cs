using AOSharp.Common.GameData;
using AOSharp.Common.SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZamTools.Models;
using ZamTools.Services;

namespace ZamTools.Utils
{
    public class DupeManagerComponenet
    {
        public static ZamLogger logger = ZamLogger.GetLogger("DupeManager");
        public static DupeManagerComponenet instance = null;

        private DupeManagerComponenet() { logger.debug($"DupeManager initialized!"); }

        public static DupeManagerComponenet CreateInstance() 
        {
            if (instance == null) 
                instance = new DupeManagerComponenet();
            
            return instance;

        }

        public static void DestroyInstance()
        {
            logger.info($"Destroying DupeManager Services...");
            if (instance != null)
                instance = null;
        }

        /// <summary>
        /// State-full "componenents" here
        /// </summary>
        public static void Updates() 
        {
 
            if (ComponentManager.DUPER_STATE == SimpleState.ENABLED)
                DupeSelection();

            if (ComponentManager.DM_WINDOW.FindView("currentSelectionInput", out TextInputView currentSelectionInput))
            {
                if (ComponentManager.DM_WINDOW.FindView("currentSelectionLabel", out TextView currentSelectionLabel))
                {
                    try
                    {
                        string ret = "No Match";
                        Inventory.Items.ForEach(item =>
                        {
                            if (item.Name.Contains(currentSelectionInput.Text) && currentSelectionInput.Text.Length > 0)
                            {
                                ComponentManager.DM_SELECTION = (EquipSlot)item.Slot.Instance;
                                ret = $"Matching item @ {(EquipSlot)item.Slot.Instance}";
                                ComponentManager.DUPE_MANAGER_SEARCH_HIT = true;
                            }
                            else 
                            {
                                ComponentManager.DUPE_MANAGER_SEARCH_HIT = false;
                            }
                        });

                        //logger.debug(ret);
                    }
                    catch (Exception ex)
                    {
                        logger.debug($"Error when looking for item, check the spelling.");
                        logger.debug($"{ex.Message}");
                    }

                    currentSelectionLabel.Text = $"Search for Item: {currentSelectionInput.Text}";
                    ComponentManager.DM_ITEM_STRING = currentSelectionInput.Text;
                }
            }

            if (ComponentManager.DM_WINDOW.FindView("recipientInput", out TextInputView recipientInput))
            {
                if (ComponentManager.DM_WINDOW.FindView("currentRecipientLabel", out TextView currentRecipientLabel))
                {
                    currentRecipientLabel.Text = $"Recipient: {recipientInput.Text}";
                    ComponentManager.DM_RECIPIENT = recipientInput.Text;
                }
            }
            
            if (ComponentManager.DM_WINDOW.FindView("quantInput", out TextInputView quantInput) && quantInput.Text.Length > 0)
            {
                int _quant = 10;
                try
                {
                    _quant = int.Parse(quantInput.Text);

                }
                catch (Exception e)
                {
                    logger.error($"Only numbers please...");
                    logger.error($"{e.Message}");
                }

                if (ComponentManager.DM_WINDOW.FindView("currentQuantLabel", out TextView currentQuantLabel))
                {
                    currentQuantLabel.Text = $"Quantity: {_quant}";
                    ComponentManager.DUPE_QUANTITY = _quant;
                }
            }
        }

        /// <summary>
        /// State-less "componenents" here
        /// </summary>
        public static void Render()
        {
            ComponentManager.DM_WINDOW.Show(true);
            if (ComponentManager.DM_WINDOW.IsValid)
            {
                //Labels
                if (ComponentManager.DM_WINDOW.FindView("instructionLabel1", out TextView instructionLabel1))
                    instructionLabel1.Text = $"1. Head to a MailBox Terminal and stand close it.";
                if (ComponentManager.DM_WINDOW.FindView("instructionLabel2", out TextView instructionLabel2))
                    instructionLabel2.Text = $"2. Add a recipient and a number of times to dupe.";
                if (ComponentManager.DM_WINDOW.FindView("instructionLabel3", out TextView instructionLabel3))
                    instructionLabel3.Text = $"3. Copy and Paste the name of the item that you want to dupe. (It must be equiped!) ";

                //Buttons
                if (ComponentManager.DM_WINDOW.FindView("startDuperButton", out ButtonBase startDuperButton))
                    startDuperButton.Clicked += StartDuperButtonCallBack;
                
            }
        }

        public static void StartDuperButtonCallBack(object sender, ButtonBase e)
        {
            if (ComponentManager.DUPER_STATE == SimpleState.DISABLED)
            {
                logger.info($"Duping Turned on!");
                ComponentManager.DUPER_STATE = SimpleState.ENABLED;
            }
            else
            {
                logger.info($"Duping Turned off!");
                ComponentManager.DUPER_STATE = SimpleState.DISABLED;
            }
        }

        public static void DupeSelection()
        {
            if (!DynelManager.LocalPlayer.Cooldowns.ContainsKey(Stat.ComputerLiteracy))
            {
                //upper the first name of the recipient
                ComponentManager.DM_RECIPIENT = ComponentManager.DM_RECIPIENT.First().ToString().ToUpper() + ComponentManager.DM_RECIPIENT.Substring(1);

                //Create a dummy item for our forged packet
                Identity dupeItem = new Identity();
                dupeItem.Type = IdentityType.Inventory;
                dupeItem.Instance = (int)ComponentManager.DM_SELECTION;
                try
                {
                    Network.Send(new MailMessage()
                    {
                        Unknown1 = 06,
                        Recipient = ComponentManager.DM_RECIPIENT,
                        Subject = $"{ComponentManager.DM_ITEM_STRING}",
                        Body = "",
                        Item = dupeItem,
                        Credits = 40000,
                        Express = true
                    });
                    logger.debug($"Sending:{ ComponentManager.DM_ITEM_STRING} To: {ComponentManager.DM_RECIPIENT}, {ComponentManager.DUPE_QUANTITY} Times!");
                    ComponentManager.DUPE_COUNT++;
                }
                catch (Exception ex)
                {
                    logger.error($"{ex.Message}");
                }
            }
        }
    }
}
