using UnityEngine;

public class Plank : MonoBehaviour
{
    [SerializeField] Transform wholePlank;
    [SerializeField] MeshCollider brokenCollider;
    [SerializeField] BoxCollider plankCollider;
    [SerializeField] bool testBreakPlank;

    void Start()
    {
        brokenCollider.enabled = false;
        plankCollider.enabled = true;
    }

    void Update()
    {
        //TEMPORARY
        if (testBreakPlank) { BreakPlanks(); }
    }

    void BreakPlanks()
    {
        wholePlank.gameObject.SetActive(false);
        brokenCollider.enabled = true;
        plankCollider.enabled = false;
    }
}
