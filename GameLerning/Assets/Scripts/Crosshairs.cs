using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 40;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private SpriteRenderer dot;
    [SerializeField] private Color dotHighlightColor;
    private Color dotOriginalColor;

    private void Start()
    {
        Cursor.visible = false;
        dotOriginalColor = dot.color;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    public void DetectTarget(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColor;
        }
        else
        {
            dot.color = dotOriginalColor;
        }
    }
}
