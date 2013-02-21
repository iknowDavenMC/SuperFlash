#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Superflash
{
    public static class SpriteDatabase
    {
        /*-------------------------------------------------------------------------*/
        #region Fields 
        
        private static SortedDictionary<string, Animation> animations = new SortedDictionary<string, Animation>();

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Animation Insertion, Deletion

        public static Animation AddAnimation(Animation a)
        {
            if (!HasAnimation(a.AnimationId))
            {
                animations.Add(a.AnimationId, a);
                return a;
            }
            return null;
        }

        public static void RemoveAnimation(String animId)
        {
            animations.Remove(animId);
        }

        public static void RemoveAllAnimations()
        {
            animations.Clear();
        }

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Animation Query

        public static Animation GetAnimation(String name)
        {
            if (HasAnimation(name))
            {
                return animations[name];
            }
            return null;
        }

        public static bool HasAnimation(String name)
        {
            return animations.ContainsKey(name);
        }

        #endregion
    }
}
