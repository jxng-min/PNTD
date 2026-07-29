using UnityEngine;

namespace PNTD
{
    public class MapFactory : MonoBehaviour
    {
        [SerializeField] private Transform mapRoot;

        public StageMap Create(GameObject mapPrefab)
        {
            var mapObject = Instantiate(mapPrefab, mapRoot);
            return mapObject.GetComponent<StageMap>();
        }

        public void Release(StageMap map)
        {
            Destroy(map.gameObject);
        }
    }
}