using System;

namespace Unit
{
  [Serializable]
  public class Transition 
  {
    public Decision Decision;
    public State trueState;
    public State falseState;
  }
}
