﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveScript : MonoBehaviour
{
    //Movement related
    [Header("Movement")]
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    Vector3 velocity;
    public bool canMove;
    //Jump
    public float jumpHeight = 3f;

    //Ground Check
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    bool isGrounded;

    //Active next wagon
    public WagonManager current;

    [Header("Croshair")]
    public float hitDistance;
    Ray ray;
    RaycastHit hit;
    public Image croshairPlaceholder;
    public Sprite interactSprite;
    public Sprite standardSprite;
    public TicketManager ticketManager;
    public GameObject[] rebusDingen;
    public LookScript lookScript;
    public FadeAtNoMovement lvl2Mech;
    public bool lvl2;

    [FMODUnity.EventRef]
    public string steps;
    [FMODUnity.EventRef]
    public string liquidsteps;
    [FMODUnity.EventRef]
    public string ticket;
    [SerializeField]
    public FMODUnity.StudioEventEmitter emitter;

    [SerializeField]
    public FMODUnity.StudioEventEmitter shepard;

    [SerializeField]
    public FMODUnity.StudioEventEmitter wobbel;
    [SerializeField]
    public FMODUnity.StudioEventEmitter lvl4;
    public bool disableWobbel;
    public bool cantMove;

    public void disa()
    {
        foreach (GameObject go in rebusDingen)
        {
            go.SetActive(false);
        }
    }
    public void stepSound()
    {
        if (canMove)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                if (lvl2)
                {
                    FMODUnity.RuntimeManager.PlayOneShot(liquidsteps, this.transform.position);
                }
                else
                {
                    FMODUnity.RuntimeManager.PlayOneShot(steps, this.transform.position);
                }
            }
        }
    }
    int tempInt;
    private void Start()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/standup", GetComponent<Transform>().position);
        InvokeRepeating("stepSound", 0, 0.5f);

    }
    private void Update()
    {
        if (cantMove)
        {

        }
        else
        {
            if (disableWobbel)
            {
                wobbel.Stop();
            }
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                float hoverError = hitDistance - hit.distance;
                if (Input.GetMouseButtonDown(0))
                {
                    ticketManager.disa();
                }

                foreach (GameObject go in rebusDingen)
                {
                    if (go.activeSelf)
                    {
                        tempInt++;
                    }
                }
                if (tempInt > 0)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        disa();
                    }
                    tempInt = 0;
                }
                else
                {
                    if (hoverError > 0)
                    {
                        if (hit.collider.tag == "Interactable")
                        {
                            croshairPlaceholder.sprite = interactSprite;
                            if (Input.GetMouseButtonDown(0))
                            {
                                ticketManager.disa();

                                if (hit.collider.name == "Ticket")
                                {
                                    FMODUnity.RuntimeManager.PlayOneShot(ticket);
                                    ticketManager.interact();
                                    ticketManager.addState();
                                }

                                if (hit.collider.name == "btn")
                                {
                                    hit.collider.GetComponent<ButtonManager>().interact();
                                }

                                if (hit.collider.name == "Button")
                                {
                                    hit.collider.GetComponent<levelCbtnHandler>().interact();
                                }

                                if (hit.collider.name == "SolveA")
                                {
                                    hit.collider.GetComponent<SolveA>().interact();
                                }

                                if (hit.collider.name == "Rebus_1")
                                {
                                    canMove = false;
                                    lookScript.canLook = false;
                                    rebusDingen[0].SetActive(true);
                                }

                                if (hit.collider.name == "Rebus_2")
                                {
                                    canMove = false;
                                    lookScript.canLook = false;
                                    rebusDingen[1].SetActive(true);
                                }

                                if (hit.collider.name == "Rebus_3")
                                {
                                    canMove = false;
                                    lookScript.canLook = false;
                                    rebusDingen[2].SetActive(true);
                                }

                                if (hit.collider.name == "Rebus_4")
                                {
                                    canMove = false;
                                    lookScript.canLook = false;
                                    rebusDingen[3].SetActive(true);
                                }

                                if (hit.collider.name == "Rebus_5")
                                {
                                    canMove = false;
                                    lookScript.canLook = false;
                                    rebusDingen[4].SetActive(true);
                                }
                            }
                        }
                        else
                        {
                            croshairPlaceholder.sprite = standardSprite;
                        }
                    }
                    else
                    {
                        croshairPlaceholder.sprite = standardSprite;
                    }
                }

                isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

                if (canMove)
                {
                    if (isGrounded && velocity.y < 0)
                    {
                        velocity.y = -2;
                    }
                    else
                    {
                        velocity.y += gravity * Time.deltaTime;
                    }

                    if (Input.GetButtonDown("Jump") && isGrounded)
                    {
                        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    }

                    float x = Input.GetAxis("Horizontal");
                    float z = Input.GetAxis("Vertical");

                    Vector3 move = transform.right * x + transform.forward * z;
                    controller.Move(move * speed * Time.deltaTime);

                    controller.Move(velocity * Time.deltaTime);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Sets Current wagon to the room you are in
        if (collision.gameObject.GetComponent<WagonManager>())
        {
            current = collision.gameObject.GetComponent<WagonManager>();
        }
    }

    public float temp2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "lvl3")
        {
            if (!lvl4.IsPlaying())
            {
                temp2 = 0;
                lvl4.SetParameter("ToCredits", temp2);
                lvl4.Play();
            }
        }
        if (other.tag == "lvl2" || other.tag == "lvl4" || other.tag == "tempDisable")
        {
            if (!shepard.IsPlaying())
            {
                shepard.Play();
            }
        }

        if (other.tag == "lvl3")
        {
            if (!lvl4.IsPlaying())
            {
                temp2 -= 0;
                lvl4.SetParameter("ToCredits", temp2);
                lvl4.Play();
            }
        }
        else
        {
            temp2 += (float)0.1f * Time.deltaTime;
            lvl4.SetParameter("ToCredits", temp2);
            if (temp2 >= 1)
            {
                lvl4.Stop();
            }
        }

        if (other.tag == "lvl3")
        {
            wobbel.Stop();
            shepard.Stop();
        }

        if (other.tag == "lvl2" || other.tag == "tempDisable" || other.tag == "lvl4")
        {
            if (other.tag == "lvl4" || other.tag == "lvl2")
            {
                if (!wobbel.IsPlaying())
                {
                    wobbel.Play();
                }
            }
            lvl2 = true;
            lvl2Mech.enabled = true;
        }
        else
        {

            lvl2 = false;
            lvl2Mech.enabled = false;
            lvl2Mech.imageFade.color = new Color(0, 0, 0, 0);
        }
    }
    float temp;
    bool played;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "lvl3")
        {
            if (!lvl4.IsPlaying())
            {
                temp2 = 0;
                Debug.Log("ik speel");
                lvl4.SetParameter("ToCredits", temp2);
            }
        }
        else
        {
            temp2 += (float)0.1f * Time.deltaTime;
            lvl4.SetParameter("ToCredits", temp2);
            if (temp2 >= 1)
            {
                lvl4.Stop();
            }
        }

        if (other.tag == "lvl4")
        {
            if (ticketManager.current < 5)
            {
                if (temp < 1)
                {
                    temp += (float)0.12 * Time.deltaTime;
                }
            }
            else
            {
                temp -= (float)0.5 * Time.deltaTime;
            }
            emitter.SetParameter("panic level", temp);
            if (!emitter.IsPlaying())
            {
                if (!played)
                {
                    played = true;
                    Debug.Log("reee");
                    emitter.Play();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "lvl3" || other.tag == "tempDisable")
        {
            lvl2Mech.enabled = true;
        }

        if (other.tag == "lvl4")
        {
            emitter.Stop();
            Debug.Log("exitt");

        }
    }
}
