
using UnityEngine;

namespace lofi.RLCore
{
    [CreateAssetMenu(fileName = "GameStateType", menuName = "SO/GameState/GameStateType/GameStateType")]
    public class GameStateType : ScriptableObject
    {
        [SerializeField]
        string Type;
    }
}