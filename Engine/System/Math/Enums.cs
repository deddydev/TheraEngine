namespace System
{
    public enum EPlaneIntersection
    {
        Back,
        Front,
        Intersecting,
    }
    public enum EContainment
    {
        Disjoint = 0,   //Not intersecting
        Contains = 1,   //Fully contained within
        Intersects = 2  //Partially contained, not fully
    }
}
