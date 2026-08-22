using UnityEngine;

public class XFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform xFollowTarget;   // The object whose X we follow

    private void Update()
    {
        FollowXOnly();
    }

    private void FollowXOnly()
    {
        if (xFollowTarget == null)
            return;

        Vector3 pos = transform.position;
        pos.x = xFollowTarget.position.x;
        transform.position = pos;
    }
}