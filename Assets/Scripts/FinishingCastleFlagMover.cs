using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishingCastleFlagMover : MonoBehaviour
{
    private Vector3 posA;

    private Vector3 posB;

    private Vector3 nextPos;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform childTransform;

    [SerializeField]
    private Transform transformB;
    // Start is called before the first frame update
    void Start()
    {
        posA = childTransform.localPosition;
        posB = transformB.localPosition;
        nextPos = posB;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Move()
    {
            StartCoroutine(FlagMover());
    }
    private IEnumerator FlagMover()
    {
        while (childTransform.localPosition != posB)
        {
            childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPos, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.001f); 
        }
    }
}
