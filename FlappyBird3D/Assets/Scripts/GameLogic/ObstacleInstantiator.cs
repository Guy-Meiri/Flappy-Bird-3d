using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleInstantiator : MonoBehaviour
{
    Queue<KeyValuePair<GameObject, GameObject>> m_ObstacleQueue;
    Queue<GameObject> m_SceneryObjectsQueue;

    [SerializeField]
    private GameObject m_ObstaclePrefab;

    [SerializeField]
    private GameObject[] m_Obstacles;

    [SerializeField]
    private GameObject[] m_SceneryObjects;

    [SerializeField]
    private GameObject m_Player;

    [SerializeField]
    private float m_HorizontalDistanceBetweenObstacles;

    [SerializeField]
    private float m_MinObstacleVerticleGap;
    [SerializeField]
    private float m_MaxObstacleVerticleGap;
    [SerializeField]
    private float m_MinFloorObstacleYPosition;
    [SerializeField]
    private float m_MaxFloorObstacleYPosition;

    [SerializeField]
    private int m_FloorTileLengthZ;

    private GameObject m_LastFloorSpawnObstacle;
    private GameObject m_LastCeilingSpawnObstacle;
    private GameObject m_LastFloorSpawnScenery;

    [SerializeField]
    private float m_ObstacleMovementSpeed;

    private int m_Score = 0;

    private int m_ScorePerObstacle = 10;

    [SerializeField]
    private Text m_ScoreText;

    private float m_ObstacleHeight;

    private PlayerMovement m_PlayerMovementScript;


    [SerializeField]
    private GameObject m_CountdownObject;
    private CountdownAnimationManager m_CountdownAnimatorScript;

    void Start()
    {
        enQueueAllObstaclesOnInitialization();
        enQueueAllSceneryObjectsOnInitialization();
        m_PlayerMovementScript = m_Player.GetComponent<PlayerMovement>();
        m_PlayerMovementScript.m_Collided += OnCollisionWithPlayer; // We add ourselves as listeners to the Player's collision event
        m_CountdownAnimatorScript = m_CountdownObject.GetComponent<CountdownAnimationManager>();
    }
  
    void Update()
    {
        if (!m_PlayerMovementScript.IsGameOver() && !m_CountdownAnimatorScript.GetIsCountdownPlaying())
        {
            moveObstaclesTowardsPlayer();
            moveSceneryObjectsTowardsPlayer();
            moveFrontObstacleToBackIfPlayerPassedIt();
            moveFrontSceneryObjectToBackIfPlayerPassedIt();
        }

        handleInGameMenuIfNeeded();
    }

    private void OnCollisionWithPlayer(object sender, EventArgs e)
    {
        moveFrontObstaclesToTheBack();
    }

    private void moveObstaclesTowardsPlayer()
    {
        foreach (KeyValuePair<GameObject, GameObject> currLowerAndUpperObstacles in m_ObstacleQueue)
        {
            currLowerAndUpperObstacles.Key.transform.position -= new Vector3(0, 0, m_ObstacleMovementSpeed * Time.deltaTime);
            currLowerAndUpperObstacles.Value.transform.position -= new Vector3(0, 0, m_ObstacleMovementSpeed * Time.deltaTime);
        }
    }

    private void moveSceneryObjectsTowardsPlayer()
    {
        foreach (GameObject currLowerAndUpperObstacles in m_SceneryObjectsQueue)
        {
            currLowerAndUpperObstacles.transform.position -= new Vector3(0, 0, m_ObstacleMovementSpeed * Time.deltaTime);
        }
    }

    private void moveFrontObstacleToBackIfPlayerPassedIt()
    {
        KeyValuePair<GameObject, GameObject> lowerAndUpperObstaclesPeek = m_ObstacleQueue.Peek();
        if (lowerAndUpperObstaclesPeek.Key.transform.position.z < m_Player.transform.position.z)
        {
            moveFrontObstaclesToTheBack();
            m_Score += m_ScorePerObstacle;
            m_ScoreText.text = "Score: " + m_Score.ToString();
        }
    }

    private void moveFrontSceneryObjectToBackIfPlayerPassedIt()
    {
        GameObject SceneryObjectPeeked = m_SceneryObjectsQueue.Peek();
        if (SceneryObjectPeeked.transform.position.z < m_Player.transform.position.z - m_FloorTileLengthZ)
        {
            moveFrontSceneryObjectToTheBack();
        }
    }
    
    public int Score
    {
        get { return m_Score; }
    }

    private void moveFrontSceneryObjectToTheBack()
    {
        GameObject lowerAndUpperObstacles = m_SceneryObjectsQueue.Dequeue();
        lowerAndUpperObstacles.transform.position = new Vector3(lowerAndUpperObstacles.transform.position.x, lowerAndUpperObstacles.transform.position.y, m_LastFloorSpawnObstacle.transform.position.z + m_FloorTileLengthZ);

        m_SceneryObjectsQueue.Enqueue(lowerAndUpperObstacles);
        m_LastFloorSpawnScenery = lowerAndUpperObstacles;
    }

    private void moveFrontObstaclesToTheBack()
    {
        KeyValuePair<GameObject, GameObject> lowerAndUpperObstacles = m_ObstacleQueue.Dequeue();
        float newFloorObstacleYPosition = UnityEngine.Random.Range(m_MinFloorObstacleYPosition, m_MaxFloorObstacleYPosition);
        float newCeilingObstacleYPosition = newFloorObstacleYPosition + m_ObstacleHeight/2 + UnityEngine.Random.Range(m_MinObstacleVerticleGap, m_MaxObstacleVerticleGap);

        lowerAndUpperObstacles.Key.transform.position = new Vector3(0, newFloorObstacleYPosition, m_LastFloorSpawnObstacle.transform.position.z + m_HorizontalDistanceBetweenObstacles);
        lowerAndUpperObstacles.Value.transform.position = new Vector3(0, newCeilingObstacleYPosition, m_LastCeilingSpawnObstacle.transform.position.z + m_HorizontalDistanceBetweenObstacles);

        m_ObstacleQueue.Enqueue(lowerAndUpperObstacles);
        m_LastFloorSpawnObstacle = lowerAndUpperObstacles.Key;
        m_LastCeilingSpawnObstacle = lowerAndUpperObstacles.Value;
    }

    private void enQueueAllObstaclesOnInitialization()
    {
        m_ObstacleHeight = m_ObstaclePrefab.transform.localScale.y;
        m_ObstacleQueue = new Queue<KeyValuePair<GameObject, GameObject>>();// each pair holds a matching floor and ceiling obstacles

        for (int i = 0; i < m_Obstacles.Length; i += 2)
        {
            m_ObstacleQueue.Enqueue(new KeyValuePair<GameObject, GameObject>(m_Obstacles[i], m_Obstacles[i + 1]));
        }

        m_LastFloorSpawnObstacle = m_Obstacles[m_Obstacles.Length - 2]; // take the last obstacle in the array
        m_LastCeilingSpawnObstacle = m_Obstacles[m_Obstacles.Length - 1];
    }

    
    private void enQueueAllSceneryObjectsOnInitialization()
    {
        m_SceneryObjectsQueue = new Queue<GameObject>();

        for (int i = 0; i < m_SceneryObjects.Length; i++)
        {
            m_SceneryObjectsQueue.Enqueue(m_SceneryObjects[i]);
        }

        m_LastFloorSpawnScenery = m_SceneryObjects[m_SceneryObjects.Length - 1]; // take the last Object in the array
    }

    public void OnResume()
    {
        this.enabled = true;
    }

    private void handleInGameMenuIfNeeded()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            this.enabled = false;
        }
    }
}
