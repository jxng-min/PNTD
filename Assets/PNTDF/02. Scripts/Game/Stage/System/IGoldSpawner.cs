using UnityEngine;

namespace PNTD
{
    public interface IGoldSpawner
    {
        void Spawn(Vector3 center, int goldAmount, float minOffset = 0.15f, float maxOffset = 0.35f);
    }
}
