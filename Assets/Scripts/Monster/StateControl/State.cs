using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "NewMonsterState", menuName = "Data/Monster State")]
public class MonsterState : _ScriptableObject
{
  public MonsterAction[] Actions;
  public MonsterTransition[] Transitions;
  public Color GizmoColor = Color.grey;

  public void UpdateState(MonsterController controller)
  {
    this.PerformAction(controller);
    this.CheckTransition(controller);
  }

  void CheckTransition(MonsterController controller)
  {
    foreach (var transition in this.Transitions) {
      bool isDecidedToTransition = transition.Decision.Decide(controller);
      if (isDecidedToTransition) {
        controller.TransitionTo(transition.trueState);
      } 
      else {
        controller.TransitionTo(transition.falseState);
      }
    }
  }

  void PerformAction(MonsterController controller)
  {
    for (int i = 0; i < this.Actions.Length; i++) {
      this.Actions[i].Act(controller);
    }
  }
}
