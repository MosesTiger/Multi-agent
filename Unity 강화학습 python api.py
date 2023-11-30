from mlagents_envs.environment import UnityEnvironment
# 유니티와 파이썬 코드간 상호작용을 하는 주요 요소를 모두 포함한 라이브러리 
if __name__ == '__main__': # 환경을 정의
    env = UnityEnvironment(file_name ='.') # 빌드할 unity scene의 경로

    env.reset() # 환경 초기화
    behavior_name = list(env.behavior_specs.keys())[0] # behaviour 관련 정보를 behaviour_name과 spec에 저장하다. 
    print(f'name of behaviour: {behavior_name}')
    spec = env.behavior_specs[behavior_name]

for ep in range(10):
    env.reset()
    decision_steps, terminal_steps=env.get_steps(behavior_name)

    tracked_agent = -1
    done = False 
    ep_rewards = 0

    tracked_agent = -1
    done = False 
    ep_rewards = 0

    while not done:
        if tracked_agent == -1 and len(decision_steps) >= 1:
            tracked_agent = decision_steps.agent_id[0]
        action = spec.action_spec.random_action(len(decision_steps))
        env.set_actions(behavior_name, action)
        env.step()

        decision_steps, terminal_steps = env.get_steps(behavior_name)
        
        if tracked_agent in decision_steps:
            ep_rewards += decision_steps[tracked_agent].reward
        if tracked_agent in terminal_steps:
            ep_rewards += terminal_steps[tracked_agent].reward
            done = True
    print(f'total reward for ep {ep} is {ep_rewards}')

env.close()

