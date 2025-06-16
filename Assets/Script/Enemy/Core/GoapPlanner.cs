using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

public class GoapPlanner
{
    private class Node
    {
        public Node parent;
        public float cost;
        public WorldState state;
        public GoapAction action;

        public Node(Node parent, float cost, WorldState state, GoapAction action)
        {
            this.parent = parent;
            this.cost = cost;
            this.state = state;
            this.action = action;
        }
    }

    public Queue<GoapAction> Plan(GameObject agent, List<GoapAction> actions, WorldState worldState, GoapGoal goal)
    {
        // 实现保持不变...
    }
}