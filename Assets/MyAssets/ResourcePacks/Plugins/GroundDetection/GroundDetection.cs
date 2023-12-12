using System;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [SerializeField] private bool isGround;
    public bool IsGround { get=> isGround; private set=> isGround = value; }
    int groundCount;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundCount++;
            IsGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundCount--;

            if (groundCount == 0)
                IsGround = false;
        }
    }


}
