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
        /// <summary>
        /// Not intersecting
        /// </summary>
        Disjoint = 0,
        /// <summary>
        /// Fully contained within
        /// </summary>
        Contains = 1,
        /// <summary>
        /// Partially contained, not fully
        /// </summary>
        Intersects = 2
    }
    [Flags]
    public enum EContainmentFlags
    {
        None = 0,
        /// <summary>
        /// Not intersecting
        /// </summary>
        Disjoint = 1,
        /// <summary>
        /// Fully contained within
        /// </summary>
        Contains = 2,
        /// <summary>
        /// Partially contained, not fully
        /// </summary>
        Intersects = 4
    }
}
