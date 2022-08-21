using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubeController : MonoBehaviour
{
    static int _staticIdx = 0;
    [SerializeField] private TMP_Text[] _numbersTxt;

    [HideInInspector] public int _cubeIdx;
    [HideInInspector] public Color _cubeColor;
    [HideInInspector] public int _cubeNum;
    [HideInInspector] public Rigidbody _cubeRb;
    [HideInInspector] public bool isMainCube;       //유저가 움직이는 큐브인지

    private MeshRenderer _cubeMeshRender;

    private void Awake()
    {
        _cubeIdx = _staticIdx++;
        _cubeMeshRender = GetComponent<MeshRenderer>();
        _cubeRb = GetComponent<Rigidbody>();
    }

    public void SetColor(Color color)
    {
        _cubeColor = color;
        _cubeMeshRender.material.color = color;
    }

    public void SetNum(int num)
    {
        _cubeNum = num;
        for(var i = 0; i < _numbersTxt.Length; i++)
        {
            _numbersTxt[i].text = num.ToString();
        }
    }
}
