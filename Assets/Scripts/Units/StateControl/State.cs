using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
  [CreateAssetMenu( menuName = "Data/Unit/State")]
  public class State : _ScriptableObject
  {
    public Action[] Actions;
    public Transition[] Transitions;
    public Dictionary<string, Event> Events;
    public float ActionInterval;
    public float TransitionInterval;
    public Color GizmoColor = Color.grey;

    public void UpdateTransition(Controller controller)
    {
      foreach (var transition in this.Transitions) {
        bool isDecidedTrue = transition.Decision.Decide(controller);
        controller.TransitionTo(
          isDecidedTrue ? transition.trueState: transition.falseState);
      }
    }

    public void PerformAction(Controller controller)
    {
      for (int i = 0; i < this.Actions.Length; i++) {
        this.Actions[i].Act(controller);
      }
    }
  }

}
