using System;
using System.Collections.Generic;
using System.Linq;

namespace PNTD
{
    public class PartySystem
    {
        private readonly List<HeroContext> _heroContexts = new();

        public event Action<HeroContext> OnHeroAdded;
        public event Action<HeroContext> OnHeroRemoved;
        public event Action<HeroContext> OnHeroLevelUpdated;
        public event Action OnPartyChanged;
        
        public IReadOnlyList<HeroContext> HeroContexts => _heroContexts;

        public void Initialize(IEnumerable<HeroContext> heroContexts)
        {
            _heroContexts.Clear();

            if (heroContexts == null)
            {
                return;
            }

            foreach (var heroContext in heroContexts)
            {
                if (IsInvalidHeroContext(heroContext))
                {
                    continue;
                }

                if (_heroContexts.Contains(heroContext))
                {
                    continue;
                }
                
                _heroContexts.Add(heroContext);
            }
            
            OnPartyChanged?.Invoke();
        }

        public bool TryAddHeroContext(HeroContext heroContext)
        {
            if (IsInvalidHeroContext(heroContext))
            {
                return false;
            }

            var targetContext = FindHeroContext(heroContext.HeroDataTableRow.rowID);
            if (targetContext != null)
            {
                return TryUpdateExp(targetContext);
            }
            
            _heroContexts.Add(heroContext);
            OnHeroAdded?.Invoke(heroContext);
            OnPartyChanged?.Invoke();

            return true;
        }
        
        public void RemoveHeroContext(HeroContext heroContext)
        {
            if (heroContext == null)
            {
                return;
            }

            if (!_heroContexts.Contains(heroContext))
            {
                return;
            }
            
            _heroContexts.Remove(heroContext);
            OnHeroRemoved?.Invoke(heroContext);
            OnPartyChanged?.Invoke();
        }

        public void Clear()
        {
            if (_heroContexts.Count == 0)
            {
                return;
            }
            
            _heroContexts.Clear();
            OnPartyChanged?.Invoke();
        }

        public bool TryReorderParty(IReadOnlyList<HeroContext> heroContexts)
        {
            if (heroContexts == null)
            {
                return false;
            }

            if (heroContexts.Count != _heroContexts.Count)
            {
                return false;
            }

            if (heroContexts.Where((t, index) => _heroContexts[index] != t).Any())
            {
                _heroContexts.Clear();
                _heroContexts.AddRange(heroContexts);
                OnPartyChanged?.Invoke();
                return true;
            }

            return false;
        }

        public List<HeroContext> GetHeroContexts(string heroId)
        {
            if (string.IsNullOrWhiteSpace(heroId))
            {
                return new List<HeroContext>();
            }
            
            return _heroContexts
                .Where(heroContext => heroContext != null &&
                                      heroContext.HeroDataTableRow != null &&
                                      heroContext.HeroDataTableRow.rowID == heroId)
                .ToList();
        }

        public bool TryFindMergeTargets(string heroId, int requiredCount, out List<HeroContext> mergeTargets)
        {
            mergeTargets = GetHeroContexts(heroId)
                .OrderBy(heroContext => heroContext.Level)
                .ToList();

            return mergeTargets.Count >= requiredCount;
        }

        private HeroContext FindHeroContext(string heroId)
        {
            if (string.IsNullOrWhiteSpace(heroId))
            {
                return null;
            }
            
            return _heroContexts.FirstOrDefault(heroContext => heroContext != null &&
                                                               heroContext.HeroDataTableRow != null &&
                                                               heroContext.HeroDataTableRow.rowID == heroId);
        }

        private bool TryUpdateExp(HeroContext heroContext)
        {
            if (IsInvalidHeroContext(heroContext))
            {
                return false;
            }

            if (!heroContext.TryGetExp(1))
            {
                return false;
            }
            
            OnHeroLevelUpdated?.Invoke(heroContext);
            OnPartyChanged?.Invoke();
            return true;
        }

        private static bool IsInvalidHeroContext(HeroContext heroContext)
        {
            return heroContext == null || heroContext.HeroDataTableRow == null;
        }
    }
}