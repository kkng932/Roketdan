// 위치 기즈모 표시 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionGizmo : MonoBehaviour
{
    [SerializeField]
    Color gizmoColor = new Color(1f, 0f, 0f, 0.2f);

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
