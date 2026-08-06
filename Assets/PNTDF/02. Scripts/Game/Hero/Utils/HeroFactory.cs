using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public class HeroFactory
    {
        private const string MagitechRobotHeroId = "Hero_MageRobo";
        
        private readonly Hero _heroPrefab;
        private readonly Hero _magitechRobotPrefab;
        private readonly DataTable _heroDataTable;
        private readonly DataTable _heroAttackDataTable;
        private readonly HeroSkillContext _skillContext;
        private readonly Transform _defaultParent;

        public HeroFactory(Hero heroPrefab,
                           Hero magitechRobotPrefab,
                           DataTable heroDataTable,
                           DataTable heroAttackDataTable,
                           HeroSkillContext skillContext,
                           Transform defaultParent)
        {
            _heroPrefab = heroPrefab;
            _magitechRobotPrefab = magitechRobotPrefab;
            _heroDataTable = heroDataTable;
            _heroAttackDataTable = heroAttackDataTable;
            _skillContext = skillContext;
            _defaultParent = defaultParent;
            _skillContext?.SetHeroFactory(this);
        }

        public Hero Create(DeployContext deployContext, Vector3 position, Transform parent = null)
        {
            if (deployContext == null || deployContext.HeroDataTableRow == null || _heroPrefab == null)
            {
                return null;
            }

            var hero = Object.Instantiate(_heroPrefab, position, Quaternion.identity, parent != null ? parent : _defaultParent);
            var stat = CreateStat(deployContext);
            var skill = CreateSkill(deployContext);
            
            skill.Initialize(_skillContext);
            hero.Initialize(deployContext.HeroDataTableRow, stat, skill, deployContext.Level);

            if (deployContext.Effects == null)
            {
                return hero;
            }

            foreach (var effect in deployContext.Effects)
            {
                hero.TryAddEffect(effect);
            }

            return hero;
        }
        
        public Hero CreateSummoned(string heroId, int level, Hero summoner, Vector3 position, Transform parent = null)
        {
            if (string.IsNullOrWhiteSpace(heroId) || summoner == null)
            {
                return null;
            }

            var heroDataTableRow = _heroDataTable?.Find<HeroDataTableRow>(row => row.isEnable && row.rowID == heroId);
            if (heroDataTableRow == null)
            {
                return null;
            }
            
            return Create(heroDataTableRow,
                          Mathf.Clamp(level, 1, 3),
                          position,
                          parent,
                          true,
                          summoner);
        }
        
        private Hero Create(HeroDataTableRow heroDataTableRow,
                            int level,
                            Vector3 position,
                            Transform parent,
                            bool isSummoned,
                            Hero summoner)
        {
            var prefab = GetPrefab(heroDataTableRow);
            if (heroDataTableRow == null || prefab == null)
            {
                return null;
            }
            
            var hero = Object.Instantiate(prefab, position, Quaternion.identity, parent != null ? parent : _defaultParent);
            var stat = CreateStat(heroDataTableRow);
            var skill = CreateSkill(heroDataTableRow.rowID);
            
            skill.Initialize(_skillContext);
            hero.Initialize(heroDataTableRow, stat, skill, level, isSummoned, summoner);

            return hero;
        }

        private HeroStat CreateStat(DeployContext deployContext)
        {
            return CreateStat(deployContext.HeroDataTableRow);
        }
        
        private HeroStat CreateStat(HeroDataTableRow heroDataTableRow)
        {
            var attackDataTableRow = _heroAttackDataTable?.Find<HeroAttackDataTableRow>(
                row => row.isEnable && row.rowID == heroDataTableRow.rowID);

            return new HeroStat(attackDataTableRow);
        }

        private HeroSkill CreateSkill(DeployContext deployContext)
        {
            return CreateSkill(deployContext.HeroDataTableRow.rowID);
        }
        
        private HeroSkill CreateSkill(string heroId)
        {
            return heroId switch
            {
                "Hero_Archer" => new ArcherSkill(),
                "Hero_Handgunner" => new HandgunnerSkill(),
                "Hero_Shotgunner" => new ShotgunnerSkill(),
                "Hero_Artillery" => new ArtillerySkill(),
                "Hero_Sniper" => new SniperSkill(),
                "Hero_Trickshooter" => new TrickshooterSkill(),
                "Hero_Magician" => new MagicianSkill(),
                "Hero_Wizard" => new WizardSkill(),
                "Hero_Explomancer" => new ExplomancerSkill(),
                "Hero_Telekinetic" => new TelekineticSkill(),
                "Hero_Transmuter" => new TransmuterSkill(),
                "Hero_Artificer" => new ArtificerSkill(),
                "Hero_Miner" => new MinerSkill(),
                "Hero_Alchemist" => new AlchemistSkill(),
                "Hero_Saint" => new SaintSkill(),
                "Hero_Sancitifier" => new SanctifierSkill(),
                "Hero_Sanctifier" => new SanctifierSkill(),
                "Hero_Crusader" => new CrusaderSkill(),
                MagitechRobotHeroId => new ArtificerRobotSkill(),
                "Hero_Martian" => new MartianSkill(),
                "Hero_Venusian" => new VenusianSkill(),
                "Hero_Jovian" => new JovianSkill(),
                "Hero_Saturnian" => new SaturnianSkill(),
                "Hero_Uranian" => new UranianSkill(),
                _ => new EmptyHeroSkill()
            };
        }

        private Hero GetPrefab(HeroDataTableRow heroDataTableRow)
        {
            if (heroDataTableRow != null && heroDataTableRow.rowID == MagitechRobotHeroId)
            {
                return _magitechRobotPrefab != null ? _magitechRobotPrefab : _heroPrefab;
            }

            return _heroPrefab;
        }
    }
}
