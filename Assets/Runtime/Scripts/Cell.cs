using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [SerializeField] private Sprite _xIcon; // value = 1
    [SerializeField] private Sprite _oIcon; // value = 2
    [SerializeField] private Image _cellImage;
    [SerializeField] private bool _isMarket = false;
        
    private Button _cellButton;
    private int _x, _y;

    void Start()
    {
        if (_xIcon == null || _oIcon == null)
        {
            Debug.LogError("Sprite icon empty");
            return;
        }
        _cellButton = GetComponent<Button>();
        _cellButton.onClick.AddListener(() => handleOnClick());
    }

    #region Init data
    public void initXY(int x, int y)
    {
        _x = x;
        _y = y;
    } 
    #endregion
    
    #region handle click
    
    private void handleOnClick()
    {
        if (_isMarket) return;
        int value = GameController.Instance.GetTurn % 2 == 0 ? 1 : 2;
        Debug.LogWarning(value);
        GameController.Instance.SetCell(_x, _y, value);
        setMark(value);
        //Debug.Log($"Postion {_x},{_y}");
    }

    public void setMark(int value)
    {
        _isMarket = true;
        if (value == 1)
        {
            _cellImage.sprite = _xIcon;
        }
        else
        {
            _cellImage.sprite = _oIcon;
        }
    }
    #endregion

}
