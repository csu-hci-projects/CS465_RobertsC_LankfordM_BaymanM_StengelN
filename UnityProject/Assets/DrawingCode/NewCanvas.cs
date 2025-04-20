/* 
 * mayra barrera, 2017
 * script for create new canvas (delete all)
 * 
 */

using UnityEngine;

public class NewCanvas : MonoBehaviour {

    private GameObject background;

	// Use this for initialization
	void Start () {

        background = transform.Find("nc_Background").gameObject;

        DefaultValues();
	}
	
    public void CreateNewCanvas()
    {
        string name = "Textures/newCanvas";
        background.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load(name) as Texture;

        EventManager.Instance.TriggerEvent(new NewCanvasEvent());
    }

    public void DefaultValues()
    {
        background.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load("Textures/newCanvasB") as Texture;
    }
}
