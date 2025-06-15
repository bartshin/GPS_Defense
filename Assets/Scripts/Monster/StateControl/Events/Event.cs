using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
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

    public void OnEventOccur(Controller controller)
    {
      if (this.trigger.IsTriggerToReact(controller)) {
        bool reactionResult = this.reaction.React(controller);
        controller.TransitionTo(reactionResult ? trueState: falseState);
      }
    }
  }

}
