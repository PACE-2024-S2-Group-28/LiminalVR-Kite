using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStringList", menuName = "New String List")]
public class StringArraySO : ScriptableObject
{
    //private String whatsThisFor;
    [SerializeField]
    private String[] strings;
    public String[] Strings { get { return strings; } }
}
