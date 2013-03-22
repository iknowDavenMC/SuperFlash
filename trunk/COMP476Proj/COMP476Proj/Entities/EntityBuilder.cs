using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class EntityBuilder
    {
        private static EntityBuilder instance = null;

        private EntityBuilder() { }

        public static EntityBuilder getInstance()
        {
            if (instance == null)
            {
                instance = new EntityBuilder();
            }
            return instance;
        }

        public static Streaker buildStreaker()
        {
            Streaker s = new Streaker();
            s.physics = new PhysicsComponent();
            s.draw = new DrawComponent(SpriteDatabase.GetAnimation("streaker_static"), Color.White, Vector2.Zero, new Vector2(.5f, .5f), .5f, 150);
            return s;
        }

    }
}
