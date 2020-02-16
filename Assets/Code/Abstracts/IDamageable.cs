public interface IDamageable 
{
    // int maxHealth { get; set; }
    int currentHealth { get; set; }
    void DealDamage(int amount);
    void AddHealth(int amount);
    void ResetHealth();
    void DestroyTank();
}
