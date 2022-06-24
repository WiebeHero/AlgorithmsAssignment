using GXPEngine;
using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph.Mine;
using System;
using System.Collections.Generic;
using System.Linq;

class BreadthFirstPathFinderDijkstra : PathFinder
{
    private Dictionary<Node, double> toDoList = new Dictionary<Node, double>();
    private List<string> been = new List<string>(), exclude = new List<string>();
    private List<Node> queue = new List<Node>();
    private Node from, to;
    private EasyDraw indicator;

    public BreadthFirstPathFinderDijkstra(NodeGraph pGraph) : base(pGraph) {
        this._nodeGraph.OnNodeRightClicked += NodeClick;
        this.indicator = new EasyDraw(100, 100);
        this.AddChild(this.indicator);
    }

    protected override List<Node> generate(Node pFrom, Node pTo)
    {
        this.from = pFrom;
        this.to = pTo;
        this.generatePath();
        Console.WriteLine("Queue count:" + queue.Count);
        return queue;
    }

    private void generatePath()
    {
        this.ParentClear(); 
        this.toDoList.Add(from, 0);
        been.Add(from.id);
        while (toDoList.Count != 0)
        {
            var enumarator = toDoList.Keys.GetEnumerator();
            enumarator.MoveNext();
            Node focussedNode = enumarator.Current;
            foreach (Node connection in focussedNode.connections)
            {
                Console.WriteLine(been.Count);
                Console.WriteLine("{System}: Node ID: " + connection.id);
                if (connection.connections.Count > 1)
                {
                    if (!been.Contains(connection.id) && !exclude.Contains(connection.id))
                    {
                        been.Add(connection.id);
                        connection.parent = focussedNode;
                        //Cost calculation
                        Vec2 current = new Vec2(focussedNode.location.X, focussedNode.location.Y);
                        Vec2 target = new Vec2(connection.location.X, connection.location.Y);
                        double distance = current.DistanceTo(target);
                        double oldCost = 0;
                        if (this.toDoList.ContainsKey(focussedNode))
                        {
                            this.toDoList.TryGetValue(focussedNode, out oldCost);
                        }
                        //Calculate new cost
                        double newCost = oldCost + distance;
                        Console.WriteLine("New Cost: " + newCost);
                        Console.WriteLine("{System}: " + been.Contains(connection.id));
                        toDoList.Add(connection, newCost);
                    }
                }
                else
                {
                    connection.parent = focussedNode;
                    toDoList.Add(connection, this.toDoList[focussedNode]);
                }
                if (connection.id.Equals(to.id))
                {
                    Console.WriteLine("{System} To node!");
                    connection.parent = focussedNode;
                    Console.WriteLine("{System}: Path found with cost: " + this.toDoList[to]);
                    this.BuildQueue(connection);
                    this.toDoList.Clear();
                    this.been.Clear();
                    break;
                }
            }
            foreach (double cost in this.toDoList.Values)
            {
                Console.WriteLine("{System}: Current cost: " + cost);
            }
            toDoList.Remove(focussedNode);
            toDoList = toDoList.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach (double pair in toDoList.Values)
            {
                Console.WriteLine("{System}: Ordered cost: " + pair);
            }
        }
    }
    /// <summary>
    /// Starts building the queue based on the end node.
    /// </summary>
    /// <param name="node"></param>
    private void BuildQueue(Node node)
    {
        this.queue = new List<Node>();
        this.ParentLoop(node);
        Console.WriteLine("{System}: Complete!");
    }
    /// <summary>
    /// Loops through the final nodes parent to get to the top.
    /// </summary>
    /// <param name="node"></param>
    private void ParentLoop(Node node)
    {
        Console.WriteLine("{System}: In parent loop!");
        this.queue.Add(node);
        if (node.parent != null)
        {
            this.ParentLoop(node.parent);
        }
    }
    /// <summary>
    /// Clear the parents of all nodes (Only do this when done with a node)
    /// </summary>
    private void ParentClear()
    {
        foreach (Node node in this._nodeGraph.nodes)
        {
            node.parent = null;
        }
    }

    public void SetFrom(Node from)
    {
        this.from = from;
    }


    public void NodeClick(Node pNode)
    {
        Console.WriteLine("{System}: Node is doing the locking procedure!");
        if (pNode.nodeType == NodeTypes.DOOR)
        {
            List<Node> rooms = new List<Node>();
            List<Node> toDo = new List<Node>();
            List<Node> done = new List<Node>();
            toDo.Add(from);
            if (this.exclude.Contains(pNode.id))
            {
                Console.WriteLine("{System}: Door is locked!");
                this.exclude.Remove(pNode.id);
            }
            else
            {
                Console.WriteLine("{System}: Door is unlocked!");
                this.exclude.Add(pNode.id);
            }
            done.Add(from);
            if (from.nodeType == NodeTypes.ROOM)
            {
                rooms.Add(from);
            }
            while (toDo.Count != 0)
            {
                Node node = toDo[0];
                foreach (Node connection in node.connections)
                {
                    if (node.connections.Count > 1)
                    {
                        if (!this.exclude.Contains(node.id) && !done.Contains(connection))
                        {
                            if (connection.nodeType == NodeTypes.ROOM)
                            {
                                rooms.Add(connection);
                            }
                            toDo.Add(connection);
                            done.Add(connection);
                        }
                    }
                    else
                    {
                        if (!this.exclude.Contains(node.id) && !done.Contains(connection))
                        {
                            if (connection.nodeType == NodeTypes.ROOM)
                            {
                                rooms.Add(connection);
                            }
                            toDo.Add(connection);
                            done.Add(connection);
                        }
                    }
                }
                toDo.RemoveAt(0);
            }
            Console.WriteLine("{System}: Done Count: " + rooms.Count);
            
            int roomCountTotal = 0;
            foreach (Node node in _nodeGraph.nodes)
            {
                if (node.nodeType == NodeTypes.ROOM)
                {
                    roomCountTotal++;
                }
            }
            Console.WriteLine("{System}: Node Graph: " + roomCountTotal);
            if (rooms.Count == roomCountTotal)
            {
                this.indicator.Clear(System.Drawing.Color.Green);
            }
            else
            {
                this.indicator.Clear(System.Drawing.Color.Red);
            }
        }
        
    }

}

