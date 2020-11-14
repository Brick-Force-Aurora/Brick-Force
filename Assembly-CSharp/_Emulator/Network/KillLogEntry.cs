using System.Collections.Generic;

namespace _Emulator
{
    class KillLogEntry
    {
        public int id;
        public sbyte killerType;
        public int killer;
        public sbyte victimType;
        public int victim;
        public Weapon.BY weaponBy;
        public int slot;
        public int category;
        public int hitpart;
        public Dictionary<int, int> damageLog;
        public KillLogEntry(int _id, sbyte _killerType, int _killer, sbyte _victimType, int _victim, Weapon.BY _weaponBy, int _slot, int _category, int _hitpart, Dictionary<int, int> _damageLog)
        {
            id = _id;
            killerType = _killerType;
            killer = _killer;
            victimType = _victimType;
            victim = _victim;
            weaponBy = _weaponBy;
            slot = _slot;
            category = _category;
            hitpart = _hitpart;
            damageLog = _damageLog;
        }
    }
}
