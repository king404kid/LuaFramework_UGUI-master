using System;
using UnityEngine;

[Serializable]
public class BindItem
{
    [SerializeField]
    public string name;
    [SerializeField]
    public UnityEngine.Object obj;
    [SerializeField]
    public UnityEngine.Object[] objArr;
}