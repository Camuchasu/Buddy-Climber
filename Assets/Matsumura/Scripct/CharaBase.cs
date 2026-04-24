using UnityEngine;

public abstract class CharaBase : MonoBehaviour
{
    [SerializeField] protected int m_maxHp = 10;
    protected int m_hp = 0;

    protected virtual void Awake()
    {
        m_hp = m_maxHp;
    }
    public virtual void TakeDamage(int damage)
    {
        if (IsDeath()) return;

        m_hp -= damage;

        if (IsDeath())
        {
            Death();
        }
    }

    public virtual bool IsDeath()
    {
        return m_hp <= 0;
    }

    public virtual void Death()
    {

    }
}
