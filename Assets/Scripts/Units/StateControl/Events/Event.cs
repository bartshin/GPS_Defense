using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unit
{
  [Serializable]
  public class Event 
  {
    [SerializeField]
    Trigger trigger;
    [SerializeField]
    Reaction reaction;
    [SerializeField]
    State trueState;
    [SerializeField]
    State falseState;

    public void OnEventOccur(BaseUnit unit)
    {
      if (this.trigger.IsTriggerToReact(unit)) {
        bool reactionResult = this.reaction.React(unit);
        unit.StateController.TransitionTo(reactionResult ? trueState: falseState);
      }
    }
  }

}
