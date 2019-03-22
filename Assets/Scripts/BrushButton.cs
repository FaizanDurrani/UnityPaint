using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushButton : MonoBehaviour
{
    [SerializeField] private RawImage _preview;
    [SerializeField] private Button _button;

    public void SetBrush(Texture2D texture, DrawScript drawScript)
    {
        _preview.texture = texture;
        
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(()=>drawScript.SetBrush(texture));
    }
}
