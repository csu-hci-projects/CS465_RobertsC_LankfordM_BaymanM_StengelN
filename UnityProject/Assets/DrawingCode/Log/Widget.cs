/* 
 * mayra b, 2017
 * base class of a widget
 * 
 */


using UnityEngine;

public class Widget {

    private GameObject _thisWidget;
    private GlobalVars.WidgetType _type;

    private Vector3 _position;
    private Color _color;

    private Vector3 _direction;
    private float _size;
    private string _text;

    public Widget()
    {
    }

    public Widget(GlobalVars.WidgetType type, Vector3 position, Color color, Vector3 direction, float size)
    {
        _type = type;
        _position = position;
        _color = color;
        _direction = direction;
        _size = size;
    }

    public Widget(GlobalVars.WidgetType type, Vector3 position, Color color, float size)
    {
        _type = type;
        _position = position;
        _color = color;
        _size = size;
    }

    public Widget(Color color, GlobalVars.WidgetType type, Vector3 position, float size, string text)
    {
        _type = type;
        _position = position;
        _color = color;
        _size = size;
        _text = text;
    }

    public GameObject thisWidget
    {
        get { return _thisWidget; }
        set { _thisWidget = value; }
    }

    public GlobalVars.WidgetType type
    {
        get { return _type; }
    }

    public Vector3 position
    {
        get { return _position; }
        set { _position = value; }
    }

    public Color color
    {
        get { return _color; }
    }

    public Vector3 direction
    {
        get { return _direction; }
        set { _direction = value; }
    }

    public float size
    {
        get { return _size; }
    }

    public string text
    {
        get { return _text; }
    }
}
