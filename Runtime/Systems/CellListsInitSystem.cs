using System.Collections.Generic;
using CellListsECS.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace CellListsECS.Runtime.Systems
{
    public class CellListsInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<CellListsConfig> _cellListsConfig = default;
        private readonly EcsPoolInject<Cell> _cells = default;
        private readonly EcsPoolInject<CellNeighbours> _neighbours = default;

        public void Init(IEcsSystems systems)
        {
            var config = _cellListsConfig.Value;

            var length = config.Width * config.Height;
            var size = config.Size;
            var width = config.Width;
            var height = config.Height;
            var centerPosition = config.Center;
            var overall = new AABB
            {
                Size = size
            };
            var cellSize = new Vector2(size.x / width, size.y / height);
            var cells = new (Cell, int)[length]; // cell, entity
            var overallHalfExtents = overall.HalfExtents;
            var startPosition = new Vector2(centerPosition.x - overallHalfExtents.x,
                centerPosition.y + overallHalfExtents.y);
            var currentPosition = startPosition;
            var right = cellSize.x;
            var down = -cellSize.y;
            var currentColumn = 0;


            for (var i = 0; i < length; i++)
            {
                var cell = new Cell()
                {
                    AABB = new AABB()
                    {
                        Size = cellSize
                    },
                    Position = currentPosition
                };
                var cellEntity = systems.GetWorld().NewEntity();
                cells[i] = (cell, cellEntity);

                currentPosition.x += right;
                currentColumn++;
                if (currentColumn == width)
                {
                    currentPosition.x = startPosition.x;
                    currentPosition.y += down;
                    currentColumn = 0;
                }
            }
            
            // pure madness
            int[] GetClosestIndexesFor(int i)
            {
                var leftIndex = i - 1;
                var rightIndex = i + 1;
                var upIndex = i - width;
                var downIndex = i + width;
                var leftUpIndex = i - width - 1;
                var rightUpIndex = i - (width - 1);
                var rightDownIndex = i + width + 1;
                var leftDownIndex = i + (width - 1);
                var leftExist = i % width != 0 && leftIndex >= 0;
                var rightExist = rightIndex % width != 0 && rightIndex < length;
                var upExist = upIndex >= 0;
                var downExist = downIndex < length;
                var leftUpExist = leftUpIndex >= 0 && i % width != 0;
                var rightUpExist = rightUpIndex % width != 0 && rightUpIndex >= 0;
                var rightDownExist = rightDownIndex < length && rightDownIndex % width != 0;
                var leftDownExist = leftDownIndex < length && i % width != 0;
                var list = new List<int>();

                if (rightExist)
                    list.Add(rightIndex);
                if (leftExist)
                    list.Add(leftIndex);
                if (upExist)
                    list.Add(upIndex);
                if (downExist)
                    list.Add(downIndex);
                if (leftUpExist)
                    list.Add(leftUpIndex);
                if (rightUpExist)
                    list.Add(rightUpIndex);
                if (rightDownExist)
                    list.Add(rightDownIndex);
                if (leftDownExist)
                    list.Add(leftDownIndex);

                return list.ToArray();
            }

            for (var i = 0; i < length; i++)
            {
                var neighbours = GetClosestIndexesFor(i);
                var list = new int[neighbours.Length];
                for (var j = 0; j < neighbours.Length; j++)
                {
                    var (_, neighbourEntity) = cells[neighbours[j]];
                    list[j] = neighbourEntity;
                }

                ref var neighboursComponent = ref _neighbours.Value.Add(cells[i].Item2);

                neighboursComponent.NeighboursEntities = list;
            }

            foreach (var (cell, cellEntity) in cells)
            {
                ref var cellComponent = ref _cells.Value.Add(cellEntity);
                cellComponent = cell;
                ref var neighbours = ref _neighbours.Value.Get(cellEntity);
                neighbours.ContainingTransforms = new List<int>();
            }
        }
    }
}