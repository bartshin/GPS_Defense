using System;

[Serializable]
public class MonsterTransition 
{
  public MonsterDecision Decision;
  public MonsterState trueState;
  public MonsterState falseState;
}
