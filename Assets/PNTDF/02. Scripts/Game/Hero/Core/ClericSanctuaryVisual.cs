using UnityEngine;

namespace PNTD
{
    public class ClericSanctuaryVisual : MonoBehaviour
    {
        [SerializeField] private Transform rotationAxis;
        [SerializeField] private SpriteRenderer modelRenderer;
        [SerializeField] private CircleCollider2D radiusCollider;
        [SerializeField] private float rotationSpeed = 18f;

        private Vector3 _baseScale = Vector3.one;
        private float _baseRadius = 1f;
        private bool _hasBaseValues;

        public void Initialize()
        {
            CacheReferences();
            CacheBaseValues();

            if (radiusCollider != null)
            {
                radiusCollider.enabled = false;
            }
        }

        public void SetColor(Color color)
        {
            CacheReferences();

            if (modelRenderer != null)
            {
                modelRenderer.color = color;
            }
        }

        public void SetRadius(float radius)
        {
            CacheBaseValues();

            var scaleMultiplier = Mathf.Max(0f, radius) / _baseRadius;
            transform.localScale = _baseScale * scaleMultiplier;
        }

        private void Update()
        {
            CacheReferences();

            if (rotationAxis == null || Mathf.Approximately(rotationSpeed, 0f))
            {
                return;
            }

            rotationAxis.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        private void CacheReferences()
        {
            if (rotationAxis == null)
            {
                rotationAxis = transform.Find("Rotation Axis");
            }

            if (modelRenderer == null)
            {
                var modelTransform = transform.Find("Rotation Axis/Model");
                if (modelTransform == null)
                {
                    modelTransform = transform.Find("Model");
                }

                modelRenderer = modelTransform != null
                    ? modelTransform.GetComponent<SpriteRenderer>()
                    : GetComponentInChildren<SpriteRenderer>(true);
            }

            if (radiusCollider == null)
            {
                radiusCollider = GetComponent<CircleCollider2D>();
                if (radiusCollider == null)
                {
                    radiusCollider = GetComponentInChildren<CircleCollider2D>(true);
                }
            }
        }

        private void CacheBaseValues()
        {
            if (_hasBaseValues)
            {
                return;
            }

            CacheReferences();

            _baseScale = transform.localScale;
            _baseRadius = CalculateBaseRadius();
            _hasBaseValues = true;
        }

        private float CalculateBaseRadius()
        {
            if (radiusCollider == null)
            {
                return 1f;
            }

            var colliderScale = radiusCollider.transform.localScale;
            var maxScale = Mathf.Max(Mathf.Abs(colliderScale.x), Mathf.Abs(colliderScale.y));
            return Mathf.Max(0.001f, radiusCollider.radius * maxScale);
        }
    }
}
