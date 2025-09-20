using UnityEngine;

namespace Frontline.Setup
{
    /// <summary>
    /// Example setup script showing how to configure the game components
    /// </summary>
    public class GameSetupExample : MonoBehaviour
    {
        [Header("Setup Instructions")]
        [TextArea(5, 10)]
        [SerializeField] private string setupInstructions = 
            "To set up Frontline Tank Combat:\n\n" +
            "1. Create a terrain or plane for the battlefield\n" +
            "2. Add spawn points (empty GameObjects) for teams\n" +
            "3. Create tank prefabs with the tank scripts\n" +
            "4. Set up the camera with CameraController\n" +
            "5. Create UI Canvas with GameUI script\n" +
            "6. Add GameManager to manage the match";

        void Start()
        {
            Debug.Log("Frontline Game Setup Example loaded.");
        }
    }
}