using Saxion.CMGT.Algorithms.sources.Assignment.NodeGraph.Mine;
using System.Diagnostics;
using System.Drawing;

/**
 * An example of a dungeon nodegraph implementation.
 * 
 * This implementation places only three nodes and only works with the SampleDungeon.
 * Your implementation has to do better :).
 * 
 * It is recommended to subclass this class instead of NodeGraph so that you already 
 * have access to the helper methods such as getRoomCenter etc.
 * 
 * TODO:
 * - Create a subclass of this class, and override the generate method, see the generate method below for an example.
 */
class HighLevelDungeonNodeGraph : NodeGraph
{
	protected Dungeon _dungeon;

	public HighLevelDungeonNodeGraph(Dungeon pDungeon) : base((int)(pDungeon.size.Width * pDungeon.scale), (int)(pDungeon.size.Height * pDungeon.scale), (int)pDungeon.scale / 3)
	{
		Debug.Assert(pDungeon != null, "Please pass in a dungeon.");

		_dungeon = pDungeon;
	}

	/// <summary>
	/// Generates a dungeon node graph, based on the doors in the Dungeon.
	/// The doors can only have 2 rooms. 
	/// </summary>
	protected override void generate()
	{
		for (int i = 0; i < _dungeon.doors.Count; i++)
        {
			Door door = _dungeon.doors[i];
			//Create the node for the door.
			Node nodeDoor = new Node(getDoorCenter(door), NodeTypes.DOOR);
			nodes.Add(nodeDoor);
			System.Console.WriteLine("{System}: Door node added!: " + nodeDoor);
			//Create 2 nodes for the rooms.
			Node nodeRoomA = new Node(getRoomCenter(door.roomA), NodeTypes.ROOM);
			Node nodeRoomB = new Node(getRoomCenter(door.roomB), NodeTypes.ROOM);
			bool nodeRoomAFound = false;
			bool nodeRoomBFound = false;
			//Check if the node for a specific room already exists in the list.
			//If so, grab it, and ONLY make a connection.
            foreach (Node node in nodes)
            {
                if (node.location.X == getRoomCenter(door.roomA).X && node.location.Y == getRoomCenter(door.roomA).Y)
                {
					nodeRoomAFound = true;
					nodeRoomA = node;
                }
				if (node.location.X == getRoomCenter(door.roomB).X && node.location.Y == getRoomCenter(door.roomB).Y)
				{
					nodeRoomBFound = true;
					nodeRoomB = node;
				}
			}

            if (!nodeRoomAFound)
			{
				nodes.Add(nodeRoomA);
				System.Console.WriteLine("{System}: Room A node added!: " + nodeRoomA);
			}
			if (!nodeRoomBFound)
			{
				nodes.Add(nodeRoomB);
				System.Console.WriteLine("{System}: Room B node added!: " + nodeRoomB);
			}
			AddConnection(nodeDoor, nodeRoomA);
			AddConnection(nodeRoomA, nodeDoor);
			System.Console.WriteLine("{System}: Door node connected to Room A node!");
			AddConnection(nodeDoor, nodeRoomB);
			AddConnection(nodeRoomB, nodeDoor);
			System.Console.WriteLine("{System}: Door node connected to Room B node!");
            
		}
        System.Console.WriteLine("{System}: -------------------------------------");
        /*foreach (Door door in _dungeon.doors)
        {
            foreach (Door nextDoor in _dungeon.doors)
            {
                if (door.roomA.Equals(nextDoor.roomA) && door != nextDoor)
                {
                    System.Console.WriteLine("{System}: Room A Match found!");
                }
				if (door.roomB.Equals(nextDoor.roomB) && door != nextDoor)
				{
					System.Console.WriteLine("{System}: Room B Match found!");
				}
			}
        }*/
		System.Console.WriteLine("{System}: -------------------------------------");
	}

	/**
	 * A helper method for your convenience so you don't have to meddle with coordinate transformations.
	 * @return the location of the center of the given room you can use for your nodes in this class
	 */
	protected Point getRoomCenter(Room pRoom)
	{
		float centerX = ((pRoom.area.Left + pRoom.area.Right) / 2.0f) * _dungeon.scale;
		float centerY = ((pRoom.area.Top + pRoom.area.Bottom) / 2.0f) * _dungeon.scale;
		return new Point((int)centerX, (int)centerY);
	}

	/**
	 * A helper method for your convenience so you don't have to meddle with coordinate transformations.
	 * @return the location of the center of the given door you can use for your nodes in this class
	 */
	protected Point getDoorCenter(Door pDoor)
	{
		return getPointCenter(pDoor.location);
	}

	/**
	 * A helper method for your convenience so you don't have to meddle with coordinate transformations.
	 * @return the location of the center of the given point you can use for your nodes in this class
	 */
	protected Point getPointCenter(Point pLocation)
	{
		float centerX = (pLocation.X + 0.5f) * _dungeon.scale;
		float centerY = (pLocation.Y + 0.5f) * _dungeon.scale;
		return new Point((int)centerX, (int)centerY);
	}

}
