
public interface IHitable
{
    public float OnHit();
    public void OnDead();
}
public interface IHealthBar
{
    public void UpdateHealth(float healthRemain); 
}