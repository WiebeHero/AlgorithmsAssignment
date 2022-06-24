using GXPEngine;
using System.Collections.Generic;

/**
 * Very simple example of a nodegraphagent that walks directly to the node you clicked on,
 * ignoring walls, connections etc.
 */
class PathFindingAgent : NodeGraphAgent
{
	private NodeGraph graph;
	private Node _target = null, currentNode;
	private PathFinder finder;
	private List<Node> queue;

	public PathFindingAgent(NodeGraph pNodeGraph, PathFinder pathFinder) : base(pNodeGraph)
	{
		this.finder = pathFinder;
		SetOrigin(width / 2, height / 2);
		this.queue = new List<Node>();
		this.graph = pNodeGraph;
		//position ourselves on a random node
		if (pNodeGraph.nodes.Count > 0)
		{
			Node node = this.graph.nodes[Utils.Random(0, pNodeGraph.nodes.Count)];
			jumpToNode(node);
			this.currentNode = node;
		}
        if (this.finder is BreadthFirstPathFinderDijkstra)
        {
			BreadthFirstPathFinderDijkstra path = (BreadthFirstPathFinderDijkstra)this.finder;
			path.SetFrom(this.currentNode);
		}
		//listen to nodeclicks
		this.graph.OnNodeLeftClicked += onNodeClickHandler;
	}

	protected override bool moveTowardsNode(Node pTarget)
	{
		float speed = Input.GetKey(SPEED_UP_KEY) ? FAST_TRAVEL_SPEED : REGULAR_SPEED;
		//increase our current frame based on time passed and current speed
		SetFrame((int)(speed * (Time.time / 100)) % frameCount);
		Node node = this.queue.Count != 0 ? this.queue[0] : null;
		//standard vector math as you had during the Physics course
		if (node == null)
			return true;
		Vec2 targetPosition = new Vec2(node.location.X, node.location.Y);
		Vec2 currentPosition = new Vec2(x, y);
		Vec2 delta = targetPosition.Sub(currentPosition);
		if (delta.Length() < speed)
		{
			jumpToNode(node);
			this.currentNode = node;
			this.queue.Remove(node);
			if (this.queue.Count == 0)
			{
				this._target = null;
				if (this.finder is BreadthFirstPathFinderDijkstra)
				{
					BreadthFirstPathFinderDijkstra path = (BreadthFirstPathFinderDijkstra)this.finder;
					path.SetFrom(this.currentNode);
				}
			}
			return false;
		}
		else
		{
			Vec2 velocity = delta.Normalize().Scale(speed);
			x += velocity.x;
			y += velocity.y;

			scaleX = (velocity.x >= 0) ? 1 : -1;

			return false;
		}
	}

	protected virtual void onNodeClickHandler(Node pNode)
	{
		System.Console.WriteLine(this.queue.Count);
		if (this._target == null)
		{
			_target = pNode;
			this.queue = this.finder.Generate(currentNode, this._target);
			this.queue.Reverse();
		}
	}

	protected override void Update()
	{
		//no target? Don't walk
		if (_target == null) return;

		//Move towards the target node, if we reached it, clear the target
		if (moveTowardsNode(_target))
		{
			_target = null;
		}
	}

	public Node GetTargetNode()
	{
		return this._target;
	}

	public List<Node> GetQueue()
	{
		return this.queue;
	}
}
