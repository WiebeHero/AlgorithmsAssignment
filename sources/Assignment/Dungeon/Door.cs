using System.Drawing;

/**
 * This class represents (the data for) a Door, at this moment only a position in the dungeon.
 * Changes to this class might be required based on your specific implementation of the algorithm.
 */
class Door
{
	public readonly Point location;

	//Keeping tracks of the Rooms that this door connects to,
	//might make your life easier during some of the assignments
	public Room roomA = null;
	public Room roomB = null;

	//You can also keep track of additional information such as whether the door connects horizontally/vertically
	//Again, whether you need flags like this depends on how you implement the algorithm, maybe you need other flags
	public bool horizontal = false;

	public Door(Point pLocation)
	{
		location = pLocation;
	}

	public Door(Point pLocation, bool horizontal)
	{
		location = pLocation;
		this.horizontal = horizontal;
	}

	public Door(Point pLocation, bool horizontal, Room roomA, Room roomB)
	{
		location = pLocation;
		this.horizontal = horizontal;
		this.roomA = roomA;
		this.roomB = roomB;
	}

    public override string ToString()
    {
		return "(Location: " + location + " Horizontal: " + this.horizontal + " Room A: " + this.roomA + " Room B: " + this.roomB + ")";
    }

    //TODO: Implement a toString method for debugging
    //Return information about the type of object and it's data
    //eg Door: (x,y)
}

