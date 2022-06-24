using GXPEngine;
using System;
using System.Collections.Generic;

class BreadthFirstPathFinder : PathFinder
{
    private List<Node> toDoList = new List<Node>();
    private List<string> been = new List<string>();
	private List<Node> queue = new List<Node>();
	private Node from, to;

	public BreadthFirstPathFinder(NodeGraph pGraph) : base(pGraph) { }

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
        this.toDoList.Add(from);
        been.Add(from.id);
        while (toDoList.Count != 0)
        {
            Node focussedNode = toDoList[0];
            foreach (Node connection in focussedNode.connections)
            {
                Console.WriteLine(been.Count);
                Console.WriteLine("{System}: Node ID: " + connection.id);
                if (connection.id.Equals(to.id))
                {
                    Console.WriteLine("{System} To node!");
                    connection.parent = focussedNode;
                    this.BuildQueue(connection);
                    this.toDoList.Clear();
                    this.been.Clear();
                    break;
                }
                else if (connection.connections.Count > 1)
                {
                    if (!been.Contains(connection.id))
                    {
                        connection.parent = focussedNode;
                        toDoList.Add(connection);
                        been.Add(connection.id);
                    }
                }
                else
                {
                    connection.parent = focussedNode;
                }
            }
            toDoList.Remove(focussedNode);
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

}

