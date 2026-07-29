using JxModule;
using UnityEngine;

namespace PNTD
{
    public class StagePath : MonoBehaviour
    {
        [BigHeader("Path Point")] [SerializeField]
        private Transform spawnPoint;

        [SerializeField] private Transform destinationPoint;
        [SerializeField] private Transform[] cornerPoints;

        public Transform SpawnPoint => spawnPoint;
        public Transform DestinationPoint => destinationPoint;
        public Transform[] CornerPoints => cornerPoints;

        public int PointCount
            => (spawnPoint != null && destinationPoint != null)
                ? cornerPoints.Length + 2
                : cornerPoints.Length;

        public Vector3 GetPointPosition(int index)
        {
            if (index == 0)
            {
                return spawnPoint.position;
            }

            if (index == PointCount - 1)
            {
                return destinationPoint.position;
            }

            return cornerPoints[index - 1].position;
        }

        public void SetPath(Transform newSpawnPoint, Transform newDestinationPoint, Transform[] newCornerPoints)
        {
            spawnPoint = newSpawnPoint;
            destinationPoint = newDestinationPoint;
            cornerPoints = newCornerPoints;
        }
    }
}