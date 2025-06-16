
public interface IProjectile
{
  public IDamagable Target { get; }
  public int CalcDamage();
}
