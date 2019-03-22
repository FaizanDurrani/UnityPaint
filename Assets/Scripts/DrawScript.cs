using UnityEngine;
using UnityEngine.UI;

public class DrawScript : MonoBehaviour
{
    private Texture2D _initialTexture;
    [SerializeField] private Texture2D _defaultTex;
    [SerializeField] private Material _blitMaterial;
    [SerializeField] private RenderTexture _renderTexture;
    [SerializeField] private int _diameter = 12;
    [SerializeField] private Color _color;
    [SerializeField] private RawImage _image;
    [SerializeField] private Text _sizeText;

    [SerializeField] private GameObject _debugCircle;
    [SerializeField] private Transform _debugParent;

    private RenderTexture _bufferTexture;
    private float _lastUpdate;
    private Vector2 _lastMPos;
    private Vector2Int TextureDimensions => new Vector2Int(_renderTexture.width, _renderTexture.height);

    public void SetBrush(Texture2D texture)
    {
        _blitMaterial.SetTexture("_BrushTex", texture);
    }
    
    public void SetSize(float size)
    {
        _diameter = (int) size;
        _sizeText.text = "Size: " + _diameter;
    }

    public void SetColor(Color color)
    {
        _color = color;
    }

    private void Start()
    {
        _initialTexture = new Texture2D(_renderTexture.width, _renderTexture.height);
        var pixels = _initialTexture.GetPixels32();
        for (int i = 0; i < TextureDimensions.x * TextureDimensions.y; i++)
        {
            pixels[i] = new Color32(255, 255, 255, 255);
        }

        _initialTexture.SetPixels32(pixels);
        _initialTexture.Apply();

        Graphics.Blit(_initialTexture, _renderTexture);
        
        _bufferTexture = RenderTexture.GetTemporary(_initialTexture.width, _initialTexture.height);

        SetBrush(_defaultTex);
    }

    private void UpdateTexture()
    {
        Graphics.Blit(_renderTexture, _bufferTexture, _blitMaterial);
        Graphics.Blit(_bufferTexture, _renderTexture);
    }

    private void Stamp(Vector2Int p)
    {
        var radius = _diameter / 2f;
        var size = new Vector2Int(_diameter, _diameter);
        var o = new Vector2(radius, radius);
        _blitMaterial.SetVector("_size", (Vector2) size);
        _blitMaterial.SetVector("_sPos", p - o);
        _blitMaterial.SetColor("_color", _color);
        UpdateTexture();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 texturePos;
            if (!GetPixelByMousePosition(Input.mousePosition, out texturePos)) return;
            Stamp(new Vector2Int(Mathf.CeilToInt(texturePos.x), Mathf.CeilToInt(texturePos.y)));
            _lastMPos = texturePos;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 texturePos;
            if (!GetPixelByMousePosition(Input.mousePosition, out texturePos) || texturePos == _lastMPos) return;
            Stamp(new Vector2Int(Mathf.CeilToInt(texturePos.x), Mathf.CeilToInt(texturePos.y)));

            var distance = Vector2.Distance(texturePos, _lastMPos);
            var f = _diameter * 0.25f;
            if (distance >= f)
            {
                var divs = distance / f;
                for (int i = 0; i < divs; i++)
                {
                    Stamp(new Vector2Int(Mathf.CeilToInt(
                                                         Mathf.Lerp(_lastMPos.x, texturePos.x, i / divs)),
                                         Mathf.CeilToInt(
                                                         Mathf.Lerp(_lastMPos.y, texturePos.y, i / divs)))
                         );
                }
            }

            _lastMPos = texturePos;
        }
    }

    private bool GetPixelByMousePosition(Vector2 mPos, out Vector2 texturePos)
    {
        Vector2 localCursor;
        var iRect = _image.GetComponent<RectTransform>();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(iRect, mPos, null, out localCursor))
        {
            texturePos = Vector2.zero;
            return false;
        }

        var mappedSize = localCursor / iRect.rect.size;
        texturePos = TextureDimensions * mappedSize;
        return true;
    }
}