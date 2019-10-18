using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    Image[] m_Hearts;
    private int m_LivesLeft = 3;

    [SerializeField]
    private GameObject m_CountdownObject;
    private CountdownAnimationManager m_CountdownAnimatorScript;

    [SerializeField]
    private float m_UpwardForceFactor;

    [SerializeField]
    private Rigidbody m_RigidBody;

    //main player Movement and rotation variables and references
    [SerializeField]
    private float m_WingRotationAngle;
    private bool m_IsWingsMovingUp = true;
    [SerializeField]
    private int m_NumberOfFramesInWingMovement;
    private int m_TimeUntilWingRotationChange;
    [SerializeField]
    private GameObject m_ButterflyBody;
    [SerializeField]
    private float m_ButterflyBodyMovementFactor;
    [SerializeField]
    private GameObject m_LeftWing;
    [SerializeField]
    private GameObject m_RightWing;

    public event EventHandler m_Collided;

    [SerializeField]
    private GameObject m_GameOverPanel;
    private GameOverScript m_GameOverScript;

    [SerializeField]
    private GameObject m_MainMenuPanel;

    private float k_UpwardForceDivider = 1.8f;
    private bool m_IsGameOver = false;

    void Start()
    {
        m_TimeUntilWingRotationChange = m_NumberOfFramesInWingMovement / 2;
        m_GameOverScript = m_GameOverPanel.GetComponent<GameOverScript>();
        m_CountdownAnimatorScript = m_CountdownObject.GetComponent<CountdownAnimationManager>();
        m_RigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (!m_CountdownAnimatorScript.GetIsCountdownPlaying())
        {
            handleInGameMenu();
            animatePlayer();
            handlePlayerMovement();
        }
    }

    private void handlePlayerMovement()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            m_RigidBody.AddForce(Vector3.up * m_UpwardForceFactor / k_UpwardForceDivider * Time.deltaTime, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_RigidBody.AddForce(Vector3.up * m_UpwardForceFactor * Time.deltaTime, ForceMode.Impulse);
        }
    }

    private void animatePlayer()
    {
        if (m_TimeUntilWingRotationChange == 0)
            m_IsWingsMovingUp = !m_IsWingsMovingUp;

        if (m_IsWingsMovingUp)
        {
            m_ButterflyBody.transform.position += Vector3.up * m_ButterflyBodyMovementFactor;
            m_LeftWing.transform.RotateAround(m_LeftWing.transform.up, m_WingRotationAngle);
            m_RightWing.transform.RotateAround(m_LeftWing.transform.up, -m_WingRotationAngle);
        }
        else
        {
            m_ButterflyBody.transform.position -= Vector3.up * m_ButterflyBodyMovementFactor;
            m_LeftWing.transform.RotateAround(m_LeftWing.transform.up, -m_WingRotationAngle);
            m_RightWing.transform.RotateAround(m_LeftWing.transform.up, m_WingRotationAngle);
        }

        m_TimeUntilWingRotationChange = (m_TimeUntilWingRotationChange + 1) % m_NumberOfFramesInWingMovement;
    }

    private void handleInGameMenu()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            m_MainMenuPanel.SetActive(true);
            m_RigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            this.enabled = false;
        }
    }

    public void OnResume()
    {
        m_MainMenuPanel.SetActive(false);
        m_RigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        this.enabled = true;
    }

    public void OnCollisionEnter(Collision i_Colider)
    {
        m_LivesLeft--;
        if (m_LivesLeft >= 0)
            m_Hearts[m_LivesLeft].color = Color.black;

        if (i_Colider.gameObject.tag == "obstacle")
        {
            m_Collided.Invoke(this, EventArgs.Empty);
            transform.position = new Vector3(transform.position.x, 3, 0);
            m_RigidBody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            if (m_LivesLeft > 0)
            {
                m_CountdownAnimatorScript.StartAnimation(m_IsGameOver);
            }
        }

        if (m_LivesLeft <= 0)
            gameOver();

    }

    public bool IsGameOver()
    {
        return m_IsGameOver;
    }

    private void gameOver()
    {
        m_IsGameOver = true;
        m_GameOverPanel.SetActive(true);
        m_GameOverScript.ShowFinalScore();
        this.enabled = false;
    }
}
