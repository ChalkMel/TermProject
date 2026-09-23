using UnityEngine;

namespace _Source.TowersSystem
{
    public class RangeIndicator : MonoBehaviour
    {
        [SerializeField] private int segments = 64;
        [SerializeField] private float lineWidth = 0.08f;
        [SerializeField] private int sortingOrder = 100;
        [SerializeField] private Material lineMaterial;

        private LineRenderer _lr;
        private Color _color = new Color(1f, 1f, 0f, 0.7f);

        private void Awake()
        {
            EnsureRenderer();
            Hide();
        }

        public void SetColor(Color color)
        {
            _color = color;
            if (_lr != null)
            {
                _lr.startColor = color;
                _lr.endColor = color;
            }
        }

        public void Show(Vector3 worldPos, float radius)
        {
            EnsureRenderer();
            transform.position = worldPos;
            UpdateCircle(radius);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }

        private void EnsureRenderer()
        {
            if (_lr != null) return;

            _lr = GetComponent<LineRenderer>();
            if (_lr == null) _lr = gameObject.AddComponent<LineRenderer>();

            _lr.useWorldSpace = false;
            _lr.loop = true;
            _lr.positionCount = Mathf.Max(8, segments);
            _lr.startWidth = lineWidth;
            _lr.endWidth = lineWidth;
            _lr.startColor = _color;
            _lr.endColor = _color;
            _lr.sortingOrder = sortingOrder;
            _lr.numCornerVertices = 2;
            _lr.numCapVertices = 2;

            if (lineMaterial != null) _lr.material = lineMaterial;
            else _lr.material = new Material(Shader.Find("Sprites/Default"));
        }

        private void UpdateCircle(float radius)
        {
            int count = _lr.positionCount;
            for (int i = 0; i < count; i++)
            {
                float angle = i * Mathf.PI * 2f / count;
                _lr.SetPosition(i, new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius);
            }
        }
    }
}