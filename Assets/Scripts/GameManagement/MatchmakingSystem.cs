using UnityEngine;
using System.Collections.Generic;

namespace Frontline.GameManagement
{
    /// <summary>
    /// Skill-based matchmaking system concept for the tank game
    /// This is a prototype implementation showing the matchmaking logic
    /// </summary>
    [System.Serializable]
    public class PlayerData
    {
        public string playerId;
        public string playerName;
        public float skillRating;
        public int wins;
        public int losses;
        public float averageDamage;
        public int gamesPlayed;
    }
    
    public class MatchmakingSystem : MonoBehaviour
    {
        [Header("Matchmaking Settings")]
        [SerializeField] private float skillRangeStart = 100f;
        [SerializeField] private float skillRangeIncrease = 50f;
        [SerializeField] private float maxWaitTime = 60f;
        [SerializeField] private int playersPerMatch = 20; // 10v10
        
        private List<PlayerData> waitingPlayers = new List<PlayerData>();
        private Dictionary<string, float> playerWaitTimes = new Dictionary<string, float>();
        
        public System.Action<List<PlayerData>> OnMatchFound;
        
        private void Update()
        {
            // Update wait times and check for matches
            UpdateWaitTimes();
            CheckForMatches();
        }
        
        public void AddPlayerToQueue(PlayerData player)
        {
            if (!waitingPlayers.Contains(player))
            {
                waitingPlayers.Add(player);
                playerWaitTimes[player.playerId] = 0f;
                Debug.Log($"Player {player.playerName} added to matchmaking queue. Skill: {player.skillRating}");
            }
        }
        
        public void RemovePlayerFromQueue(PlayerData player)
        {
            waitingPlayers.Remove(player);
            playerWaitTimes.Remove(player.playerId);
            Debug.Log($"Player {player.playerName} removed from matchmaking queue.");
        }
        
        private void UpdateWaitTimes()
        {
            var playersToUpdate = new List<string>(playerWaitTimes.Keys);
            foreach (var playerId in playersToUpdate)
            {
                playerWaitTimes[playerId] += Time.deltaTime;
            }
        }
        
        private void CheckForMatches()
        {
            if (waitingPlayers.Count < playersPerMatch)
                return;
                
            List<PlayerData> matchedPlayers = FindMatch();
            if (matchedPlayers.Count >= playersPerMatch)
            {
                CreateMatch(matchedPlayers);
            }
        }
        
        private List<PlayerData> FindMatch()
        {
            List<PlayerData> potentialMatch = new List<PlayerData>();
            
            // Sort players by skill rating
            waitingPlayers.Sort((a, b) => a.skillRating.CompareTo(b.skillRating));
            
            // Try to find a group of players with similar skill levels
            for (int i = 0; i < waitingPlayers.Count; i++)
            {
                PlayerData basePlayer = waitingPlayers[i];
                float waitTime = playerWaitTimes[basePlayer.playerId];
                float skillRange = skillRangeStart + (skillRangeIncrease * (waitTime / maxWaitTime));
                
                List<PlayerData> skillGroup = new List<PlayerData>();
                
                // Find players within skill range
                foreach (var player in waitingPlayers)
                {
                    float skillDifference = Mathf.Abs(player.skillRating - basePlayer.skillRating);
                    if (skillDifference <= skillRange)
                    {
                        skillGroup.Add(player);
                    }
                }
                
                // If we have enough players, create match
                if (skillGroup.Count >= playersPerMatch)
                {
                    potentialMatch = skillGroup.GetRange(0, playersPerMatch);
                    break;
                }
            }
            
            return potentialMatch;
        }
        
        private void CreateMatch(List<PlayerData> matchedPlayers)
        {
            Debug.Log($"Match found! {matchedPlayers.Count} players matched.");
            
            // Remove matched players from queue
            foreach (var player in matchedPlayers)
            {
                RemovePlayerFromQueue(player);
            }
            
            // Balance teams by skill
            List<PlayerData> teamA, teamB;
            BalanceTeams(matchedPlayers, out teamA, out teamB);
            
            // Log team compositions
            Debug.Log("Team A:");
            foreach (var player in teamA)
            {
                Debug.Log($"  {player.playerName} - Skill: {player.skillRating}");
            }
            
            Debug.Log("Team B:");
            foreach (var player in teamB)
            {
                Debug.Log($"  {player.playerName} - Skill: {player.skillRating}");
            }
            
            // Notify that match is ready
            OnMatchFound?.Invoke(matchedPlayers);
        }
        
        private void BalanceTeams(List<PlayerData> players, out List<PlayerData> teamA, out List<PlayerData> teamB)
        {
            // Sort players by skill rating (descending)
            players.Sort((a, b) => b.skillRating.CompareTo(a.skillRating));
            
            teamA = new List<PlayerData>();
            teamB = new List<PlayerData>();
            
            float teamASkill = 0f;
            float teamBSkill = 0f;
            
            // Distribute players to balance total skill
            foreach (var player in players)
            {
                if (teamASkill <= teamBSkill && teamA.Count < playersPerMatch / 2)
                {
                    teamA.Add(player);
                    teamASkill += player.skillRating;
                }
                else if (teamB.Count < playersPerMatch / 2)
                {
                    teamB.Add(player);
                    teamBSkill += player.skillRating;
                }
                else
                {
                    teamA.Add(player);
                    teamASkill += player.skillRating;
                }
            }
            
            Debug.Log($"Team A average skill: {teamASkill / teamA.Count:F1}");
            Debug.Log($"Team B average skill: {teamBSkill / teamB.Count:F1}");
        }
        
        public void UpdatePlayerStats(PlayerData player, bool won, float damageDealt)
        {
            if (won)
            {
                player.wins++;
                player.skillRating += 25f; // Win bonus
            }
            else
            {
                player.losses++;
                player.skillRating -= 15f; // Loss penalty
            }
            
            player.gamesPlayed++;
            
            // Update average damage
            player.averageDamage = ((player.averageDamage * (player.gamesPlayed - 1)) + damageDealt) / player.gamesPlayed;
            
            // Performance-based adjustments
            float performanceMultiplier = damageDealt / 1000f; // Assuming 1000 is average damage
            if (won)
            {
                player.skillRating += performanceMultiplier * 10f;
            }
            else
            {
                player.skillRating += performanceMultiplier * 5f; // Small bonus for good performance in loss
            }
            
            // Clamp skill rating
            player.skillRating = Mathf.Clamp(player.skillRating, 0f, 3000f);
            
            Debug.Log($"Updated {player.playerName}: Skill {player.skillRating:F1}, W/L {player.wins}/{player.losses}");
        }
        
        // Prototype method to create sample players
        public PlayerData CreateSamplePlayer(string name, float baseSkill)
        {
            return new PlayerData
            {
                playerId = System.Guid.NewGuid().ToString(),
                playerName = name,
                skillRating = baseSkill + Random.Range(-100f, 100f),
                wins = Random.Range(0, 50),
                losses = Random.Range(0, 50),
                averageDamage = Random.Range(500f, 1500f),
                gamesPlayed = Random.Range(10, 100)
            };
        }
    }
}