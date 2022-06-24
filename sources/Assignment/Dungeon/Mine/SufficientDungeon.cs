using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Mine.SplitTypes;

class SufficientDungeon : Dungeon
{
	private int loopCount = 0;
	private int splitSize = 1;
	private bool reverse = false;

	public SufficientDungeon(Size pSize) : base(pSize) { }

	/// <summary>
	/// Creates a dungeon.
	/// </summary>
	/// <param name="pMinimumRoomSize"></param>
	protected override void generate(int pMinimumRoomSize)
	{
		this.RoomCreate(0, 0, size.Width, size.Height, pMinimumRoomSize);
		this.CreateDoors();
		this.CheckDoors();
		Console.ForegroundColor = ConsoleColor.White;
	}
	
	/// <summary>
	/// Creates a room.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="minimum"></param>
	/// <returns></returns>
	public Room RoomCreate(int x, int y, int width, int height, int minimum)
	{
		Room room = new Room(new Rectangle(x, y, width, height));
		this.rooms.Add(room);
		Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("{System}: Room Created: " + room.ToString());
		int newWidth = width;
		int newHeight = height;
		//Check if the width and height is more then the minimum.
		if (width >= minimum && height >= minimum)
		{
			//If width is higher then height, then split at vertical.
			if (width >= height)
			{
				//See if width can be further devided.
                if (width / 2 >= minimum)
                {
					//Calculate new width.
					newWidth = Utils.Random(minimum, width - minimum);
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("{System}: New Width Calculations: " + newWidth + " New Width Calculations: " + (width - newWidth));

					//Check if the new width is valid.
					if (newWidth < minimum || width - newWidth < minimum)
						return null;

					//Creation of room 1.
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("{System}: First iteration width split.");
					this.RoomCreate(x, y, newWidth, newHeight, minimum);

					//Creation of room 2.
					Console.WriteLine("{System}: Second iteration width split.");
					this.RoomCreate(x + newWidth - 1, y, width - newWidth + 1, height, minimum);

					//Creation of door. (Optional Challenge!)
					/*int randomDoor = Utils.Random(1, newHeight - 1);
					Door newDoor = new Door(new Point(x + newWidth - 1, y + randomDoor), false);
					this.doors.Add(newDoor);
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.WriteLine("{System}: Door Created: " + newDoor);*/

					//Remove old room, because we can split.
					this.rooms.Remove(room);
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("{System}: Room Removed: " + room);
				}
			}
			//If height is higher then width, then split at horizontal.
			else if (height >= width)
			{
				//See if width can be further devided.
				if (height / 2 >= minimum)
                {
					//Calculate new height.
					newHeight = Utils.Random(minimum, height - minimum);
					Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("{System}: New Height Calculations: " + newHeight + " New Room Height: " + (height - newHeight));

					//Check if the new width is valid.
					if (newHeight < minimum || height - newHeight < minimum)
						return null;

					//Creation of room 1
                    Console.WriteLine("{System}: First iteration height split.");
					this.RoomCreate(x, y, newWidth, newHeight, minimum);

					//Creation of room 2
					Console.WriteLine("{System}: Second iteration height split.");
					this.RoomCreate(x, y + newHeight - 1, width, height - newHeight + 1, minimum);

					//Creation of door (Optional Challenge!)
					/*int randomDoor = Utils.Random(1, newWidth - 1);
					Door newDoor = new Door(new Point(x + randomDoor, y + newHeight - 1), true);
					this.doors.Add(newDoor);
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.WriteLine("{System}: Door Created: " + newDoor);*/

					//Remove old room, because we split.
					this.rooms.Remove(room);
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("{System}: Room Removed: " + room);
				}
			}
		}
		return room;
	}

	/// <summary>
	/// A method that creates 2 doors per room.
	/// </summary>
	public void CreateDoors()
    {
        foreach (Room room in this.rooms)
        {
			int x = room.area.X;
			int y = room.area.Y;
			int width = room.area.Width;
			int height = room.area.Height;
			int randomDoor = Utils.Random(1, width - 1);
			Door newDoor = new Door(new Point(x + randomDoor, y + height - 1), true);
            Console.WriteLine((y + height) + " " + this.size.Height);
			Console.WriteLine((y + height) != this.size.Height);
			if (y + height != this.size.Height)
            {
				this.doors.Add(newDoor);
			}
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("{System}: Door Created: " + newDoor);
			int randomDoor2 = Utils.Random(1, height - 1);
			Door newDoor2 = new Door(new Point(x + width - 1, y + randomDoor2), false);
			if (x + width != this.size.Width)
			{
				this.doors.Add(newDoor2);
			}
			Console.WriteLine("{System}: Door Created: " + newDoor2);
		}
    }	

	/// <summary>
	/// A method to check if a door is in a corner of 2 rooms. If so, move it.
	/// Also, this method assigns the doors to their specific rooms.
	/// </summary>
	public void CheckDoors()
	{
		Door door = this.doors[loopCount];
		Point loc = door.location;
		//A door cannot have 1 room. It can also not have a room that doesn't exist.
		if (!this.rooms.Contains(door.roomA))
		{
			door.roomA = null;
		}
		if (!this.rooms.Contains(door.roomB))
		{
			door.roomB = null;
		}
		//Loop through every room.
		for (int j = 0; j < this.rooms.Count; j++)
		{
			Room room = this.rooms[j];
			//Split the room.
			SplitType split = this.DoorIntersectRoom(door, room);
			if (split == SplitType.CORNER)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine("{System}: Door is Moving: " + door);
				this.doors.RemoveAt(loopCount);
				Door newDoor = new Door(new Point(door.horizontal ? loc.X + splitSize : loc.X, door.horizontal ? loc.Y : loc.Y + splitSize));
				this.reverse = true;
				this.doors.Insert(loopCount, newDoor);
				this.CheckDoor(newDoor);
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("{System}: Door Moved: " + newDoor);
			}
			else if (split == SplitType.WALL)
			{
				this.reverse = false;
				this.splitSize = 1;
				if (door.roomA == null)
				{
					door.roomA = room;
				}
				else if (door.roomB == null)
				{
					door.roomB = room;
				}
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("{System}: Door Assigned to Room: " + door);
			}
		}
		loopCount++;
		if (loopCount < this.doors.Count)
		{
			this.CheckDoors();
		}
	}

	/// <summary>
	/// A method to check if a door is in a corner of 2 rooms. If so, move it.
	/// Also, this method assigns the doors to their specific rooms.
	/// </summary>
	private void CheckDoor(Door door)
	{
		Point loc = door.location;
		for (int j = 0; j < this.rooms.Count; j++)
		{
			Room room = this.rooms[j];
			SplitType split = this.DoorIntersectRoom(door, room);
			if (split == SplitType.CORNER)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.WriteLine("{System}: Door is Moving again: " + door);
				this.doors.RemoveAt(loopCount);
                if (reverse)
                {
					this.splitSize -= splitSize + 1;
					this.reverse = false;
                }
                else
                {
					this.splitSize = Math.Abs(splitSize) + 1;
					this.reverse = true;
                }
				Door newDoor = new Door(new Point(door.horizontal ? loc.X + splitSize : loc.X, door.horizontal ? loc.Y : loc.Y + splitSize));
				this.doors.Insert(loopCount, newDoor);
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("{System}: Door Moved: " + newDoor);
				this.CheckDoor(newDoor);
			}
			else if (split == SplitType.WALL)
			{
				this.reverse = false;
				this.splitSize = 1;
				if (door.roomA == null)
				{
					door.roomA = room;
				}
				else if (door.roomB == null)
				{
					door.roomB = room;
				}
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("{System}: Door Assigned to Room: " + door);
			}
		}
	}

	/// <summary>
	/// Checks if a door intersects with a corner of a room.
	/// </summary>
	/// <param name="door"></param>
	/// <param name="room"></param>
	/// <returns></returns>
	private SplitType DoorIntersectRoom(Door door, Room room)
	{
		if (door != null && room != null)
		{
			Point location = door.location;
			Rectangle rect = room.area;
			if ((location.X == rect.Left && location.Y == rect.Top) || (location.X == rect.Right - 1 && location.Y == rect.Top) || (location.X == rect.Left && location.Y == rect.Bottom - 1) || (location.X == rect.Right - 1 && location.Y == rect.Bottom - 1))
			{
				return SplitType.CORNER;
			}
			else if ((location.X >= rect.Left && location.Y >= rect.Top) && (location.X <= rect.Right - 1 && location.Y >= rect.Top) && (location.X >= rect.Left && location.Y <= rect.Bottom - 1) && (location.X <= rect.Right - 1 && location.Y <= rect.Bottom - 1))
			{
				return SplitType.WALL;
			}
		}
		return SplitType.NONE;
	}

	public override string ToString()
    {
		return "(X: " + this.x + " Y: " + this.y + " Width: " + this.width + " Height: " + this.height + ")";
    }
}


