using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Destroy : MonoBehaviour
{
    //Destroy gameObject after aliveTime ammount of seconds
    [SerializeField] bool destroy = true;
    [SerializeField, Tooltip("Time before the object gets destroyed")] float aliveTime = 3f;

    [Space]

    //Shrink object after shrinkTime
    [SerializeField, Tooltip("If the object should shrink and then get destroyed")] bool shrink = false;
    [SerializeField, Tooltip("The time before the object shrinks")] float waitsForShrinkTime = 2f;

    [Space]

    //Make the object expand to original size over ExpandTime
    [SerializeField] bool expand = false;
    [SerializeField] float expandTime = 2f;

    void Start()
    {
        Destruct();
    }

    public void Destruct()
    {
        if (shrink && expand)
        {
            StartCoroutine(ExpandThenShrinkRoutine());
        }
        else if (shrink)
        {
            StartCoroutine(ShrinkRoutine());
        }
        else if (destroy && expand)
        {
            StartCoroutine(ExpandRoutine());
            Destroy(gameObject, aliveTime + expandTime);
        }
        else if (expand)
        {
            StartCoroutine(ExpandRoutine());
        }
        else if (destroy)
        {
            Destroy(gameObject, aliveTime);
        }
    }

    IEnumerator ShrinkRoutine()
    {
        float elapsedTime = 0f;
        Vector3 originalSize = transform.localScale;

        //Wait before the object shrinks
        yield return new WaitForSeconds(waitsForShrinkTime);

        if (gameObject.TryGetComponent<DecalProjector>(out var decal))
        {
            Vector3 originalDecalSize = decal.size;
            // Shrink object and decal over aliveTime
            while (elapsedTime < aliveTime)
            {
                float t = elapsedTime / aliveTime;
                transform.localScale = Vector3.Lerp(originalSize, Vector3.zero, t);

                // Shrink decal width and height
                decal.size = new Vector3(
                    Mathf.Lerp(originalDecalSize.x, 0f, t),
                    Mathf.Lerp(originalDecalSize.y, 0f, t),
                    originalDecalSize.z // Keep depth unchanged
                );

                elapsedTime += Time.deltaTime;
                yield return null;

                if (elapsedTime > aliveTime)
                {
                    transform.localScale = Vector3.zero;
                    decal.size = new Vector3(0f, 0f, originalDecalSize.z);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            // Shrink object over aliveTime (original code)
            while (elapsedTime < aliveTime)
            {
                float t = elapsedTime / aliveTime;
                transform.localScale = Vector3.Lerp(originalSize, Vector3.zero, t);
                elapsedTime += Time.deltaTime;
                yield return null;

                if (elapsedTime > aliveTime)
                {
                    transform.localScale = Vector3.zero;
                    Destroy(gameObject);
                }
            }
        }
    }
    IEnumerator ExpandRoutine()
    {
        float elapsedTime = 0f;
        Vector3 originalSize = transform.localScale;

        //Make the gameObjects scale zero
        gameObject.transform.localScale = Vector3.zero;

        //Expand the gameObject over expandTime
        while (elapsedTime < expandTime)
        {
            float t = elapsedTime / expandTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, originalSize, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Make sure the object is at original size
        transform.localScale = originalSize;
    }
    IEnumerator ExpandThenShrinkRoutine()
    {
        //EXPAND
        StartCoroutine(ExpandRoutine());
        yield return new WaitForSeconds(expandTime);
        StopCoroutine(ExpandRoutine());

        //SHRINK
        StartCoroutine(ShrinkRoutine());
        yield return new WaitForSeconds(waitsForShrinkTime);
        StopCoroutine(ShrinkRoutine());

        //DESTROY
        Destroy(gameObject, aliveTime);
    }
    public void SetAliveTime(float time)
    {
        aliveTime = time;
    }
}
