using UnityEngine;

// Ensure that there is a LineRenderer component on the same object
[RequireComponent(typeof(LineRenderer))]
public class EyeRayRenderer : MonoBehaviour
{
    // Pass through the Main Camera
    [SerializeField]
    private Camera mainCamera;

    // Pass through the line end point (for gaze viz, something like the followning should work (gazeOrigin + (gazeDirection * focusDistance)) )
    [SerializeField]
    private Vector3 lineEndPoint;

    // Reference to the LineRenderer
    private LineRenderer lineRenderer;

    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Make sure to specify 'mainCamera'!");
        }

        // Get the LineRenderer on the same object
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // Make the LineRenderer start rendering from 10cm below the mainCamera's position
        lineRenderer.SetPosition(0, mainCamera.transform.position - new Vector3(0f, 0.1f, 0f));
        
        lineEndPoint = mainCamera.transform.position + mainCamera.transform.forward * 2.0f;
        // Make the LineRenderer stop rendering at the lineEndPoint (gaze focus point)
        lineRenderer.SetPosition(1, lineEndPoint);
    }
}
