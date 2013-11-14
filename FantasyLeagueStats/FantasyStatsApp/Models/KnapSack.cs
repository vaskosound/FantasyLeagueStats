using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FantasyStatsApp.Models
{
    public class Knapsack
    {
        private PlayerValuableModel[] players; // Input list of items 
        private decimal capacity; // Max weight allowed in knapsack
        public int[] bestSolution; // Best solution array: item i in if xi=1 
        private int[] workSolution; // Working solution array at current tree node 
        private decimal solutionProfit = -1; // Profit of best solution so far 
        private decimal currWgt; // Weight of solution at this tree node 
        private decimal currProfit; // Profit of solution at this tree node 
        private decimal newWgt; // Weight of solution from bound() method 
        private decimal newProfit; // Profit of solution from bound() method 
        private int k = -1; // Level of tree in knapsack() method 
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
            int playersCount = this.players.Length;
            do
            { // While upper bound < known soln,backtrack 
                while (Bound() <= solutionProfit)
                {
                    while (k != 0 && this.workSolution[k] != 1) // Back up while item k not in sack 
                    {
                        k--; // to find last object in knapsack 
                    }
                    if (k == 0) // If at root,, we’re done. Return. 
                        return;
                    this.workSolution[k] = 0; // Else take k out of soln (R branch) 
                    currWgt -= this.players[k].Weight; // Reduce soln wgt by k’s wgt 
                    currProfit -= this.players[k].Value; // Reduce soln profit by k’s prof 
                }
                currWgt = newWgt; // Reach here if bound> soln profit 
                currProfit = newProfit; // and we may have new soln. 
                k = this.partItem; // Set tree level k to last, possibly 
                // partial it em i n greed y solution 
                if (k == playersCount)
                { // If we’ve reached leaf node, have 
                    solutionProfit = currProfit; // actual soln, not just bound 
                    Array.Copy(this.workSolution, 0, this.bestSolution, 0, this.workSolution.Length); // Copy soln into array bestSolution
                    k = playersCount - 1; // Back up to prev tree level, which may leave solution 
                }
                else // Else not at leaf, just have bound 
                    this.workSolution[k] = 0; // Take last item k out of soln 
            } while (true);
        }

        private decimal Bound()
        {
            bool found = false; // Was bound found?I.e.,is last item partial 
            decimal boundVal = -1; // Value of upper bound 
            int playersCount = this.players.Length; // Number of items in problem 
            newProfit = currProfit; // Set new prof as current prof at this node 
            newWgt = currWgt;
            partItem = k + 1; // Go to next lower level,, try to put in soln 
            List<PlayerValuableModel> missedPlayers = new List<PlayerValuableModel>();
            while (partItem < playersCount && !found) // More items & haven’t found partial 
            {
                if (newWgt + this.players[partItem].Weight <= capacity)
                {
                    this.workSolution[partItem] = 1; // Update curr soln to show item k is in it 
                    if (IsValidPositions() && IsValidPlayersFromTeam())
                    {
                        newWgt += this.players[partItem].Weight; // Update new wgt, prof 
                        newProfit += this.players[partItem].Value; // by adding item wgt,prof 
                    }
                    else
                    {
                        this.workSolution[partItem] = 0;
                        SearchInMissedPlayers(missedPlayers);
                        missedPlayers.Add(this.players[partItem]);
                    }                   
                }
                else
                {
                    if (GetPlayersInSolution() < 15)
                    {
                        int mostExpensivePlayerIndex = GetMostExpensivePlayer();
                        this.workSolution[mostExpensivePlayerIndex] = 0;
                        newWgt -= this.players[mostExpensivePlayerIndex].Weight; // Update new wgt, prof 
                        newProfit -= this.players[mostExpensivePlayerIndex].Value; // by adding item wgt,prof                        
                        partItem--;

                    }
                    else
                    {
                        boundVal = newProfit + (this.capacity - newWgt) *
                            this.players[partItem].Value / this.players[partItem].Weight;
                        found = true;
                    }
                } // Compute upper b ound b ased on partial fit 
                partItem++; // Go to next item and try to put in sack 
            }
            if (found)
            {
                partItem--; // Back up to prev item, which is fully in sack 
                return boundVal; // Return the upper bound 
            }
            else
            {
                return newProfit;
            }
        }

        private void SearchInMissedPlayers(List<PlayerValuableModel> missedPlayers)
        {
            for (int i = 0; i < missedPlayers.Count; i++)
            {
                var player = this.players.FirstOrDefault(p => p.Id == missedPlayers[i].Id);
                if (newWgt + player.Weight <= capacity)
                {
                    int index = Array.IndexOf(this.players, player);
                    this.workSolution[index] = 1;
                    if (IsValidPositions() && IsValidPlayersFromTeam())
                    {
                        newWgt += player.Weight;
                        newProfit += player.Value;
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

        private int GetMostExpensivePlayer()
        {
            decimal maxPrice = 0;
            int currentIndex = 0;
            for (int i = 0; i < this.workSolution.Length; i++)
            {
                if (this.workSolution[i] == 1)
                {
                    decimal playerPrice = this.players[i].Weight;
                    if (maxPrice < playerPrice)
                    {
                        maxPrice = playerPrice;
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

        private bool IsValidPositions()
        {
            Dictionary<int, int> positions = new Dictionary<int, int>();
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

            foreach (var postion in positions)
            {
                if (postion.Key == 0 && postion.Value > 2)
                {
                    return false;
                }

                if (postion.Key == 1 && postion.Value > 5)
                {
                    return false;
                }

                if (postion.Key == 2 && postion.Value > 5)
                {
                    return false;
                }

                if (postion.Key == 3 && postion.Value > 3)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsValidPlayersFromTeam()
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