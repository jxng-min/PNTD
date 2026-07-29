using System.Collections.Generic;
using JxModule;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PNTD
{
    public class MapGenerator : MonoBehaviour
    {
        [BigHeader("Map Settings")]
        [Header("Tilemap")]
        [SerializeField] private Tilemap buildTilemap;
        [SerializeField] private Tilemap pointTilemap;
        
        [Header("Point Tiles")]
        [SerializeField] private TileBase spawnTile;
        [SerializeField] private TileBase destinationTile;
        [SerializeField] private TileBase pathTile;
        [SerializeField] private TileBase cornerTile;
        
        [Space(30f)]
        [BigHeader("Path References")]
        [SerializeField] private Transform pathRoot;
        [SerializeField] private StagePath stagePath;

#if UNITY_EDITOR
        [ContextMenu("PNTD/Generate Map")]
        public void GenerateMap()
        {
            ClearChildren(pathRoot);

            Transform generatedSpawnPoint = null;
            Transform generatedDestinationPoint = null;
            
            List<Transform> generatedCornerPoints = GeneratePoints(ref generatedSpawnPoint, ref generatedDestinationPoint);

            if (stagePath != null)
            {
                stagePath.SetPath(generatedSpawnPoint, generatedDestinationPoint, generatedCornerPoints.ToArray());
            }
            
            UnityEditor.EditorUtility.SetDirty(this);
            if (stagePath != null)
            {
                EditorUtility.SetDirty(stagePath);
            }
            
            DebugExtension.LogColor("성공적으로 스테이지를 생성했습니다.", Color.green);
        }
#endif

        private List<Transform> GeneratePoints(ref Transform generatedSpawnPoint, ref Transform generatedDestinationPoint)
        {
            List<Transform> generatedCornerPoints = new();

            if (pointTilemap == null || pathRoot == null)
            {
                return generatedCornerPoints;
            }

            Vector3Int? spawnCell = null;
            Vector3Int? destinationCell = null;
            
            var bounds = pointTilemap.cellBounds;
            for (var x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (var y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var cellPosition = new Vector3Int(x, y, 0);
                    
                    var tile = pointTilemap.GetTile(cellPosition);
                    if (tile == null)
                    {
                        continue;
                    }
                    
                    var worldPosition = pointTilemap.GetCellCenterWorld(cellPosition);

                    if (tile == spawnTile)
                    {
                        var spawnPoint = new GameObject("Spawn Point");
                        spawnPoint.transform.SetParent(pathRoot);
                        spawnPoint.transform.position = worldPosition;
                        generatedSpawnPoint = spawnPoint.transform;
                        spawnCell = cellPosition;
                    }
                    else if (tile == destinationTile)
                    {
                        var destinationPoint = new GameObject("Destination Point");
                        destinationPoint.transform.SetParent(pathRoot);
                        destinationPoint.transform.position = worldPosition;
                        generatedDestinationPoint = destinationPoint.transform;
                        destinationCell = cellPosition;
                    }
                }
            }

            if (spawnCell == null)
            {
                DebugExtension.LogColor("스테이지에서 스폰 포인트를 찾는 데 실패했습니다..", Color.red);
                return generatedCornerPoints;
            }

            if (destinationCell == null)
            {
                DebugExtension.LogColor("스테이지에서 목적 포인트를 찾는 데 실패했습니다.", Color.red);
                return generatedCornerPoints;
            }

            generatedCornerPoints = GenerateCornerPoints(spawnCell.Value, destinationCell.Value);
            return generatedCornerPoints;
        }

        private List<Transform> GenerateCornerPoints(Vector3Int spawnPosition, Vector3Int destinationPosition)
        {
            List<Transform> generatedCornerPoints = new();
            
            var currentPosition = spawnPosition;
            var prevPosition = new Vector3Int(int.MinValue, int.MinValue, 0);

            var safety = 0;
            const int maxStep = 10000;

            while (currentPosition != destinationPosition)
            {
                safety++;
                if (safety > maxStep)
                {
                    DebugExtension.LogColor($"경로 탐색 횟수가 {maxStep}회를 초과했습니다. 순환 경로를 확인해주세요.", Color.red);
                    break;
                }

                var nextCandidates = GetConnectedPathCells(currentPosition, prevPosition);
                if (nextCandidates.Count == 0)
                {
                    DebugExtension.LogColor($"경로가 끊어졌습니다. 위치: {currentPosition}", Color.red);
                    break;
                }

                if (nextCandidates.Count > 1)
                {
                    DebugExtension.LogColor($"경로가 분기되어 있습니다. 위치: {currentPosition}", Color.red);
                    break;
                }
                
                
                var nextPosition = nextCandidates[0];
                var nextTile = pointTilemap.GetTile(nextPosition);
                if (nextTile == cornerTile)
                {
                    var worldPosition = pointTilemap.GetCellCenterWorld(nextPosition);

                    var cornerPoint = new GameObject($"Corner Point [{generatedCornerPoints.Count:00}]");
                    cornerPoint.transform.SetParent(pathRoot);
                    cornerPoint.transform.position = worldPosition;
                    
                    generatedCornerPoints.Add(cornerPoint.transform);
                }
                
                prevPosition = currentPosition;
                currentPosition = nextPosition;
            }
            
            return generatedCornerPoints;
        }

        private List<Vector3Int> GetConnectedPathCells(Vector3Int currentPosition, Vector3Int prevPosition)
        {
            List<Vector3Int> result = new();

            Vector3Int[] directions =
            {
                Vector3Int.right,
                Vector3Int.left,
                Vector3Int.up,
                Vector3Int.down,
            };

            foreach (var direction in directions)
            {
                var nextPosition = currentPosition + direction;
                if (nextPosition == prevPosition)
                {
                    continue;
                }
                
                var tile = pointTilemap.GetTile(nextPosition);
                if (tile == pathTile || tile == cornerTile || tile == destinationTile)
                {
                    result.Add(nextPosition);
                }
            }

            return result;
        }
        
        private void ClearChildren(Transform parent)
        {
            if (parent == null)
            {
                return;
            }

            List<GameObject> children = new();
            for (var i = 0; i < parent.childCount; i++)
            {
                children.Add(parent.GetChild(i).gameObject);
            }

            foreach (var child in children)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    DestroyImmediate(child);
                }
                else
                {
                    Destroy(child);
                }
#else
                Destroy(child);
#endif
            }
        }
    }
}