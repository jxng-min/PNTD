using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public class HeroFactory
    {
        private readonly Hero _heroPrefab;
        private readonly DataTable _heroAttackDataTable;
        private readonly HeroSkillContext _skillContext;
        private readonly Transform _defaultParent;

        public HeroFactory(Hero heroPrefab,
                           DataTable heroAttackDataTable,
                           HeroSkillContext skillContext,
                           Transform defaultParent)
        {
            _heroPrefab = heroPrefab;
            _heroAttackDataTable = heroAttackDataTable;
            _skillContext = skillContext;
            _defaultParent = defaultParent;
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

        private HeroStat CreateStat(DeployContext deployContext)
        {
            var attackDataTableRow = _heroAttackDataTable?.Find<HeroAttackDataTableRow>(
                row => row.isEnable && row.rowID == deployContext.HeroDataTableRow.rowID);

            return new HeroStat(attackDataTableRow);
        }

        private HeroSkill CreateSkill(DeployContext deployContext)
        {
            return new EmptyHeroSkill();
        }
    }
}
