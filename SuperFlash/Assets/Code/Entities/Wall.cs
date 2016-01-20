using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace COMP476Proj
{
    public class Wall : Entity
    {
        private bool seeThrough;
        public bool IsSeeThrough { get { return seeThrough; } }
        public Wall(Vector2 pos, BoundingRectangle boundRect, bool seeThrough = false)
        {
            this.pos = pos;
            this.rect = boundRect;
            this.seeThrough = seeThrough;
        }

        public override void ResolveCollision(Entity other)
        {
            throw new NotImplementedException();
        }
    }
}
