using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {

    public int _name;
    public int _type;

    public string _pair;
    public string _view;
    public string _nameNormal;
    public Vector3 _normal;

    public void setPoint(int name)
    {
        _type = name % 2;
        _name = name;
    }

    public void setNormal()
    {
        switch (_nameNormal)
        {
            case "z":
                _normal = Vector3.forward;
                break;

            case "y":
                _normal = Vector3.up;
                break;

            case "x":
                _normal = Vector3.right;
                break;

            default:
                _normal = Vector3.zero;
                break;

        }
    }
}
