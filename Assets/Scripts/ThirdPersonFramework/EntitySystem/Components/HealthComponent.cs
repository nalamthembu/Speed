using UnityEngine;

namespace ThirdPersonFramework
{
    public class HealthComponent : BaseEntityComponent
    {
        [SerializeField] float m_Health;
        [SerializeField] float m_Armour;

        public delegate void OnDeathEvent(Entity targetEntity);
        public event OnDeathEvent OnDeath;

        public bool IsDead { get; private set; }

        protected override void Initialise()
        {
            base.Initialise();

            m_ThisEntity = GetComponent<Entity>();

            m_Health = 100;
        }

        public void TakeDamage(float damage, Entity instigator, Entity DamageCauser)
        {
            if (IsDead)
                return;

            if (m_Armour > 0)
            {
                m_Armour -= damage;

                return;
            }
            else
            {
                m_Health -= damage;

                if (m_Health < 0)
                {
                    IsDead = true;
                    OnDeath?.Invoke(m_ThisEntity);
                }
            }
        }
        public void AddArmour(float additional) => m_Armour = Mathf.Clamp(m_Armour + additional, 0, 100);
        public void AddHealth(float additional) => m_Health = Mathf.Clamp(m_Health + additional, 0, 100);

        #region CHEATS
        [ContextMenu("Cheats/Infinity Health")]
        public void GiveInfiniteHealth() => m_Health = Mathf.Infinity;
        [ContextMenu("Cheats/Infinity Armour")]
        public void GiveInfiniteArmour() => m_Armour = Mathf.Infinity;
        [ContextMenu("Cheats/Take Away Invincibility")]
        public void TakeAwayInvincibility()
        {
            m_Health = 100;
            m_Armour = 0;
        }
        #endregion
    }
}
