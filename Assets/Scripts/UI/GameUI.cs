using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Frontline.UI
{
    /// <summary>
    /// Main game UI controller for health, ammo, crosshair, and game info
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("Health UI")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        
        [Header("Ammo UI")]
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI reloadText;
        
        [Header("Game Info")]
        [SerializeField] private TextMeshProUGUI matchTimerText;
        [SerializeField] private TextMeshProUGUI teamScoreText;
        
        [Header("Crosshair")]
        [SerializeField] private Image crosshair;
        
        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverText;
        
        private Tank.TankHealth playerHealth;
        private Tank.TankShooting playerShooting;
        private GameManagement.GameManager gameManager;
        
        private void Start()
        {
            // Find player tank components
            GameObject playerTank = GameObject.FindWithTag("Player");
            if (playerTank != null)
            {
                playerHealth = playerTank.GetComponent<Tank.TankHealth>();
                playerShooting = playerTank.GetComponent<Tank.TankShooting>();
                
                // Subscribe to health changes
                if (playerHealth != null)
                {
                    playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
                    playerHealth.OnTankDestroyed.AddListener(OnPlayerDestroyed);
                }
            }
            
            // Find game manager
            gameManager = Object.FindAnyObjectByType<GameManagement.GameManager>();
            
            // Initialize UI
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
        }
        
        private void Update()
        {
            UpdateAmmoUI();
            UpdateGameInfoUI();
        }
        
        private void UpdateHealthUI(float currentHealth)
        {
            if (playerHealth == null) return;
            
            if (healthBar != null)
            {
                healthBar.value = playerHealth.HealthPercentage;
            }
            
            if (healthText != null)
            {
                healthText.text = $"{currentHealth:F0}/{playerHealth.MaxHealth:F0}";
            }
        }
        
        private void UpdateAmmoUI()
        {
            if (playerShooting == null) return;
            
            if (ammoText != null)
            {
                ammoText.text = $"Ammo: {playerShooting.CurrentAmmo}/{playerShooting.MaxAmmo}";
            }
            
            if (reloadText != null)
            {
                reloadText.gameObject.SetActive(playerShooting.IsReloading);
                if (playerShooting.IsReloading)
                {
                    reloadText.text = "RELOADING...";
                }
            }
        }
        
        private void UpdateGameInfoUI()
        {
            if (gameManager == null) return;
            
            if (matchTimerText != null)
            {
                float timeRemaining = gameManager.MatchTimeRemaining;
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                matchTimerText.text = $"{minutes:00}:{seconds:00}";
            }
            
            if (teamScoreText != null)
            {
                teamScoreText.text = $"Team A: {gameManager.TeamAAlive} | Team B: {gameManager.TeamBAlive}";
            }
        }
        
        private void OnPlayerDestroyed()
        {
            ShowGameOver("You were destroyed!");
        }
        
        public void ShowGameOver(string message)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            
            if (gameOverText != null)
            {
                gameOverText.text = message;
            }
        }
        
        public void RestartGame()
        {
            if (gameManager != null)
            {
                gameManager.StartMatch();
            }
            
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged.RemoveListener(UpdateHealthUI);
                playerHealth.OnTankDestroyed.RemoveListener(OnPlayerDestroyed);
            }
        }
    }
}