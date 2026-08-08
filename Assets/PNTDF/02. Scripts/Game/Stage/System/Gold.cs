using System;
using JxModule;
using UnityEngine;

namespace PNTD
{
    public class Gold : MonoBehaviour
    {
        [SerializeField] private Collider2D clickCollider;

        private Action<Gold, int> _onCollected;
        private int _goldAmount;
        private bool _isCollected;

        public void Initialize(int goldAmount, Action<Gold, int> onCollected)
        {
            _goldAmount = Mathf.Max(1, goldAmount);
            _onCollected = onCollected;
            _isCollected = false;

            CacheReferences();
            if (clickCollider != null)
            {
                clickCollider.enabled = true;
            }

            gameObject.SetActive(true);
        }

        public void Collect()
        {
            if (_isCollected)
            {
                return;
            }

            _isCollected = true;
            _onCollected?.Invoke(this, _goldAmount);
            ReturnToPool();
        }

        private void OnMouseDown()
        {
            SoundManager.Instance.PlaySFX("SFX_Coin");
            Collect();
        }

        private void CacheReferences()
        {
            if (clickCollider == null)
            {
                clickCollider = GetComponentInChildren<Collider2D>(true);
            }
        }

        private void ReturnToPool()
        {
            _onCollected = null;
            _goldAmount = 0;
            if (clickCollider != null)
            {
                clickCollider.enabled = false;
            }

            ObjectPoolManager.Instance.Return(gameObject);
        }

        private void OnDisable()
        {
            _onCollected = null;
            _isCollected = false;
        }
    }
}
