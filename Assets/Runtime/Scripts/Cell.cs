using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] private Sprite _xIcon; // index = 1
    [SerializeField] private Sprite _oIcon; // index = 2
    [SerializeField] private Image _cellImage;
    private Button _cellButton;

    void Start()
    {
        if (_xIcon == null || _oIcon == null)
        {
            Debug.LogError("Sprite icon empty");
            return;
        }
        _cellButton = GetComponent<Button>();
        _cellButton.onClick.AddListener(() => handleOnClick(1));
                
    }

    void Update()
    {

    }
    
    #region handle button
    private void handleOnClick(int index)
    {
        if (index == 1)
        {
            _cellImage.sprite = _xIcon;
        }
        else
        {
            _cellImage.sprite = _xIcon;
        }
    }
    #endregion

}
