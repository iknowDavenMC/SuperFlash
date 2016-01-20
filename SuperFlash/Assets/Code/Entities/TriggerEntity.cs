using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Assets.Code._XNA;

namespace COMP476Proj
{
    public class TriggerEntity : Entity
    {
        private bool triggered;
        private int id;
        public int ID { get { return id; } }
        private static int count = 0;
        public TriggerEntity(int x, int y, int w, int h)
        {
            triggered = false;
            rect = new BoundingRectangle(x, y, w, h);
            id = ++count;
        }
        public TriggerEntity(int x, int y, int w, int h, int id)
        {
            triggered = false;
            rect = new BoundingRectangle(x, y, w, h);
            this.id = id;
            if (count < id)
                count = id + 1;
        }
        public override void ResolveCollision(Entity entity)
        {
            if (!triggered)
            {
                triggered = true;
            }
        }
        public bool hasTriggered()
        {
            return triggered;
        }

        public void setTriggered()
        {
            triggered = true;
        }

        public void clearTriggered()
        {
            triggered = false;
        }
    }
}
