using AOSharp.Common.GameData;

namespace ZamTools.Models
{

    public class Config
    {
        //logging
        public static LogLevel LOG_LEVEL = LogLevel.DEBUG;
        public static LogStyle LOG_STYLE = LogStyle.VERBOSE;

        //BM
        public static string BAG_MANAGER_FOCUS = "<Not Focused>";
        public static EquipSlot BAG_MANAGER_SELECTION = EquipSlot.Weap_Hud3;
        public static SimpleState STACKER_STATE = SimpleState.DISABLED;

        //DM
        public static EquipSlot DUPE_MANAGER_SELECTION = EquipSlot.Cloth_Head;
        public static bool DUPE_MANAGER_SEARCH_HIT = false;
        public static string RECIPIENT = "<Not Specified>";
        public static SimpleState DUPER_STATE = SimpleState.DISABLED;
        public static int DUPE_QUANTITY = 10;
        public static int DUPE_COUNT = 0;

    }
}
