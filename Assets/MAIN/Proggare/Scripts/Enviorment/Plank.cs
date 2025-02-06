using UnityEngine;

public class Plank : MonoBehaviour
{
    [SerializeField] BoxCollider hitBox;
    [SerializeField] MeshRenderer plank, brokenPlank;

    void Start()
    {
        plank.enabled = true;
        brokenPlank.enabled = false;
    }

    public void BreakPlanks()
    {
        hitBox.enabled = false;
        plank.enabled = false;
        brokenPlank.enabled = true;
    }
}
