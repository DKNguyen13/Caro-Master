using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform _boardTransform;
    [SerializeField] private GridLayoutGroup _boardGridLayoutGroup;
    [SerializeField] private GameObject _cell;
    [SerializeField] private int _boardCell = 9;

    void Start()
    {
        initCell();
    }

    void Update()
    {

    }
    
    private void initCell()
    {
        _boardGridLayoutGroup.constraintCount = _boardCell;
        for(int i = 0; i < _boardCell; i++)
        {
            for(int j = 0; j < _boardCell; j++)
            {
                Instantiate(_cell, _boardTransform.position, Quaternion.identity, _boardTransform);
            }
        }
    }
}
