using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace COMP476Proj
{
    public class Flock
    {
        private int flockDist = 300;
        public List<NPC> Members;

        private float alignmentWeight = 0f;
        private float cohesionWeight = 8f;
        private float separationWeight = 8f;

        public Flock()
        {
            Members = new List<NPC>();
        }

        public float AlignmentWeight
        {
            get { return alignmentWeight; }
            set
            {
                if (value < -1f)
                    value = -1f;
                if (value > 1f)
                    value = 1f;
                alignmentWeight = value;
            }
        }

        public float CohesionWeight
        {
            get { return cohesionWeight; }
            set
            {
                if (value < -1f)
                    value = -1f;
                if (value > 1f)
                    value = 1f;
                cohesionWeight = value;
            }
        }

        public float SeparationWeight
        {
            get { return separationWeight; }
            set
            {
                if (value < -1f)
                    value = -1f;
                if (value > 1f)
                    value = 1f;
                separationWeight = value;
            }
        }

        public Vector2 computeAlignment(NPC member)
        {
            Vector2 v = new Vector2();
            int neighbourCount = 0;
            foreach (NPC npc in Members)
            {
                if (npc != member)
                {
                    float distSq = (npc.Position - member.Position).LengthSquared();
                    if (distSq < flockDist * flockDist)
                    {
                        v += npc.ComponentPhysics.Velocity;
                        ++neighbourCount;
                    }
                }
            }
            if (neighbourCount == 0 || v.LengthSquared() == 0)
                return v;
            v /= neighbourCount;
            v.Normalize();
            return v * alignmentWeight;
        }

        public Vector2 computeCohesion(NPC member)
        {
            Vector2 v = new Vector2();
            int neighbourCount = 0;
            foreach (NPC npc in Members)
            {
                if (npc != member)
                {
                    float distSq = (npc.Position - member.Position).LengthSquared();
                    if (distSq < flockDist * flockDist)
                    {
                        v += npc.ComponentPhysics.Position;
                        ++neighbourCount;
                    }
                }
            }
            if (neighbourCount == 0 || v.LengthSquared() == 0)
                return v;
            v /= neighbourCount;
            v -= member.Position;
            v.Normalize();
            return v * cohesionWeight;
        }

        public Vector2 computeSeparation(NPC member)
        {
            Vector2 v = new Vector2();
            int neighbourCount = 0;
            foreach (NPC npc in Members)
            {
                if (npc != member)
                {
                    float distSq = (member.Position - npc.Position).LengthSquared();
                    if (distSq < flockDist * flockDist)
                    {
                        v += member.ComponentPhysics.Position - npc.ComponentPhysics.Position;
                        ++neighbourCount;
                    }
                }
            }
            if (neighbourCount == 0 || v.LengthSquared() == 0)
                return v;
            v /= neighbourCount;
            v.Normalize();
            return v * separationWeight;
        }
    }
}
