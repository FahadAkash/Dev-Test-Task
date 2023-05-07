using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
   public float speed;

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    public void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 vector = new Vector3 (moveHorizontal, 0, moveVertical).normalized;
        this.transform.position +=  ( vector * Time.deltaTime * speed);
    }
}
