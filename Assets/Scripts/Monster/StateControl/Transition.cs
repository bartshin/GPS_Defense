using System;

namespace Monster
{
  [Serializable]
  public class Transition 
  {
    public Decision Decision;
    public State trueState;
    public State falseState;
  }
}
