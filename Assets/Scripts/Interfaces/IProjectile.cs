
public interface IProjectile
{
  public IDamagable Target { get; set; }
  public int CalcDamage { get; set; }
}
