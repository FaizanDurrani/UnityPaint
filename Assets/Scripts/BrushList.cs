using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushList : MonoBehaviour
{

    [SerializeField] private Texture2D[] _brushes;

    [SerializeField] private Transform _list;
    [SerializeField] private BrushButton _buttonPrefab;
    [SerializeField] private DrawScript _drawScript;

    private void Start()
    {
        foreach (var brush in _brushes)
        {
            var button = Instantiate(_buttonPrefab.gameObject, _list).GetComponent<BrushButton>();
            button.SetBrush(brush, _drawScript);
        }
    }
}
