using JxModule;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PNTD
{
    public class StageMap : MonoBehaviour
    {
        [BigHeader("References")]
        [SerializeField] private StagePath stagePath;
        [SerializeField] private TilemapEffect tilemapEffect;
        [SerializeField] private Tilemap buildableTilemap;
        
        public StagePath StagePath => stagePath;
        public TilemapEffect MapEffect => tilemapEffect;
        public Tilemap BuildMap => buildableTilemap;
    }
}