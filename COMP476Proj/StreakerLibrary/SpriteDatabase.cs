#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace StreakerLibrary
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

        #region Load Content

        public static void loadSprites(ContentManager Content)
        {
            Texture2D streaker = Content.Load<Texture2D>("streaker");
            Texture2D cop = Content.Load<Texture2D>("cop");
            Texture2D student1 = Content.Load<Texture2D>("student1");
            Texture2D student2 = Content.Load<Texture2D>("student2");
            Texture2D student3 = Content.Load<Texture2D>("student3");
            Texture2D level1 = Content.Load<Texture2D>("levelDesign_Full");
            //Hud elements
            Texture2D banner = Content.Load<Texture2D>("Hud/banner");
            Texture2D notorietyBar = Content.Load<Texture2D>("Hud/notorietyBar");
            Texture2D notorietyMeter = Content.Load<Texture2D>("Hud/notorietyMeter");

            Texture2D achieveBanner = Content.Load<Texture2D>("AchievementUnlocked");

            //Animation elements 
            int notMoving = 150, walk = 60, fall = 60, getUp = 100, dance = 100, attack = 50, flash = 60;

            SpriteDatabase.AddAnimation(new Animation("streaker_static", streaker, 2, 143, 184, 0, notMoving));
            SpriteDatabase.AddAnimation(new Animation("streaker_walk", streaker, 5, 143, 184, 184, walk));
            SpriteDatabase.AddAnimation(new Animation("streaker_fall", streaker, 7, 143, 184, 368, fall));
            SpriteDatabase.AddAnimation(new Animation("streaker_getup", streaker, 7, 143, 184, 552, getUp));
            SpriteDatabase.AddAnimation(new Animation("streaker_dance", streaker, 5, 143, 184, 736, dance));
            SpriteDatabase.AddAnimation(new Animation("streaker_flash", streaker, 8, 143, 184, 920, flash));

            SpriteDatabase.AddAnimation(new Animation("cop_static", cop, 2, 143, 184, 0, notMoving));
            SpriteDatabase.AddAnimation(new Animation("cop_walk", cop, 5, 143, 184, 184, walk));
            SpriteDatabase.AddAnimation(new Animation("cop_fall", cop, 7, 143, 184, 368, fall));
            SpriteDatabase.AddAnimation(new Animation("cop_getup", cop, 7, 143, 184, 552, getUp));
            SpriteDatabase.AddAnimation(new Animation("cop_attack", cop, 5, 143, 184, 736, attack));

            SpriteDatabase.AddAnimation(new Animation("student1_static", student1, 2, 137, 155, 0, notMoving));
            SpriteDatabase.AddAnimation(new Animation("student1_walk", student1, 5, 137, 155, 155, walk));
            SpriteDatabase.AddAnimation(new Animation("student1_fall", student1, 7, 137, 155, 310, fall));
            SpriteDatabase.AddAnimation(new Animation("student1_getup", student1, 7, 137, 155, 465, getUp));

            SpriteDatabase.AddAnimation(new Animation("student2_static", student2, 2, 137, 155, 0, notMoving));
            SpriteDatabase.AddAnimation(new Animation("student2_walk", student2, 5, 137, 155, 155, walk));
            SpriteDatabase.AddAnimation(new Animation("student2_fall", student2, 7, 137, 155, 310, fall));
            SpriteDatabase.AddAnimation(new Animation("student2_getup", student2, 7, 137, 155, 465, getUp));

            SpriteDatabase.AddAnimation(new Animation("student3_static", student3, 2, 145, 164, 0, notMoving));
            SpriteDatabase.AddAnimation(new Animation("student3_walk", student3, 5, 145, 164, 164, walk));
            SpriteDatabase.AddAnimation(new Animation("student3_fall", student3, 7, 145, 164, 328, fall));
            SpriteDatabase.AddAnimation(new Animation("student3_getup", student3, 7, 145, 164, 492, getUp));

            SpriteDatabase.AddAnimation(new Animation("level_1", level1));

            SpriteDatabase.AddAnimation(new Animation("achievement_banner", achieveBanner));

        }

        #endregion
    }
}
