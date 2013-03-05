using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COMP476Proj
{
    public enum CharacterState { STATIC, WALK_LEFT, WALK_RIGHT, FALL, GET_UP, DANCE }
    public class IntelligenceComponent
    {
        public CharacterState charState = CharacterState.STATIC;

        public IntelligenceComponent()
        {
        }


    }
}
