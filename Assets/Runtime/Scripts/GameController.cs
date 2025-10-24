using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : Singleton <GameController>
{
    [SerializeField] private int _cellQuantity = 9;
    [SerializeField] private int[,] _board;
    [SerializeField] private int _turn = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private Button _loadMenuBtn;

    void Start()
    {
        initBoard();
        SetTurnText();
        handleOnClick();
    }

    void Update()
    {

    }

    #region Init board
    private void initBoard()
    {
        _board = new int[_cellQuantity, _cellQuantity];
        for (int x = 0; x < _cellQuantity; x++)
        {
            for (int y = 0; y < _cellQuantity; y++)
            {
                _board[x, y] = 0;
            }
        }
        Debug.Log($"Init board {_cellQuantity} x {_cellQuantity}");
    }
    #endregion

    #region Set cell
    public void SetCell(int x, int y, int value)
    {
        if (x < 0 || y < 0 || x >= _cellQuantity || y >= _cellQuantity)
        {
            Debug.LogError($"Player value with position [{x}, {y}] out board");
        }
        _turn++;
        SetTurnText();
        _board[x, y] = value; //value 0 init, 1 player, 2 AI
        //Debug.Log($"Mark position {x},{y} and turn {_turn}");
    }

    public int GetCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _cellQuantity || y >= _cellQuantity)
        {
            Debug.LogError($"Player value with position [{x}, {y}] out board");
            return -999;
        }
        return _board[x, y];
    }

    public void SetTurnText()
    {
        _turnText.text = "Turn " + _turn;
    }
    #endregion

    #region On click
    public void handleOnClick()
    {
        if (_loadMenuBtn)
        {
            _loadMenuBtn.onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
        }
        else
        {
            Debug.LogError("Load menu scene error");
        }
    }
    #endregion

    // Getter, setter
    public int CellQuantity => _cellQuantity;
    public int[,] GetBoard => _board;
    public int GetTurn => _turn;
}
