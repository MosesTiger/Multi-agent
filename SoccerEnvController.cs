using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class SoccerEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public AgentSoccer Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
    }


    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    /// <returns></returns>
    [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

    /// <summary>
    /// The area bounds.
    /// </summary>

    /// <summary>
    /// We will be changing the ground material based on success/failue
    /// </summary>

    public GameObject ball;
    [HideInInspector]
    public Rigidbody ballRb;
    Vector3 m_BallStartingPos;

    //List of Agents On Platform
    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();

    private SoccerSettings m_SoccerSettings;


    private SimpleMultiAgentGroup m_BlueAgentGroup;
    private SimpleMultiAgentGroup m_PurpleAgentGroup;

    private int m_ResetTimer;

    void Start()
    {

        m_SoccerSettings = FindObjectOfType<SoccerSettings>();
        // Initialize TeamManager
        m_BlueAgentGroup = new SimpleMultiAgentGroup();
        m_PurpleAgentGroup = new SimpleMultiAgentGroup();
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallStartingPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z);
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            if (item.Agent.team == Team.Blue)
            {
                m_BlueAgentGroup.RegisterAgent(item.Agent);
            }
            else
            {
                m_PurpleAgentGroup.RegisterAgent(item.Agent);
            }
        }
        ResetScene();
    }

void FixedUpdate()
{
    m_ResetTimer += 1;
    // 최대 환경 스텝에 도달하면 장면을 재설정합니다.
    if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
    {
        m_BlueAgentGroup.GroupEpisodeInterrupted(); // 파란 팀 에이전트 그룹 에피소드 중단
        m_PurpleAgentGroup.GroupEpisodeInterrupted(); // 보라 팀 에이전트 그룹 에피소드 중단
        ResetScene(); // 장면 재설정
    }
}

public void ResetBall()
{
    // 공의 위치를 무작위로 재설정합니다.
    var randomPosX = Random.Range(-2.5f, 2.5f);
    var randomPosZ = Random.Range(-2.5f, 2.5f);

    // 공의 위치와 속도를 초기화합니다.
    ball.transform.position = m_BallStartingPos + new Vector3(randomPosX, 0f, randomPosZ);
    ballRb.velocity = Vector3.zero; // 공의 선속도 초기화
    ballRb.angularVelocity = Vector3.zero; // 공의 각속도 초기화
}

public void GoalTouched(Team scoredTeam)
{
    // 골이 들어갔을 때 점수 계산 및 에이전트 업데이트
    if (scoredTeam == Team.Blue)
    {
        // 파란 팀이 득점했을 때 보상 계산
        m_BlueAgentGroup.AddGroupReward(1 - m_ResetTimer / MaxEnvironmentSteps);
        m_PurpleAgentGroup.AddGroupReward(-1);
    }
    else
    {
        // 보라 팀이 득점했을 때 보상 계산
        m_PurpleAgentGroup.AddGroupReward(1 - m_ResetTimer / MaxEnvironmentSteps);
        m_BlueAgentGroup.AddGroupReward(-1);
    }
    // 에피소드 종료 및 장면 재설정
    m_PurpleAgentGroup.EndGroupEpisode();
    m_BlueAgentGroup.EndGroupEpisode();
    ResetScene();
}

public void ResetScene()
{
    m_ResetTimer = 0; // 타이머 초기화

    // 모든 에이전트를 재설정합니다.
    foreach (var item in AgentsList)
    {
        // 에이전트의 위치와 회전을 무작위로 재설정합니다.
        var randomPosX = Random.Range(-5f, 5f);
        var newStartPos = item.Agent.initialPos + new Vector3(randomPosX, 0f, 0f);
        var rot = item.Agent.rotSign * Random.Range(80.0f, 100.0f);
        var newRot = Quaternion.Euler(0, rot, 0);
        item.Agent.transform.SetPositionAndRotation(newStartPos, newRot);

        // 에이전트의 속도와 각속도를 초기화합니다.
        item.Rb.velocity = Vector3.zero;
        item.Rb.angularVelocity = Vector3.zero;
    }

    // 공을 재설정합니다.
    ResetBall();
}
