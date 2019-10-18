using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownAnimationManager : MonoBehaviour
{
    private bool isCountdownPlaying = false;

    private Animator m_animation;

    [SerializeField]
    private GameObject m_Player;

    private Rigidbody m_PlayerRigidBody;

    private ObstacleInstantiator m_ObstacleInstantiatorScriptScript;

    [SerializeField]
    private GameObject m_ObstacleManager;

    void Start()
    {
        m_animation = GetComponent<Animator>();
        m_PlayerRigidBody = m_Player.GetComponent<Rigidbody>();
        m_ObstacleInstantiatorScriptScript = m_ObstacleManager.GetComponent<ObstacleInstantiator>();

    }

    public bool GetIsCountdownPlaying()
    {
        return isCountdownPlaying;
    }

    public void StartAnimation(bool i_IsGameOver)
    {
        this.isCountdownPlaying = true;
        m_PlayerRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        if (!i_IsGameOver)
        {
            m_animation.Play("countdownAnimation");
        }
    }

    public void onAnimationEnter()
    {
        isCountdownPlaying = true;
    }

    public void onAnimationEnd()
    {
        m_ObstacleInstantiatorScriptScript.enabled = true;
        isCountdownPlaying = false;
        m_PlayerRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
