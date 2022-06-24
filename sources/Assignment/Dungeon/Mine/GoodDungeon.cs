using GXPEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using static Saxion.CMGT.Algorithms.sources.Assignment.Dungeon.Mine.SplitTypes;

class GoodDungeon : SampleDungeon
{
	private int loopCount = 0;
	private Random random;
	private int splitSize = 1;
	private bool reverse = false;

	public GoodDungeon(Size pSize) : base(pSize) { }

	/// <summary>
	/// Creates a dungeon.
	/// </summary>
	/// <param name="pMinimumRoomSize"></param>
	protected override void generate(int pMinimumRoomSize)
	{
		this.random = new Random(Utils.Random(1, 10));
		this.RoomCreate(0, 0, size.Width, size.Height, pMinimumRoomSize);

		this.CreateDoors();
		this.CheckDoors();
		this.RoomVolumes();
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
					newWidth = this.random.Next(minimum, width - minimum);
					Console.WriteLine("{System}: New Width Calculations: " + newWidth + " New Width Calculations: " + (width - newWidth));

					//Check if the new width is valid.
					if (newWidth < minimum || width - newWidth < minimum)
						return null;

					//Creation of room 1.
					Console.WriteLine("{System}: First iteration width split.");
					this.RoomCreate(x, y, newWidth, newHeight, minimum);

					//Creation of room 2.
					Console.WriteLine("{System}: Second iteration width split.");
					this.RoomCreate(x + newWidth - 1, y, width - newWidth + 1, height, minimum);

					//Creation of door. (Optional Challenge!)
					/*int randomDoor = Utils.Random(1, newHeight - 1);
					Door newDoor = new Door(new Point(x + newWidth - 1, y + randomDoor), false);
					this.doors.Add(newDoor);
					Console.WriteLine("{System}: Door Created: " + newDoor);*/

					//Remove old room, because we can split.
					this.rooms.Remove(room);
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
					newHeight = this.random.Next(minimum, height - minimum);
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
					Console.WriteLine("{System}: Door Created: " + newDoor);*/

					//Remove old room, because we split.
					this.rooms.Remove(room);
					Console.WriteLine("{System} Room Removed: " + room);
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
				Console.WriteLine("{System}: Door is Moving: " + door);
				this.doors.RemoveAt(loopCount);
				Door newDoor = new Door(new Point(door.horizontal ? loc.X + this.splitSize : loc.X, door.horizontal ? loc.Y : loc.Y + this.splitSize));
				this.reverse = true;
				this.doors.Insert(loopCount, newDoor);
				this.CheckDoor(newDoor);
				Console.WriteLine("{System}: Door Moved: " + newDoor);
			}
			else if (split == SplitType.WALL)
            {
				this.splitSize = 1;
				this.reverse = false;
                if (door.roomA == null)
                {
					door.roomA = room;
                }
                else if (door.roomB == null)
                {
					door.roomB = room;
                }
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
				Console.WriteLine("{System}: Door is Moving: " + door);
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
				Door newDoor = new Door(new Point(door.horizontal ? loc.X + 1 : loc.X, door.horizontal ? loc.Y : loc.Y + 1));
				this.doors.Insert(loopCount, newDoor);
				Console.WriteLine("{System}: Door Moved: " + newDoor);
				this.CheckDoor(newDoor);
			}
			else if (split == SplitType.WALL)
			{
				if (door.roomA == null)
				{
					door.roomA = room;
				}
				else if (door.roomB == null)
				{
					door.roomB = room;
				}
				Console.WriteLine("{System}: Door Assigned to Room: " + door);
			}
		}
	}

	/// <summary>
	/// Draws the rooms with colors, depending on how many doors a room has.
	/// </summary>
	/// <param name="pRooms"></param>
	/// <param name="pWallColor"></param>
	/// <param name="pFillColor"></param>
	protected override void drawRooms(IEnumerable<Room> pRooms, Pen pWallColor, Brush pFillColor = null)
	{
		foreach (Room room in pRooms)
		{
			int doorCount = 0;
			foreach (Door door in doors)
			{
				Rectangle rect = new Rectangle(new Point(door.location.X, door.location.Y), new Size(1, 1));
				if (room.area.IntersectsWith(rect))
				{
					doorCount++;
				}
			}
			Color color = Color.White;
			switch (doorCount)
			{
				case 0:
					color = Color.Red;
					break;
				case 1:
					color = Color.Orange;
					break;
				case 2:
					color = Color.Yellow;
					break;
			}
			if (doorCount >= 3)
			{
				color = Color.Green;
			}
			pFillColor = new SolidBrush(color);
			drawRoom(room, pWallColor, pFillColor);
		}
	}

	/// <summary>
	/// Deletes the room with the biggest volume and the lowest volume. (Surface Area)
	/// </summary>
	public void RoomVolumes()
	{
		//Basic values
		int highestRoomVolume = 0;
		int lowestRoomVolume = 20000;

		//Loop through every room.
		for (int i = 0; i < this.rooms.Count; i++)
		{
			Room room = this.rooms[i];
			Rectangle rect = room.area;

			//Calculate Surface Area of Room.
			int roomVolume = rect.Width * rect.Height;

			//Check if this surface area is the highest.
			if (roomVolume > highestRoomVolume)
			{
				highestRoomVolume = roomVolume;
			}

			//Check if this surface area is the lowest.
			if (roomVolume < lowestRoomVolume)
			{
				lowestRoomVolume = roomVolume;
			}
		}
		//Remove all rooms that have the same volume 
		this.rooms.RemoveAll(v => v.area.Width * v.area.Height == highestRoomVolume || v.area.Width * v.area.Height == lowestRoomVolume);
		this.doors.RemoveAll(v => v.roomA != null && (v.roomA.area.Width * v.roomA.area.Height == highestRoomVolume || v.roomA.area.Width * v.roomA.area.Height == lowestRoomVolume));
		this.doors.RemoveAll(v => v.roomB != null && (v.roomB.area.Width * v.roomB.area.Height == highestRoomVolume || v.roomB.area.Width * v.roomB.area.Height == lowestRoomVolume));
		this.doors.RemoveAll(v => v.roomA == null || v.roomB == null);
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
			if((location.X == rect.Left && location.Y == rect.Top) || (location.X == rect.Right - 1 && location.Y == rect.Top) || (location.X == rect.Left && location.Y == rect.Bottom - 1) || (location.X == rect.Right - 1 && location.Y == rect.Bottom - 1))
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


