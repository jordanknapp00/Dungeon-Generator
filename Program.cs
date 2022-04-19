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

        Draw();
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

        levelsToGo--;

        //determine the direction upon which we are going to split. whichever wall is longer is
        //the one being split
        if(room.rightWall - room.leftWall > room.topWall - room.bottomWall)
        {
            int splitPoint = Split(room.leftWall, room.rightWall, splitVariance);

            //using the split point, make two new rooms split down the line along that point
            Room leftRoom = new Room(room.leftWall, splitPoint, room.bottomWall, room.topWall);
            Room rightRoom = new Room(splitPoint, room.rightWall, room.bottomWall, room.topWall);

            //since this room started as one big room, we need to make sure that doors end up in
            //the appropriate room. any doors that were on the left side of the split should be a
            //part of the left room, and vice versa
            foreach(Door doorAt in room.doors)
            {
                Room toGetDoor;

                if(doorAt.x < splitPoint)
                {
                    toGetDoor = leftRoom;
                }
                else
                {
                    toGetDoor = rightRoom;
                }

                toGetDoor.AddDoor(doorAt);

                //since we're splitting vertically, and there is a specific allowed variance for
                //door locations, we want to relocate a door so it is within the allowed variance.
                //for example, with 0 variance, doors are always in the center of a room. vertical
                //doors, however, would no longer be because we've split along the vertical axis.
                //fortunately, we can use the same split function in order to move the door within
                //the appropriate bounds
                if(!doorAt.horizontal)
                {
                    doorAt.x = Split(toGetDoor.leftWall, toGetDoor.rightWall, doorVariance);
                }
            }

            //after any necessary doors have been added and relocated, we need to add a new set of
            //doors between the two rooms we've just created. we've split vertically, so the doors
            //are going to be horizontal. in both cases, their location along the y axis will be
            //based on the door variance using the split function
            Door leftDoor = new Door(splitPoint, Split(room.bottomWall, room.topWall, doorVariance), true);
            Door rightDoor = new Door(splitPoint, Split(room.bottomWall, room.topWall, doorVariance), true);

            leftDoor.AssignDoor(rightDoor);
            rightDoor.AssignDoor(leftDoor);

            leftRoom.doors.Add(leftDoor);
            rightRoom.doors.Add(rightDoor);

            //now recursively split these subrooms
            BinarySpacePartition(leftRoom, levelsToGo);
            BinarySpacePartition(rightRoom, levelsToGo);
        }
        else
        {
            //split across the opposite dimension. the code here is largely the same as above, just
            //doing things in the x direction instead of y
            int splitPoint = Split(room.bottomWall, room.topWall, splitVariance);

            Room bottomRoom = new Room(room.leftWall, room.rightWall, room.bottomWall, splitPoint);
            Room topRoom = new Room(room.leftWall, room.rightWall, splitPoint, room.topWall);

            foreach(Door doorAt in room.doors)
            {
                Room toGetDoor;

                if(doorAt.y < splitPoint)
                {
                    toGetDoor = bottomRoom;
                }
                else
                {
                    toGetDoor = topRoom;
                }

                toGetDoor.AddDoor(doorAt);

                if(doorAt.horizontal)
                {
                    doorAt.y = Split(toGetDoor.bottomWall, toGetDoor.topWall, doorVariance);
                }
            }

            Door bottomDoor = new Door(Split(room.leftWall, room.rightWall, doorVariance), splitPoint, false);
            Door topDoor = new Door(Split(room.leftWall, room.rightWall, doorVariance), splitPoint, false);

            bottomDoor.AssignDoor(topDoor);
            topDoor.AssignDoor(bottomDoor);

            bottomRoom.AddDoor(bottomDoor);
            topRoom.AddDoor(topDoor);

            BinarySpacePartition(bottomRoom, levelsToGo);
            BinarySpacePartition(topRoom, levelsToGo);
        }
    }

    static int Split(int min, int max, float variance)
    {
        //get our random variance value, the amount from which we're deviating from the center
        //with no variance, we will always split right down the middle of a room.
        //the variance value will always be between -1 and 1. -1 inclusive, 1 exclusive
        float randomVariance = (float) (random.NextDouble() * 2 - 1);

        float midPoint = (min + max) / 2;

        //we're using the variance that was randomly generated as well as the given varaince
        //value, which will depend on the purposes for which the function is being used
        midPoint += randomVariance * variance * (max - min) / 2;

        return (int) Math.Round(midPoint, 0);
    }

    static void Draw()
    {
        //whoops not implemented yet
    }
}

class Room
{
    //room wall positions are gettable, but not settable after creation.
    //this may change when the shrinking algorithm is put in place
    public int leftWall { get; }
    public int rightWall { get; }
    public int topWall { get; }
    public int bottomWall { get; }

    public char id;

    public List<Door> doors { get; }

    public Room(int leftWall, int rightWall, int bottomWall, int topWall)
    {
        this.leftWall = leftWall;
        this.rightWall = rightWall;
        this.topWall = topWall;
        this.bottomWall = bottomWall;

        doors = new List<Door>();
    }

    public void AddDoor(Door door)
    {
        doors.Add(door);
    }
}

class Door
{
    //door x,y positions are completely public because they made need to be changed
    public int x;
    public int y;
    private int divDim;

    public bool horizontal { get; }

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