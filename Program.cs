using System;

class Dungeon
{
    private static int seed;
    private static System.Random random = new Random();

    private static float splitVariance = 0f;    //used to determine variance in bsp splits
    private static float sizeVariance = 0f;     //used to determine variance in room size
    private static float doorVariance = 0f;     //used to determine variance in door location

    private static int minSize = 3;             //minimum size for rooms in any dimension

    private static int depth = 4;               //depth of recursion for bsp algorithm

    private static int width = 80;
    private static int height = 40;

    private static List<Room> rooms = new List<Room>();

    private static char currID = 'A';         //used to assign unique id to each room

    static void Main(string[] args)
    {
        Console.WriteLine("These are the arguments you've supplied:");
        
        foreach(string str in args)
        {
            Console.WriteLine(str);
        }

        //we start with a room that's the size of the entire area. it will be subdivided
        Room startRoom = new Room(0, width - 1, 0, height - 1);

        BinarySpacePartition(startRoom, depth);
    }

    static void BinarySpacePartition(Room room, int levelsToGo)
    {
        //check base cases first
        //stop when we've reached the desired depth, or if the current room reached the min size
        if(levelsToGo == depth || room.rightWall - room.leftWall < minSize || room.topWall - room.bottomWall < minSize)
        {
            room.id = currID++;
            rooms.Add(room);
            return;
        }
    }
}

class Room
{
    public int leftWall { get; }
    public int rightWall { get; }
    public int topWall { get; }
    public int bottomWall { get; }

    public char id;

    private List<Door> doors;

    public Room(int leftWall, int rightWall, int topWall, int bottomWall)
    {
        this.leftWall = leftWall;
        this.rightWall = rightWall;
        this.topWall = topWall;
        this.bottomWall = bottomWall;

        doors = new List<Door>();
    }
}

class Door
{
    private int x;
    private int y;
    private int divDim;

    private bool horizontal;

    private Door? other; //a door always must connect to another door

    public Door(int x, int y, bool horizontal)
    {
        this.x = x;
        this.y = y;
        this.horizontal = horizontal;

        if(horizontal)
        {
            divDim = x;
        }
        else
        {
            divDim = y;
        }
    }

    public void AssignDoor(Door other)
    {
        this.other = other;
    }
}