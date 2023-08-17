using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DoFFocusOnObject : MonoBehaviour
{
    public Transform objectToFocusOn;
    public PostProcessVolume ppVolume;

    private DepthOfField dof;

    private void Start()
    {
        ppVolume.profile.TryGetSettings(out dof);
    }

    private void Update()
    {
        if (dof != null && objectToFocusOn != null)
        {
            dof.focusDistance.value = Vector3.Distance(Camera.main.transform.position, objectToFocusOn.position);
        }
    }
}
