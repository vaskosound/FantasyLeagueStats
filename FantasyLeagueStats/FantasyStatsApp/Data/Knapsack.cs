using FantasyStats.Model;
using FantasyStatsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasyStatsApp.Data
{
    public class Knapsack
    {
        private PlayerValuableModel[] players; // Input list of items 
        private decimal capacity; // Max weight allowed in knapsack
        public int[] bestSolution; // Best solution array
        private int[] workSolution; // Working solution array at current tree node 
        private decimal solutionValue = -1; // Value of best solution so far 
        private decimal solutionWeight; // Weight of best solution so far 
        private decimal currentWeight; // Weight of solution at this tree node 
        private decimal currentValue; // Value of solution at this tree node 
        private decimal newWeight; // Weight of solution from bound() method 
        private decimal newValue; // Vaule of solution from bound() method 
        private int k = -2; // Level of tree in knapsack() method 
        private int partItem; // Level of tree in bound() method 


        public Knapsack(PlayerValuableModel[] players, decimal capacity)
        {
            this.players = players;
            this.capacity = capacity;
            this.bestSolution = new int[players.Length];
            this.workSolution = new int[players.Length];
        }

        public void KnapsackProblem()
        {
            do // While upper bound < known soln,backtrack 
            {
                while (Bound() <= solutionValue)
                {
                    Array.Copy(this.bestSolution, 0, this.workSolution, 0, this.workSolution.Length);
                    currentWeight = solutionWeight;
                    currentValue = solutionValue;
                    while (k != 0 && this.workSolution[k] != 1) // Back up while item k not in sack 
                    {
                        k--; // to find last object in knapsack 
                    }
                    if (k == 0) // If at root, we’re done. 
                        return;
                    this.workSolution[k] = 0; // Else take k out of soln (R branch) 
                    currentWeight -= this.players[k].Weight; // Reduce soln weight by k’s weight 
                    currentValue -= this.players[k].Value; // Reduce soln value by k’s value 
                    k--;
                }

                k = this.partItem; // Set tree level k to last, possible 
                currentWeight = newWeight; // Reach here if bound > soln value 
                currentValue = newValue; // and we may have new soln.                 

                if (GetPlayersInSolution() == 15)
                {
                    solutionValue = currentValue;
                    solutionWeight = currentWeight;
                    Array.Copy(this.workSolution, 0, this.bestSolution, 0, this.workSolution.Length); // Copy soln into array bestSolution    
                }
            } while (true);
        }

        private decimal Bound()
        {
            bool found = false; // Was bound found?I.e.,is last item partial 
            int playersCount = this.players.Length; // Number of items in problem 
            newValue = currentValue;
            newWeight = currentWeight;
            partItem = 0;
            List<PlayerValuableModel> missedPlayers = new List<PlayerValuableModel>();
            while (partItem < playersCount && !found)
            {
                if (newWeight + this.players[partItem].Weight <= capacity && GetPlayersInSolution() < 15)
                {
                    if (this.workSolution[partItem] == 1 || partItem == k + 1 || players[partItem].IsInjured)
                    {
                        partItem++;
                        continue;
                    }

                    this.workSolution[partItem] = 1; // Update current soln 
                    if (IsValidPositions() && IsValidPlayersCountFromTeam())
                    {
                        newWeight += this.players[partItem].Weight;
                        newValue += this.players[partItem].Value;
                    }
                    else
                    {
                        this.workSolution[partItem] = 0;
                        missedPlayers.Add(this.players[partItem]);
                    }
                }
                else
                {
                    if (GetPlayersInSolution() < 15) // remove player when team is not filled
                    {
                        if (k < 0)
                        {
                            int mostExpensivePlayerIndex = GetAtLeastValuablePlayer();
                            this.workSolution[mostExpensivePlayerIndex] = 0;
                            newWeight -= this.players[mostExpensivePlayerIndex].Weight;
                            newValue -= this.players[mostExpensivePlayerIndex].Value;
                            SearchInMissedPlayers(missedPlayers);
                            partItem--;
                        }
                    }
                    else
                    {
                        found = true;
                    }
                }
                partItem++;
            }
            if (found)
            {
                partItem--; // Back up to prev item, which is fully in sack 
                return newValue; // Return the upper bound 
            }
            else
            {
                return newValue;
            }
        }

        public List<PlayerValuableModel> OutputSolution()
        {
            List<PlayerValuableModel> bestPlayers = new List<PlayerValuableModel>();

            for (int i = 0; i < this.bestSolution.Length; i++)
            {
                if (this.bestSolution[i] == 1)
                {
                    bestPlayers.Add(this.players[i]);
                }
            }

            return bestPlayers;
        }

        private void SearchInMissedPlayers(List<PlayerValuableModel> missedPlayers)
        {
            for (int i = 0; i < missedPlayers.Count; i++)
            {
                var player = this.players.FirstOrDefault(p => p.Id == missedPlayers[i].Id);
                if (newWeight + player.Weight <= capacity)
                {
                    int index = Array.IndexOf(this.players, player);
                    this.workSolution[index] = 1;
                    if (IsValidPositions() && IsValidPlayersCountFromTeam())
                    {
                        newWeight += player.Weight;
                        newValue += player.Value;
                        missedPlayers.Remove(player);
                        break;
                    }
                    else
                    {
                        this.workSolution[index] = 0;
                    }
                }
            }
        }

        private int GetAtLeastValuablePlayer()
        {
            decimal minValue = int.MaxValue;
            int currentIndex = 0;
            for (int i = 0; i < this.workSolution.Length; i++)
            {
                if (this.workSolution[i] == 1)
                {
                    decimal playerValuable = this.players[i].Value / this.players[i].Weight;
                    if (minValue > playerValuable)
                    {
                        minValue = playerValuable;
                        currentIndex = i;
                    }
                }
            }

            return currentIndex;
        }

        private int GetPlayersInSolution()
        {
            int countPlayers = 0;
            for (int i = 0; i < this.workSolution.Length; i++)
            {
                if (this.workSolution[i] == 1)
                {
                    countPlayers++;
                }
            }

            return countPlayers;
        }

        private bool IsValidPositions()
        {
            Dictionary<Position, int> positions = GetPositionsCount();

            foreach (var postion in positions)
            {
                if (postion.Key == Position.GKP && postion.Value > 2)
                {
                    return false;
                }

                if (postion.Key == Position.DEF && postion.Value > 5)
                {
                    return false;
                }

                if (postion.Key == Position.MID && postion.Value > 5)
                {
                    return false;
                }

                if (postion.Key == Position.FWD && postion.Value > 3)
                {
                    return false;
                }
            }
            return true;
        }

        private Dictionary<Position, int> GetPositionsCount()
        {
            Dictionary<Position, int> positions = new Dictionary<Position, int>();
            for (int i = 0; i < this.players.Length; i++)
            {
                if (this.workSolution[i] == 1)
                {
                    int count = 1;
                    if (positions.ContainsKey(this.players[i].Position))
                    {
                        count = positions[this.players[i].Position] + 1;
                    }
                    positions[this.players[i].Position] = count;

                }
            }
            return positions;
        }

        private bool IsValidPlayersCountFromTeam()
        {
            Dictionary<string, int> teams = new Dictionary<string, int>();
            for (int i = 0; i < this.players.Length; i++)
            {
                if (this.workSolution[i] == 1)
                {
                    int count = 1;
                    if (teams.ContainsKey(this.players[i].Team))
                    {
                        count = teams[this.players[i].Team] + 1;
                    }
                    teams[this.players[i].Team] = count;

                }
            }

            foreach (var playersInTeam in teams)
            {
                if (playersInTeam.Value > 3)
                {
                    return false;
                }
            }

            return true;
        }
    }
}