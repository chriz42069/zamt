using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZamTools.Services;

namespace ZamTools
{
    public class StatStacker
    {

        public static List<Item> characterItems = Inventory.Items;
        public static Container stackBag = Inventory.Backpacks.FirstOrDefault(x => x.IsOpen);
        public static ZamLogger logger = ZamLogger.GetLogger("StatStacker");
        public static string stackingItemName = "";


        public static void Stack(EquipSlot slotToStack)
        {
            if (BagManagerComponent.GetStackBagId().Equals(""))
            {
                logger.debug($"Could not find persisted bagId");
                logger.debug($"Using first or default");
            }
            else 
            {
                stackBag = Inventory.Backpacks.FirstOrDefault(x => x.Identity.ToString().Contains(BagManagerComponent.GetStackBagId()));
                logger.debug($"Focus: {BagManagerComponent.GetStackBagId()} :: StackSlot: {BagManagerComponent.GetSelection()}");
            }

            if (stackBag != null)
            {
                foreach (Item item in characterItems)
                {
                    //Look for an item equiped to either of the slots we want to stack
                    if ((int)slotToStack == item.Slot.Instance)
                    {
                        stackingItemName = item.Name;
                        logger.debug($"Found {item.Name} at {slotToStack}");
                        Identity stackBagId = stackBag.Identity;
                        Identity bank = new Identity();
                        bank.Type = IdentityType.BankByRef;
                        bank.Instance = (int)ComponentManager.BM_SELECTION;
                        StripItem(bank, stackBag);
                        return;
                    }

                    EquipItem(stackBag, slotToStack);
                }
            }
        }

        private static void StripItem(Identity bank, Container stackBag)
        {
            logger.debug($"Sending Item to => {stackBag.Identity.Instance}");

            Network.Send(new ClientContainerAddItem()
            {
                Target = stackBag.Identity,
                Source = bank
            });
        }

        private static void EquipItem(Container stackBag, EquipSlot slotToStack)
        {

            foreach (Item item in stackBag.Items)
            {
                item.Equip(slotToStack);
                break;
            }
        }
    }
}
