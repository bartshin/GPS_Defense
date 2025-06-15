
namespace Monster
{
  public abstract class Decision : _ScriptableObject
  {
    public abstract bool Decide(Controller controller);
  }

}
