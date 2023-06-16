using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float velocity = 10.0f;
    public float rotationSpeed = 90.0f;

    private float mouseX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

        Vector3 dir = new Vector3(x, 0, y) * velocity * Time.deltaTime;

        transform.Translate(dir);

        transform.rotation = Quaternion.Euler(0, mouseX, 0);
    }
}
