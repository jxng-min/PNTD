using System.Collections.Generic;
using System.Linq;
using JxModule;
using JxModule.DataTable;
using UnityEngine;

namespace PNTD
{
    public class StageBuildSystem
    {
        private readonly DataTable _stageDataTable;
        private readonly DataTable _waveDataTable;
        private readonly DataTable _turnDataTable;
        private readonly DataTable _enemyDataTable;

        public StageBuildSystem(DataTable stageDataTable,
                                DataTable waveDataTable,
                                DataTable turnDataTable,
                                DataTable enemyDataTable)
        {
            _stageDataTable = stageDataTable;
            _waveDataTable = waveDataTable;
            _turnDataTable = turnDataTable;
            _enemyDataTable = enemyDataTable;
        }

        public StageContext Build(string stageId)
        {
            var stageDataTableRow = _stageDataTable.Find<StageDataTableRow>(row => row.isEnable && row.rowID == stageId);
            if (stageDataTableRow == null)
            {
                DebugExtension.LogColor($"Stage Builder: Stage not found. Stage: {stageId}", Color.red);
                return null;
            }

            var waveDataTableRows = _waveDataTable
                .FindAll<WaveDataTableRow>(row => row.isEnable && row.stageID == stageId)
                .OrderBy(row => row.order).ToList();
            
            var waveContexts = new List<WaveContext>();
            foreach (var waveDataTableRow in waveDataTableRows)
            {
                var turnDataTableRows = _turnDataTable
                    .FindAll<TurnDataTableRow>(row => row.isEnable && row.waveID == waveDataTableRow.rowID)
                    .OrderBy(row => row.order).ToList();
                
                var turnContexts = new List<TurnContext>();

                foreach (var turnDataTableRow in turnDataTableRows)
                {
                    var enemyDataTableRow = _enemyDataTable.Find<EnemyDataTableRow>(row => row.isEnable && row.rowID == turnDataTableRow.enemyID);
                    if (enemyDataTableRow == null)
                    {
                        DebugExtension.LogColor($"StageBuilder: Enemy not found. " +
                                                $"Enemy ID: {turnDataTableRow.enemyID}, " +
                                                $"Wave ID: {waveDataTableRow.rowID}, " +
                                                $"Turn ID: {turnDataTableRow.rowID}", Color.red);

                        continue;
                    }
                    
                    turnContexts.Add(new TurnContext(turnDataTableRow.enemyID, 
                                                     turnDataTableRow.spawnCount, 
                                                     turnDataTableRow.startTime, 
                                                     turnDataTableRow.spawnInterval));
                }
                
                waveContexts.Add(new WaveContext(waveDataTableRow.rowID, 
                                                 waveDataTableRow.endDelay, 
                                                 turnContexts));
            }

            return new StageContext(stageDataTableRow.rowID, 
                                    stageDataTableRow.maxLife, 
                                    stageDataTableRow.rewardGold,
                                    stageDataTableRow.mapID, waveContexts);
        }
    }
}
