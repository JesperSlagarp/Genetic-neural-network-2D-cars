using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public float sensorDistance;
    [SerializeField] private float hitpointX;
    [SerializeField] private float hitpointY;
    private float maxDist = 100;
    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    private void Awake()
    {
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
    }

    void scan() {
        if (Physics2D.Raycast(transform.position, transform.up, maxDist, LayerMask.GetMask("Walls")))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, maxDist,LayerMask.GetMask("Walls"));
            drawLine(transform.position, hit.point);
            hitpointX = hit.point.x;
            hitpointY = hit.point.y;
            sensorDistance = Vector3.Distance(transform.position, hit.point);
        } else {
            drawLine(transform.position, transform.position + transform.up * maxDist);
            sensorDistance = maxDist;
        }
    }

    void drawLine(Vector2 startPos, Vector2 endPos) {
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }



    // Update is called once per frame
    void Update()
    {
        scan();
    }
}
