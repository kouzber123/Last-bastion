// This interface defines the contract for any damageable entity in the game, such as players, enemies, destructible objects, etc.
// It includes methods for taking damage, healing, and death handling.
public interface IDamageable
{
    void TakeDamage(float amount);
    void Heal(float amount);
    void Die();
}
