using UnityEngine;
using UnityEngine.Tilemaps;

namespace PNTD
{
    public class TilemapEffect : MonoBehaviour
    {
        [Header("Tilemap Renderer")]
        [SerializeField] private TilemapRenderer tilemapRenderer;
        
        [Space(30f)]
        [Header("Settings")]
        [SerializeField] private string highlightPropertyName = "_Enabled";
        [SerializeField] private bool useInstanceMaterial = true;

        private Material _runtimeMaterial;
        private int _highlightPropertyId;

        private void Awake()
        {
            _highlightPropertyId = Shader.PropertyToID(highlightPropertyName);
            
            tilemapRenderer ??= GetComponent<TilemapRenderer>();
            if (tilemapRenderer == null)
            {
                Debug.LogError("TilemapHighlighter: tilemapRenderer is null.");
                enabled = false;
                return;
            }

            _runtimeMaterial = useInstanceMaterial ? tilemapRenderer.material 
                : tilemapRenderer.sharedMaterial;
            if (_runtimeMaterial == null)
            {
                Debug.LogError("TilemapHighlighter: runtime material is null.");
                enabled = false;
                return;
            }

            SetHighlight(false);
        }

        public void SetHighlight(bool isOn)
        {
            _runtimeMaterial?.SetFloat(_highlightPropertyId, isOn ? 1f : 0f);
        }
    }
}