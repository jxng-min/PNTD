using System.Collections.Generic;
using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public class SynergyDataTableRow : DataTableRowBase
    {
        public ESynergy synergy;
        public string displayName;
        public string description;
        public int maxStep;
        public Sprite icon;
        public Color color;
        public List<int> thresholds;
        public List<ESynergyEffect> synergyEffects; 
        public List<float> primaryValues;
        public List<float> secondaryValues;
    }
}