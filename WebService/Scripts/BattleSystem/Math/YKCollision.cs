namespace BattleSystem
{
    public static class YKCollision
    {
        public struct Sphere
        {
            public YKVector2d Center;
            public double Radius;

            public Sphere(YKVector2d center, double radius)
            {
                Center = center;
                Radius = radius;
            }
        }

        public static bool IsCollide(Sphere sphere1, Sphere sphere2)
        {
            var sqrDis = sphere1.Radius + sphere2.Radius;
            sqrDis *= sqrDis;

            return (sphere1.Center - sphere2.Center).SqrMagnitude <= sqrDis;
        }
    }
}
