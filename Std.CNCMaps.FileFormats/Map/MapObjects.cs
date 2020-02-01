using System;
using System.Collections.Generic;

namespace Std.CNCMaps.FileFormats.Map {

	public class MapObject {
		public IsoTile Tile;
	}
	public class NamedMapObject : MapObject {
		public string Name { get; set; }
	}
	public class NumberedMapObject : MapObject {
		public virtual int Number { get; set; }
	}


	// all the stuff found on maps
	public class IsoTile : NumberedMapObject {
		public ushort Dx;
		public ushort Dy;
		public ushort Rx;
		public ushort Ry;
		public byte Z;
		public short TileNum;
		public byte SubTile;

		public IsoTile(ushort p1, ushort p2, ushort rx, ushort ry, byte z, short tilenum, byte subtile) {
			Dx = p1;
			Dy = p2;
			Rx = rx;
			Ry = ry;
			Z = z;
			TileNum = tilenum;
			SubTile = subtile;
		}

		public List<byte> ToMapPack5Entry() {
			var ret = new List<byte>();
			ret.AddRange(BitConverter.GetBytes(Rx));
			ret.AddRange(BitConverter.GetBytes(Ry));
			ret.AddRange(BitConverter.GetBytes(TileNum));
			ret.Add(0); ret.Add(0);
			ret.Add(SubTile);
			ret.Add(Z);
			ret.Add(0);
			return ret;
		}
	}

	public class Aircraft : NamedMapObject {
		public Aircraft(string owner, string name, short health, short direction, bool onBridge) {
			Owner = owner;
			Name = name;
			Health = health;
			Direction = direction;
			OnBridge = onBridge;
		}
		public short Health { get; set; }
		public short Direction { get; set; }
		public bool OnBridge { get; set; }
		public string Owner { get; set; }
	}

	public class Infantry : NamedMapObject {
		public Infantry(string owner, string name, short health, short direction, bool onBridge) {
			Owner = owner;
			Name = name;
			Health = health;
			Direction = direction;
			OnBridge = onBridge;
		}
		public short Health { get; set; }
		public short Direction { get; set; }
		public bool OnBridge { get; set; }
		public string Owner { get; set; }
	}

	public class Overlay : NumberedMapObject {
		public byte OverlayID { get; set; }
		public byte OverlayValue { get; set; }
		public Overlay(byte overlayID, byte overlayValue) {
			OverlayID = overlayID;
			OverlayValue = overlayValue;
		}
		public override int Number {
			get { return OverlayID; }
			set { OverlayID = (byte)value; }
		}
	}
	public class Smudge : NamedMapObject {
		public Smudge(string name) {
			Name = name;
		}
	}
	public class Structure : NamedMapObject {
		public Structure(string owner, string name, short health, short direction) {
			Owner = owner;
			Name = name;
			Health = health;
			Direction = direction;
		}

		public short Health { get; set; }
		public short Direction { get; set; }
		public bool OnBridge { get; set; }
		public string Owner { get; set; }

		public string Upgrade1 { get; set; }
		public string Upgrade2 { get; set; }
		public string Upgrade3 { get; set; }
	}
	public class Terrain : NamedMapObject {
		public Terrain(string name) {
			Name = name;
		}
	}
	public class Unit : NamedMapObject {
		public Unit(string owner, string name, short health, short direction, bool onBridge) {
			Owner = owner;
			Name = name;
			Health = health;
			Direction = direction;
			OnBridge = onBridge;
		}
		public short Health { get; set; }
		public short Direction { get; set; }
		public bool OnBridge { get; set; }
		public string Owner { get; set; }
	}

	public class Waypoint : NumberedMapObject { }
}