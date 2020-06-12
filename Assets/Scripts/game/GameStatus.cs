using System.Collections;
using System.Collections.Generic;

namespace de.mp.future.warhammer.game
{
   public enum GameStatus
    {
        STARTUP,
        NEXTSTART,
        READY,
        MOVING,
        ATTACK,
        NEXTUNIT,
        NEXTATTACK,
        NEXTMOVE,
        FINISHED
    }
}