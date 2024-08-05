using UnityEngine;

public class MouseRaycast : KSMonoBehaviour
{
    [SerializeField]
    private bool raycastEnabled = true;

    // Update is called once per frame
    void Update()
    {
        if (raycastEnabled && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Bone bone = hit.collider.GetComponent<Bone>();
                if (bone != null)
                    bone.Collect();
            }
        }      
    }

    public void EnableRaycast()
    {
        raycastEnabled = true;
    }

    public void DisableRaycast()
    {
        raycastEnabled = false;
    }
}
