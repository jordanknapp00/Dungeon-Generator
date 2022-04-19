using System;

class Dungeon
{
    private static int seed;
    private static System.Random random = new Random();

    private static float splitVariance = 0f;
    private static float sizeVariance = 0f;
    private static float doorVariance = 0f;

    private static int depth = 4;

    private static int width = 80;
    private static int height = 40;

    private static List<Room> rooms = new List<Room>();

    static void Main(string[] args)
    {
        Console.WriteLine("These are the arguments you've supplied:");
        
        foreach(string str in args)
        {
            Console.WriteLine(str);
        }
    }
}

class Room
{
    private int leftWall;
    private int rightWall;
    private int topWall;
    private int bottomWall;

    private char id;

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

    public void assignDoor(Door other)
    {
        this.other = other;
    }
}