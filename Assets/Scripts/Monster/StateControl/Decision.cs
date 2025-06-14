using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterDecision : _ScriptableObject
{
  public abstract bool Decide(MonsterController controller);
}
