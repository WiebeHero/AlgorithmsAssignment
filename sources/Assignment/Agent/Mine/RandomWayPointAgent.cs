using GXPEngine;
using System.Collections.Generic;

/**
 * Very simple example of a nodegraphagent that walks directly to the node you clicked on,
 * ignoring walls, connections etc.
 */
class RandomWayPointAgent : NodeGraphAgent
{
	//Current target to move towards
	private NodeGraph graph;
	private Node _target = null, currentNode, newTarget, lastNode;

	public RandomWayPointAgent(NodeGraph pNodeGraph) : base(pNodeGraph)
	{

		SetOrigin(width / 2, height / 2);
		this.graph = pNodeGraph;
		//position ourselves on a random node
		if (pNodeGraph.nodes.Count > 0)
		{
			Node node = this.graph.nodes[Utils.Random(0, pNodeGraph.nodes.Count)];
			jumpToNode(node);
			this.currentNode = node;
		}

		//listen to nodeclicks
		this.graph.OnNodeLeftClicked += onNodeClickHandler;
	}

	protected override bool moveTowardsNode(Node pTarget)
	{
		float speed = Input.GetKey(SPEED_UP_KEY) ? FAST_TRAVEL_SPEED : REGULAR_SPEED;
        //increase our current frame based on time passed and current speed
        if (newTarget == null)
        {
			if (currentNode.connections.Contains(pTarget))
			{
				newTarget = pTarget;
			}
            else
            {
				Node randomNode = currentNode.connections[Utils.Random(0, currentNode.connections.Count)];
				if (currentNode.connections.Count == 1 || !randomNode.Equals(this.lastNode)) 
				{
					newTarget = randomNode;
				}
                else
                {
					newTarget = null;
				}
			}
            
        }
		SetFrame((int)(speed * (Time.time / 100)) % frameCount);
		//standard vector math as you had during the Physics course
		if (newTarget != null)
        {
			Vec2 targetPosition = new Vec2(newTarget.location.X, newTarget.location.Y);
			Vec2 currentPosition = new Vec2(x, y);
			Vec2 delta = targetPosition.Sub(currentPosition);
			if (delta.Length() < speed)
			{
				jumpToNode(newTarget);
				this.lastNode = this.currentNode;
				this.currentNode = newTarget;
				this.newTarget = null;
				if (this.currentNode.Equals(pTarget))
				{
					return true;
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
		return false;
	}

	protected virtual void onNodeClickHandler(Node pNode)
	{
        if (_target == null)
        {
			_target = pNode;
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
}
