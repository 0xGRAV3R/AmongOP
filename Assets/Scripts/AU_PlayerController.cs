using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AU_PlayerController : MonoBehaviour
{
    [SerializeField] bool hasControl;
    public static AU_PlayerController localPlayer;


    //Components
    Rigidbody myRB;
    Animator myAnim;
    Transform myAvatar;
    //Player movement
    [SerializeField] InputAction WASD;
    Vector2 movementInput;
    [SerializeField] float movementSpeed;
    //Player Color
    [SerializeField] Color myColor;
    SpriteRenderer myAvatarSprite;

    //Role
    [SerializeField] bool isImposter;
    [SerializeField] InputAction KILL;
    float killInput;

    List<AU_PlayerController> targets;
    [SerializeField] Collider myCollider;

    bool isDead;

    [SerializeField] GameObject bodyPrefab;

    public static List<Transform> allBodies;

    List<Transform> bodiesFound;

    [SerializeField] InputAction REPORT;
    [SerializeField] LayerMask ignoreForBody;

    private void Awake()
    {
        KILL.performed += KillTarget;
        REPORT.performed += ReportBody;
    }



    private void OnEnable()
    {
        WASD.Enable();
        KILL.Enable();
        REPORT.Enable();

    }

    private void OnDisable()
    {
        WASD.Disable();
        KILL.Disable();
        REPORT.Disable();

    }


    // Start is called before the first frame update
    void Start()
    {
        if (hasControl)
        {
            localPlayer = this;
        }

        targets = new List<AU_PlayerController>();
        myRB = GetComponent<Rigidbody>();
        myAnim = GetComponent<Animator>();
        myAvatar = transform.GetChild(0);
        myAvatarSprite = myAvatar.GetComponent<SpriteRenderer>();
        if (!hasControl)
            return;
        if (myColor == Color.clear)
            myColor = Color.white;
        myAvatarSprite.color = myColor;


        allBodies = new List<Transform>();

        bodiesFound = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasControl)
            return;

        movementInput = WASD.ReadValue<Vector2>();
        myAnim.SetFloat("Speed", movementInput.magnitude);
        if (movementInput.x != 0)
        {
            myAvatar.localScale = new Vector2(Mathf.Sign(movementInput.x), 1);
        }



        if (allBodies.Count > 0)
        {
            BodySearch();
        }

    }

    private void FixedUpdate()
    {
        myRB.velocity = movementInput * movementSpeed;
    }

    public void SetColor(Color newColor)
    {
        myColor = newColor;
        if (myAvatarSprite != null)
        {
            myAvatarSprite.color = myColor;
        }
    }

    public void SetRole(bool newRole)
    {
        isImposter = newRole;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AU_PlayerController tempTarget = other.GetComponent<AU_PlayerController>();
            if (isImposter)
            {
                if (tempTarget.isImposter)
                    return;
                else
                {
                    targets.Add(tempTarget);

                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            AU_PlayerController tempTarget = other.GetComponent<AU_PlayerController>();
            if (targets.Contains(tempTarget))
            {
                targets.Remove(tempTarget);
            }
        }
    }

    private void KillTarget(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //Debug.Log(targets.Count);
            if (targets.Count == 0)
                return;
            else
            {

                if (targets[targets.Count - 1].isDead)
                    return;

                transform.position = targets[targets.Count - 1].transform.position;
                targets[targets.Count - 1].Die();
                targets.RemoveAt(targets.Count - 1);
            }
        }
    }

    public void Die()
    {
        AU_Body tempBody = Instantiate(bodyPrefab, transform.position, transform.rotation).GetComponent<AU_Body>();
        tempBody.SetColor(myAvatarSprite.color);

        isDead = true;

        myAnim.SetBool("IsDead", isDead);
        gameObject.layer = 9;
        myCollider.enabled = false;
    }

    void BodySearch()
    {
        foreach (Transform body in allBodies)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, body.position - transform.position);
            Debug.DrawRay(transform.position, body.position - transform.position, Color.cyan);
            if (Physics.Raycast(ray, out hit, 1000f, ~ignoreForBody))
            {

                if (hit.transform == body)
                {
                    //Debug.Log(hit.transform.name);
                    //Debug.Log(bodiesFound.Count);
                    if (bodiesFound.Contains(body.transform))
                        return;
                    bodiesFound.Add(body.transform);
                }
                else
                {

                    bodiesFound.Remove(body.transform);
                }
            }
        }
    }

    private void ReportBody(InputAction.CallbackContext obj)
    {
        if (bodiesFound == null)
            return;
        if (bodiesFound.Count == 0)
            return;
        Transform tempBody = bodiesFound[bodiesFound.Count - 1];
        allBodies.Remove(tempBody);
        bodiesFound.Remove(tempBody);
        tempBody.GetComponent<AU_Body>().Report();
    }

}