using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AStarScore<T>
    {
        public float FScore;
        public float GScore;
        public T Previous;
        public int LastSearchIndex;
    }

    public interface IAStarNode<T>
    {
        IEnumerable<T> GetAStarNeighbours();
        AStarScore<T> GetStoredAStarScore();
        float GetAStarHeuristic(T destination);
        float GetAStarTraversalCost(T neighbour);
    }

    public static class AStar<T> where T : class, IAStarNode<T>
    {
        private static int _searchIndex = 1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _searchIndex = 1;
        }

        public static List<T> FindPath(T source, T destination)
        {
            if (source == null) return new List<T>();
            if (destination == null) return new List<T>();

            if (_searchIndex == int.MaxValue)
            {
                _searchIndex = int.MinValue;
            }
            else
            {
                _searchIndex++;
            }

            AStarScore<T> sourceScore = source.GetStoredAStarScore();

            sourceScore.FScore = source.GetAStarHeuristic(destination);
            sourceScore.GScore = 0f;
            sourceScore.Previous = null;
            sourceScore.LastSearchIndex = _searchIndex;

            //Add source to open set
            PriorityQueue<T> uncheckedNodes = new PriorityQueue<T>(CompareNodes);
            uncheckedNodes.Enqueue(source);

            while (uncheckedNodes.Count > 0)
            {

                //Get node with lowest F score
                T currentNode = uncheckedNodes.Dequeue();


                //We've found it! 
                if (currentNode == destination)
                {
                    //Reconstruct path
                    List<T> path = new List<T> { source };
                    while (currentNode != source)
                    {
                        path.Insert(1, currentNode);
                        currentNode = currentNode.GetStoredAStarScore().Previous;
                    }
                    return path;
                }

                foreach (T nextNode in currentNode.GetAStarNeighbours())
                {

                    AStarScore<T> nextScore = nextNode.GetStoredAStarScore();

                    // Make sure this cell's costs have been properly reset since the last search
                    if (nextScore.LastSearchIndex != _searchIndex)
                    {
                        nextScore.FScore = Mathf.Infinity;
                        nextScore.GScore = Mathf.Infinity;
                        nextScore.Previous = null;
                        nextScore.LastSearchIndex = _searchIndex;
                    }

                    //Calculate a hypothetical G score if we went to this neighbour via the current node
                    float newCost = currentNode.GetStoredAStarScore().GScore + currentNode.GetAStarTraversalCost(nextNode);

                    //Check if this new way is a better way to reach the neighbour
                    if (newCost < nextScore.GScore)
                    {
                        //Go via the current node and explore this neighbour

                        nextScore.FScore = newCost + nextNode.GetAStarHeuristic(destination);
                        nextScore.GScore = newCost;
                        nextScore.Previous = currentNode;

                        uncheckedNodes.Enqueue(nextNode);
                    }

                }
            }

            return new List<T>();
        }

        public static bool PathExists(T source, T destination)
        {
            if (source == null || destination == null) return false;

            return FindPath(source, destination).Count > 0;
        }


        static int CompareNodes(T a, T b)
        {
            return a.GetStoredAStarScore().FScore.CompareTo(b.GetStoredAStarScore().FScore);
        }

    }
}