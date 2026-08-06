namespace PNTD
{
    public class HeroContext
    {
        private const int MaxLevel = 3;
        private const int MaxExp = 3;

        public HeroDataTableRow HeroDataTableRow { get; }
        public int Level { get; private set; }
        public int Exp { get; private set; }

        public HeroContext(HeroDataTableRow heroDataTableRow)
        {
            HeroDataTableRow = heroDataTableRow;
            Level = 1;
            Exp = 0;
        }

        public HeroContext(HeroDataTableRow heroDataTableRow, int level, int exp)
        {
            HeroDataTableRow = heroDataTableRow;
            Level = level;
            Exp = exp;
        }

        public bool CanGetExp(int amount)
        {
            return amount > 0 && Level < MaxLevel;
        }

        public bool TryGetExp(int amount)
        {
            if (!CanGetExp(amount))
            {
                return false;
            }
            
            Exp += amount;

            while (Exp >= MaxExp && Level < MaxLevel)
            {
                Exp -= MaxExp;
                Level++;
            }

            if (Level >= MaxLevel)
            {
                Exp = 0;
            }

            return true;
        }
    }
}
