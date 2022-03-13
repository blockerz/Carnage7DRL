
using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName = "GameStateData", menuName = "SO/GameState/GameStateData")]
    public class GameStateData : ScriptableObject
    {
        [SerializeField]
        GameStateType Type;

        [SerializeField]
        string Description;

    }
}
