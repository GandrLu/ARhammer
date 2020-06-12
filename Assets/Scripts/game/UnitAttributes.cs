using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace de.mp.future.warhammer.game
{
    [System.Serializable]
    public struct UnitAttributes
    {
        // For melee
        public int weaponSkill;
        // For ranged
        public int ballisticSkill;
        public int strength;
        public int toughness;
        public int attacks;
        // Armor or other possibilities that can save wounds
        public int save;
        // We will probably not use this, is to resist the will to flee
        public int leadership;
        // Lifepoints
        public int wounds;
        // Amount of seperate warriors in the unit
        public int troopSize;
        public bool hasRangedAttack;
    }
}