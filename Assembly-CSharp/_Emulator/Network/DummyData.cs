using static MyInfoManager;

namespace _Emulator
{
    class DummyData
    {
        public int xp = 7000000;
        public int forcePoints = 100000000;
        public int brickPoints = 100000000;
        public int tokens = 100000000;
        public int coins = 100000000;
        public int starDust = 100000000;
        public int gm = 0;
        public int clanSeq = 0;
        public string clanName = "Clan";
        public int clanMark = 0;
        public int clanLv = 0;
        public int rank = 60;
        public int heavy = 0;
        public int assault = 0;
        public int sniper = 0;
        public int subMachine = 0;
        public int handGun = 0;
        public int melee = 0;
        public int special = 0;
        public sbyte tutorialed = 3;
        public int countryFilter = -1;
        public sbyte tos = 1;
        public int extraSlots = 0;
        public int firstLoginFp = 0;
        public int qjCommonMask =
                  (int)COMMON_OPT.DONOT_NEWBIE_CHANNEL_MSG
                | (int)COMMON_OPT.DONOT_BUNGEE_GUIDE
                | (int)COMMON_OPT.DONOT_MAPEDIT_GUIDE
                | (int)COMMON_OPT.DONOT_BND_GUIDE
                | (int)COMMON_OPT.DONOT_EXPLOSION_ATTACK_GUIDE
                | (int)COMMON_OPT.DONOT_EXPLOSION_DEFENCE_GUIDE
                | (int)COMMON_OPT.DONOT_BATTLE_GUIDE
                | (int)COMMON_OPT.DONOT_ZOMBIE_GUIDE
                | (int)COMMON_OPT.DONOT_FLAG_GUIDE
                | (int)COMMON_OPT.DONOT_DEFENSE_GUIDE
                | (int)COMMON_OPT.DONOT_ESCAPE_GUIDE;
        public int qjModeMask = 0;
        public int qjOfficialMask = 0;
        public static string[][] startingGear =
        {
            new string[] { "MAIN", "wau" },
            new string[] { "AUX", "wax" },
            new string[] { "BOMB", "wba" },
            new string[] { "MELEE", "wap" },
            new string[] { "UPPER", "aac" },
            new string[] { "LOWER", "aad" },
            new string[] { "BRICK-GUN", "s07" },
            new string[] { "NONE","sb7", "sb8", "sb9", "sc0", "sc1", "sc2", "sc3", "sc4", "s34", "s43", "s78", "s79" }
        };

    }
}
