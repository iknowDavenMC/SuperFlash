using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace COMP476Proj
{
    public class Wall : Entity
    {
        public Wall(Vector2 pos, BoundingRectangle boundRect)
        {
            this.pos = pos;
            this.rect = boundRect;
        }

        public override void ResolveCollision(Entity other)
        {
            throw new NotImplementedException();
        }
    }
}
