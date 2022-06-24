using GXPEngine;
using System;
using System.Collections.Generic;

class DescendingCostQueue : IComparer<List<Node>>
{
    public int Compare(List<Node> nodes1, List<Node> nodes2)
    {
        if (nodes1.Count == nodes2.Count)
        {
            return 0;
        }
        return nodes1.Count.CompareTo(nodes2.Count);

    }
}

class RecursivePathFinder : PathFinder
{
    private List<List<Node>> multipleQueues = new List<List<Node>>();
    //private List<Node> toDoList = new List<Node>();
    //private List<NodeCollection> nodeCollections = new List<NodeCollection>();
    private Node from, to;

    public RecursivePathFinder(NodeGraph pGraph) : base(pGraph) { }

    protected override List<Node> generate(Node pFrom, Node pTo)
    {
        this.from = pFrom;
        this.to = pTo;
        this.generateQueue();
        this.ParentClear();
        Console.WriteLine("{System}: Queue count:" + multipleQueues.Count);
        /*foreach (List<Node> nodeList in multipleQueues)
        {
            Console.WriteLine("Queue node count: " + nodeList.Count);
            foreach (Node node in nodeList)
            {
                Console.WriteLine("{System}: Node ID: " + node.id);
            }
        }*/
        this.multipleQueues.Sort(new DescendingCostQueue());
        return multipleQueues[0];
    }

    private void generateQueue()
    {
        this.multipleQueues.Clear();
        List<string> been = new List<string>();
        been.Add(from.id);
        Console.WriteLine("{System}: End node is: " + this.to.id);
        this.generatePath(from, been);
        /*Console.WriteLine("{System}: Connection count: " + from.connections.Count);
        this.nodeCollections.Add(new NodeCollection(this, this.from, null, true, new List<string>()));*/
    }

    private void generatePath(Node node, List<string> been)
    {
        foreach (Node connection in node.connections)
        {
            Console.WriteLine(been.Count);
            Console.WriteLine("{System}: Node ID: " + connection.id);
            if (connection.id.Equals(to.id))
            {
                Console.WriteLine("{System} To node!");
                connection.parent = node;
                this.BuildQueue(connection);
            }
            else if (connection.connections.Count > 1)
            {
                if (!been.Contains(connection.id))
                {
                    connection.parent = node;
                    been.Add(connection.id);
                    this.generatePath(connection, new List<string>(been));
                }
            }
            else
            {
                connection.parent = node;
            }
        }
    }

    private void BuildQueue(Node node)
    {
        List<Node> queue = new List<Node>();
        this.multipleQueues.Add(queue);
        this.ParentLoop(node);
        Console.WriteLine("{System}: Complete!");
    }

    private void ParentLoop(Node node)
    {
        Console.WriteLine("{System}: In parent loop!");
        this.multipleQueues[this.multipleQueues.Count - 1].Add(node);
        if (node.parent != null)
        {
            this.ParentLoop(node.parent);
        }
    }

    private void ParentClear()
    {
        foreach (Node node in this._nodeGraph.nodes)
        {
            node.parent = null;
        }
    }

    public Node StartNode
    {
        get {
            return this.from;
        }
    }

    public Node EndNode
    {
        get
        {
            return this.to;
        }
    }

    public List<List<Node>> GetQueueManager()
    {
        return this.multipleQueues;
    }

    public void AddToQueue(Node node)
    {
        this.multipleQueues[this.multipleQueues.Count - 1].Add(node);
    }
}

