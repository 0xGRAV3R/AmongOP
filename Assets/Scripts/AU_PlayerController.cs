using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AU_PlayerController : MonoBehaviour
{
    //Components
    Rigidbody myRB;
    Transform myAvatar;
    Animator myAnim;
    //Player movement
    [SerializeField] InputAction WASD;
    Vector2 movementInput;
    [SerializeField] float movementSpeed;


    private void OnEnable()
    {
        WASD.Enable();

    }

    private void OnDisable()
    {
        WASD.Disable();

    }


    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();

        myAvatar = transform.GetChild(0);

        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = WASD.ReadValue<Vector2>();

        if (movementInput.x != 0)
        {
            myAvatar.localScale = new Vector2(Mathf.Sign(movementInput.x), 1);
        }

        myAnim.SetFloat("Speed", movementInput.magnitude);
    }

    private void FixedUpdate()
    {
        myRB.velocity = movementInput * movementSpeed;
    }

}