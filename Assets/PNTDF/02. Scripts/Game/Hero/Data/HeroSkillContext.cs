using UnityEngine;
using UnityEngine.Tilemaps;

namespace PNTD
{
    public class HeroSkillContext
    {
        public Tilemap BuildMap { get; } 
        public Transform Root { get; }
        public BoardSystem BoardSystem { get; }
        public IGoldSpawner GoldSpawner { get; }
        public HeroFactory HeroFactory { get; private set; }

        public HeroSkillContext(Tilemap buildMap,
                                Transform root,
                                BoardSystem boardSystem,
                                IGoldSpawner goldSpawner = null)
        {
            BuildMap = buildMap;
            Root = root;
            BoardSystem = boardSystem;
            GoldSpawner = goldSpawner;
        }
        
        public void SetHeroFactory(HeroFactory heroFactory)
        {
            HeroFactory = heroFactory;
        }
    }
}
