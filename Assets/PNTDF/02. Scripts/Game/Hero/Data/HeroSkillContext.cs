using UnityEngine;
using UnityEngine.Tilemaps;

namespace PNTD
{
    public class HeroSkillContext
    {
        public Tilemap BuildMap { get; } 
        public Transform Root { get; }

        public HeroSkillContext(Tilemap buildMap,
                                Transform root)
        {
            BuildMap = buildMap;
            Root = root;
        }
    }
}