using CellListsECS.Runtime.Components;
using CellListsECS.Runtime.Systems;
using CellListsECS.Samples.Scenes;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Transform = CellListsECS.Runtime.Components.Transform;

namespace CellListsECS.Samples
{
    public class Entry : MonoBehaviour
    {
        [SerializeField] private TMP_Text _collisions;
        [SerializeField] private CellListsConfig _config;
        [SerializeField] private TMP_Text _entitiesCountText;
        [SerializeField] private int _entitiesCountPerBatch = 1000;
        [SerializeField] private int _startEntitiesCount = 10000;
        [SerializeField] private float _spawnRadius = 50f;
        
        private EcsSystems _systems;
        private int _entitiesCount = 0;
        
        private void Start()
        {
            Application.targetFrameRate = 0;
            var world = new EcsWorld();
            var collisionsWorld = new EcsWorld();
            _systems = new EcsSystems(world);
            _systems.AddWorld(collisionsWorld, "Collisions");
            
            _systems
                .Add(new CellListsInitSystem())
                .Add(new InsertTransformSystem())
                .Add(new RemoveFromCellListsSystem())
                .Add(new MoveTransformsSystem())
                .Add(new CellListsRebuildSystem())
                //.DelHere<Collision>("Collisions")
                //.Add(new CollisionDetectionSystem())
                //.Add(new DisplayCollisionsSystem()) // draw collisions
                //.Add(new CellDrawSystem()) // Draw cells
                //.Add(new CellNeighboursDrawSystem()) // Draw cell and neighbours connections
                //.Add(new DisplayNeighboursSystem()) // draw transform neighbours
                .Inject(_collisions, _config)
                .Init();
            
            SpawnPoints(_startEntitiesCount);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnPoints(_entitiesCountPerBatch);
            }
            _systems.Run();
        }

        private void OnDestroy()
        {
            _systems.Destroy();
        }

        private Vector2 RandomPoint()
        {
            return Random.insideUnitCircle * _spawnRadius;
        }

        private void SpawnPoints(int amount)
        {
            var world = _systems.GetWorld();
            var transformsPool = world.GetPool<Transform>();
            var commandsPool = world.GetPool<InsertInCellLists>();
            var movingPointsPool = world.GetPool<MovingPoint>();
            var aabbPool = world.GetPool<AABB>();
            
            for (var i = 0; i < amount; i++)
            {
                var entity = world.NewEntity();
                ref var transform1 = ref transformsPool.Add(entity);
                ref var movingPoint = ref movingPointsPool.Add(entity);
                ref var aabb = ref aabbPool.Add(entity);

                aabb.Size = new Vector2(1f, 1f);
                movingPoint.TimeSinceLastDirectionChange = 0;
                commandsPool.Add(entity);
                transform1.Position = RandomPoint();
            }

            _entitiesCount += amount;
            _entitiesCountText.text = _entitiesCount.ToString();
        }
    }
}