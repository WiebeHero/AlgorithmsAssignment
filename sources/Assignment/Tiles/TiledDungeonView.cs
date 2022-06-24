using GXPEngine;
using System;

/**
 * This is an example subclass of the TiledView that just generates random tiles.
 */
class TiledDungeonView : TiledView
{
	private Dungeon dungeon;
	public TiledDungeonView(Dungeon pDungeon, TileType pDefaultTileType) : base(pDungeon.size.Width, pDungeon.size.Height, (int)pDungeon.scale, pDefaultTileType)
	{
		this.dungeon = pDungeon;
        Console.WriteLine("{System}: Dungeon width: " + pDungeon.size.Width);
		Console.WriteLine("{System}: Dungeon height: " + pDungeon.size.Height);
	}

	/// <summary>
	/// Fills in tiles through a tilesheet according to the current tile.
	/// </summary>
	protected override void generate()
	{
		Console.ForegroundColor = ConsoleColor.Red;
		foreach (Room room in this.dungeon.rooms)
        {
            Console.WriteLine("{System}: Room X: " + room.area.X);
			Console.WriteLine("{System}: Total X: " + (room.area.X + room.area.Width));
			Console.WriteLine("{System}: Room Y: " + room.area.Y);
			Console.WriteLine("{System}: Total Y: " + (room.area.Y + room.area.Height));
			for (int column = room.area.X; column < room.area.X + room.area.Width; column++)
            {
				Console.WriteLine("{System}: Column: " + column);
				for (int row = room.area.Y; row < room.area.Y + room.area.Height; row++)
                {
                    Console.WriteLine("{System}: Row: " +  row);
					SetTileType(column, row, (column == room.area.X || column == room.area.X + room.area.Width - 1 || row == room.area.Y || row == room.area.Y + room.area.Height - 1) ? TileType.WALL : TileType.GROUND);
                }
            }
		}
        foreach (Door door in this.dungeon.doors)
        {
			SetTileType(door.location.X, door.location.Y, TileType.GROUND);
		}
	}
}

