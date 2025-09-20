using UnityEngine;
using System.Collections.Generic;
using Frontline.Tank;

namespace Frontline.GameManagement
{
    /// <summary>
    /// Main game manager handling game state, spawning, and match flow
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private int playersPerTeam = 10;
        [SerializeField] private float matchDuration = 600f; // 10 minutes
        [SerializeField] private Transform[] teamASpawnPoints;
        [SerializeField] private Transform[] teamBSpawnPoints;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject playerTankPrefab;
        [SerializeField] private GameObject aiTankPrefab;
        
        private List<GameObject> teamATanks = new List<GameObject>();
        private List<GameObject> teamBTanks = new List<GameObject>();
        private float matchTimer;
        private bool isMatchActive = false;
        
        public enum GameState
        {
            WaitingForPlayers,
            CountingDown,
            InMatch,
            MatchEnded
        }
        
        private GameState currentState = GameState.WaitingForPlayers;
        
        public GameState CurrentState => currentState;
        public float MatchTimeRemaining => Mathf.Max(0, matchDuration - matchTimer);
        public int TeamAAlive => CountAliveTanks(teamATanks);
        public int TeamBAlive => CountAliveTanks(teamBTanks);
        
        private void Start()
        {
            // For prototype, start a match immediately with AI tanks
            StartMatch();
        }
        
        private void Update()
        {
            if (isMatchActive)
            {
                matchTimer += Time.deltaTime;
                
                // Check win conditions
                CheckWinConditions();
                
                // Check time limit
                if (matchTimer >= matchDuration)
                {
                    EndMatch("Time's up! Draw!");
                }
            }
        }
        
        public void StartMatch()
        {
            currentState = GameState.InMatch;
            isMatchActive = true;
            matchTimer = 0f;
            
            SpawnTanks();
        }
        
        private void SpawnTanks()
        {
            // Clear existing tanks
            teamATanks.Clear();
            teamBTanks.Clear();
            
            // For prototype, spawn 1 player tank and several AI tanks
            SpawnPlayerTank();
            SpawnAITanks();
        }
        
        private void SpawnPlayerTank()
        {
            if (teamASpawnPoints == null || teamASpawnPoints.Length == 0)
            {
                Debug.LogError("GameManager: No Team A spawn points assigned! Cannot spawn player tank.");
                return;
            }
            
            if (playerTankPrefab == null)
            {
                Debug.LogError("GameManager: Player tank prefab not assigned! Cannot spawn player tank.");
                return;
            }
            
            Transform spawnPoint = teamASpawnPoints[0];
            if (spawnPoint == null)
            {
                Debug.LogError("GameManager: Team A spawn point 0 is null! Cannot spawn player tank.");
                return;
            }
            
            GameObject playerTank = Instantiate(playerTankPrefab, spawnPoint.position, spawnPoint.rotation);
            teamATanks.Add(playerTank);
            
            // Tag as player
            playerTank.tag = "Player";
        }
        
        private void SpawnAITanks()
        {
            if (teamBSpawnPoints == null || teamBSpawnPoints.Length == 0)
            {
                Debug.LogError("GameManager: No Team B spawn points assigned! Cannot spawn AI tanks.");
                return;
            }
            
            if (aiTankPrefab == null)
            {
                Debug.LogError("GameManager: AI tank prefab not assigned! Cannot spawn AI tanks.");
                return;
            }
            
            // Spawn AI tanks for team B
            int tanksToSpawn = Mathf.Min(3, teamBSpawnPoints.Length);
            for (int i = 0; i < tanksToSpawn; i++)
            {
                Transform spawnPoint = teamBSpawnPoints[i];
                if (spawnPoint == null)
                {
                    Debug.LogWarning($"GameManager: Team B spawn point {i} is null! Skipping this spawn.");
                    continue;
                }
                
                GameObject aiTank = Instantiate(aiTankPrefab, spawnPoint.position, spawnPoint.rotation);
                teamBTanks.Add(aiTank);
                
                // Add AI controller
                var aiController = aiTank.GetComponent<AITankController>();
                if (aiController == null)
                {
                    aiController = aiTank.AddComponent<AITankController>();
                }
            }
        }
        
        private void CheckWinConditions()
        {
            int teamAAlive = TeamAAlive;
            int teamBAlive = TeamBAlive;
            
            if (teamAAlive == 0 && teamBAlive > 0)
            {
                EndMatch("Team B Wins!");
            }
            else if (teamBAlive == 0 && teamAAlive > 0)
            {
                EndMatch("Team A Wins!");
            }
            else if (teamAAlive == 0 && teamBAlive == 0)
            {
                EndMatch("Draw! All tanks destroyed!");
            }
        }
        
        private int CountAliveTanks(List<GameObject> tanks)
        {
            int count = 0;
            foreach (var tank in tanks)
            {
                if (tank != null)
                {
                    var health = tank.GetComponent<Tank.TankHealth>();
                    if (health != null && !health.IsDead)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        private void EndMatch(string result)
        {
            isMatchActive = false;
            currentState = GameState.MatchEnded;
            
            Debug.Log($"Match Ended: {result}");
            
            // Here you would show end game UI, stats, etc.
            // For now, just restart after a delay
            Invoke(nameof(RestartMatch), 5f);
        }
        
        private void RestartMatch()
        {
            // Clean up existing tanks
            foreach (var tank in teamATanks)
            {
                if (tank != null) Destroy(tank);
            }
            foreach (var tank in teamBTanks)
            {
                if (tank != null) Destroy(tank);
            }
            
            StartMatch();
        }
    }
}