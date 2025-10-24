using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform _boardTransform;
    [SerializeField] private GridLayoutGroup _boardGridLayoutGroup;
    [SerializeField] private GameObject _cellPrefab;

    void Start()
    {
        initCells();
    }
    
    private void initCells()
    {
        int size = GameController.Instance.CellQuantity;
        _boardGridLayoutGroup.constraintCount = size;

        for(int i = 0; i < size; i++)
        {
            for(int j = 0; j < size; j++)
            {
                GameObject cellObj = Instantiate(_cellPrefab, _boardTransform.position, Quaternion.identity, _boardTransform);
                Cell cell = cellObj.GetComponent<Cell>();
                cell.initXY(i, j);
            }
        }
    }
}
