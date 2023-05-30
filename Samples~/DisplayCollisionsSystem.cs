using System.Text;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using TMPro;
using Unity.Profiling;

namespace CellListsECS.Samples
{
    public class DisplayCollisionsSystem : IEcsRunSystem
    {
        private readonly EcsCustomInject<TMP_Text> _text = default;
        private readonly EcsFilterInject<Inc<Collision>> _filter = "Collisions";
        private readonly EcsPoolInject<Collision> _collisions = "Collisions";

        private readonly StringBuilder _stringBuilder = new ();
        private static readonly ProfilerMarker Visualization = new(nameof(Visualization));
        
        public void Run(IEcsSystems systems)
        {
            Visualization.Begin();
            _stringBuilder.Clear();
            
            foreach (var entity in _filter.Value)
            {
                ref var collision = ref _collisions.Value.Get(entity);
                _stringBuilder.Append($"{collision.Sender.ToString()}:{collision.Receiver.ToString()},   ");
            }

            _text.Value.text = _stringBuilder.ToString();
            Visualization.End();
        }
    }
}