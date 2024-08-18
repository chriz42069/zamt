using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.Inventory;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using ZamTools.Services;

namespace ZamTools.Utils
{
    public class NoDropTraderComponent
    {
        private static ZamLogger logger = ZamLogger.GetLogger("NoDropTrader");

        private static bool innerBagFound = false;
        private static bool outerBagFound = false;
        private static NoDropTraderComponent instance = null;

        private NoDropTraderComponent() 
        {
            logger.debug($"NoDropTraderComponent initialized!");
        }

        public static NoDropTraderComponent CreateInstance() 
        {
            if (instance == null) 
                instance = new NoDropTraderComponent();

            return instance;
        }

        public static void DestroyInstance()
        {
            logger.info($"Destroying NoDropTraderComponent Services...");
            if (instance != null)
                instance = null;
        }

        public static void SetBags() 
        {
            Backpack innerBag = null;
            Backpack outerBag = null;
            logger.debug($"Searching for bags....");

            //Get the outer bag ( should be in the back slot ) 
            foreach (Backpack backpack in Inventory.Backpacks)
            {
                //logger.debug($"Comparing {backpack.Slot.Instance} => {(int)EquipSlot.Social_Back}");
                if (backpack.Slot.Instance == (int)EquipSlot.Social_Back)
                {
                    logger.debug($"Found bag instance. Assigned it to the Inner bag!");
                    innerBag = backpack;
                    innerBagFound = true;
                }
            }

            //Find the instance of the focus bag
            foreach (Backpack backpack in Inventory.Backpacks)
            {
                //logger.debug($"Comparing {BagManager.GetStackBagId()} => {backpack.Identity}");
                if (ComponentManager.BM_FOCUS == backpack.Identity.ToString().Substring(11,7))
                {
                    logger.debug($"Found bag instance. Assigned it to the Outer bag!");
                    outerBag = backpack;
                    outerBagFound = true;
                }
            }

            if (innerBagFound && outerBagFound)
            {
                logger.debug($"Forcing {innerBag.Identity} into {outerBag.Identity}");

                Identity bank = new Identity();
                bank.Instance = (int)EquipSlot.Social_Back;
                bank.Type = IdentityType.BankByRef;

                Network.Send(new ClientContainerAddItem()
                {
                    Target = outerBag.Identity,
                    Source = bank
                });
            }
            else
            {
                logger.info($"Something went wrong, make sure you have a bag on your social bag slot.");
            }
        }

    }
}
