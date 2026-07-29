namespace PNTD
{
    public class EnemyAbilityData
    {
        public string AbilityID { get; }

        protected EnemyAbilityData(string abilityID)
        {
            AbilityID = abilityID;
        }
    }
}