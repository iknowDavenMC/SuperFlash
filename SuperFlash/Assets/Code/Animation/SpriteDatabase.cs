#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;
#endregion

namespace StreakerLibrary
{
    public static class SpriteDatabase
    {
        /*-------------------------------------------------------------------------*/
        #region Fields 
        
        private static SortedDictionary<string, CustomAnimation> animations = new SortedDictionary<string, CustomAnimation>();

        #endregion

        /*-------------------------------------------------------------------------*/
        #region Animation Insertion, Deletion

        public static CustomAnimation AddAnimation(CustomAnimation a)
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

        public static CustomAnimation GetAnimation(String name)
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

				/*
        public static void loadSprites(ContentManager Content)
        {
            Texture2D streaker = Content.Load<Texture2D>("streaker");
            Texture2D cop = Content.Load<Texture2D>("cop");
            Texture2D smartCop = Content.Load<Texture2D>("smartCop");
            Texture2D roboCop = Content.Load<Texture2D>("roboCop");
            Texture2D student1 = Content.Load<Texture2D>("student1");
            Texture2D student2 = Content.Load<Texture2D>("student2");
            Texture2D student3 = Content.Load<Texture2D>("student3");
            Texture2D level1 = Content.Load<Texture2D>("levelDesign_Full");
            //Hud elements
            Texture2D banner = Content.Load<Texture2D>("Hud/banner");
            Texture2D notorietyBar = Content.Load<Texture2D>("Hud/notorietyBar");
            Texture2D notorietyMeter = Content.Load<Texture2D>("Hud/notorietyMeter");

            Texture2D achieveBanner = Content.Load<Texture2D>("AchievementUnlocked");
            Texture2D pwr_mass = Content.Load<Texture2D>("pwr_mass");
            Texture2D pwr_slick = Content.Load<Texture2D>("pwr_slick");
            Texture2D pwr_speed = Content.Load<Texture2D>("pwr_speed");
            Texture2D pwr_turn = Content.Load<Texture2D>("pwr_turn");
            Texture2D superFlashIcon = Content.Load<Texture2D>("Hud/superFlashButton");

            //Animation elements 
            int notMoving = 150, walk = 60, fall = 60, getUp = 100, dance = 100, attack = 100, flash = 60, slowWalk = 100;

            SpriteDatabase.AddAnimation(new CustomAnimation("streaker_static", streaker, 2, 143, 184, 0, notMoving));
            SpriteDatabase.AddAnimation(new CustomAnimation("streaker_walk", streaker, 5, 143, 184, 184, walk));
            SpriteDatabase.AddAnimation(new CustomAnimation("streaker_fall", streaker, 7, 143, 184, 368, fall));
            SpriteDatabase.AddAnimation(new CustomAnimation("streaker_getup", streaker, 7, 143, 184, 552, getUp));
            SpriteDatabase.AddAnimation(new CustomAnimation("streaker_dance", streaker, 5, 143, 184, 736, dance));
            SpriteDatabase.AddAnimation(new CustomAnimation("streaker_flash", streaker, 8, 143, 184, 920, flash));

            SpriteDatabase.AddAnimation(new CustomAnimation("cop_static", cop, 2, 143, 183, 0, notMoving));
            SpriteDatabase.AddAnimation(new CustomAnimation("cop_walk", cop, 5, 143, 183, 183, walk));
            SpriteDatabase.AddAnimation(new CustomAnimation("cop_fall", cop, 7, 143, 183, 366, fall));
            SpriteDatabase.AddAnimation(new CustomAnimation("cop_getup", cop, 7, 143, 183, 549, getUp));
            SpriteDatabase.AddAnimation(new CustomAnimation("cop_attack", cop, 4, 143, 183, 732, attack));

            SpriteDatabase.AddAnimation(new CustomAnimation("smartCop_static", smartCop, 2, 143, 183, 0, notMoving));
            SpriteDatabase.AddAnimation(new CustomAnimation("smartCop_walk", smartCop, 5, 143, 183, 183, walk));
            SpriteDatabase.AddAnimation(new CustomAnimation("smartCop_fall", smartCop, 7, 143, 183, 366, fall));
            SpriteDatabase.AddAnimation(new CustomAnimation("smartCop_getup", smartCop, 7, 143, 183, 549, getUp));
            SpriteDatabase.AddAnimation(new CustomAnimation("smartCop_attack", smartCop, 4, 143, 183, 732, attack));

            SpriteDatabase.AddAnimation(new CustomAnimation("roboCop_static", roboCop, 2, 137, 158, 0, notMoving));
            SpriteDatabase.AddAnimation(new CustomAnimation("roboCop_walk", roboCop, 5, 137, 158, 158, walk));
            SpriteDatabase.AddAnimation(new CustomAnimation("roboCop_attack", roboCop, 4, 137, 158, 316, attack));

            SpriteDatabase.AddAnimation(new CustomAnimation("student1_static", student1, 2, 137, 155, 0, notMoving));
            SpriteDatabase.AddAnimation(new CustomAnimation("student1_flee", student1, 5, 137, 155, 155, walk));
            SpriteDatabase.AddAnimation(new CustomAnimation("student1_fall", student1, 6, 137, 155, 310, fall));
            SpriteDatabase.AddAnimation(new CustomAnimation("student1_getup", student1, 7, 137, 155, 465, getUp));
            SpriteDatabase.AddAnimation(new CustomAnimation("student1_walk", student1, 4, 137, 155, 620, slowWalk));

            SpriteDatabase.AddAnimation(new CustomAnimation("student2_static", student2, 2, 137, 152, 0, notMoving));
            SpriteDatabase.AddAnimation(new CustomAnimation("student2_flee", student2, 5, 137, 152, 152, walk));
            SpriteDatabase.AddAnimation(new CustomAnimation("student2_fall", student2, 8, 137, 152, 304, fall));
            SpriteDatabase.AddAnimation(new CustomAnimation("student2_getup", student2, 7, 137, 152, 456, getUp));
            SpriteDatabase.AddAnimation(new CustomAnimation("student2_walk", student2, 4, 137, 152, 608, slowWalk));

            SpriteDatabase.AddAnimation(new CustomAnimation("student3_static", student3, 2, 145, 164, 0, notMoving));
            SpriteDatabase.AddAnimation(new CustomAnimation("student3_flee", student3, 5, 145, 164, 164, walk));
            SpriteDatabase.AddAnimation(new CustomAnimation("student3_fall", student3, 8, 145, 164, 328, fall));
            SpriteDatabase.AddAnimation(new CustomAnimation("student3_getup", student3, 7, 145, 164, 492, getUp));
            SpriteDatabase.AddAnimation(new CustomAnimation("student3_walk", student3, 4, 145, 164, 656, slowWalk));

            SpriteDatabase.AddAnimation(new CustomAnimation("level_1", level1));

            SpriteDatabase.AddAnimation(new CustomAnimation("achievement_banner", achieveBanner));
            SpriteDatabase.AddAnimation(new CustomAnimation("superFlashIcon", superFlashIcon));


            SpriteDatabase.AddAnimation(new CustomAnimation("pwr_mass", pwr_mass));
            SpriteDatabase.AddAnimation(new CustomAnimation("pwr_slick", pwr_slick));
            SpriteDatabase.AddAnimation(new CustomAnimation("pwr_speed", pwr_speed));
            SpriteDatabase.AddAnimation(new CustomAnimation("pwr_turn", pwr_turn));

            

        }
				 * */

        #endregion
    }
}
