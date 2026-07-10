namespace _Project._Scripts
{
    [System.Serializable]
    public class RayCastInfo
    {
        public float CollisionDistance { get; private set; }
        public bool HasCollision { get; private set; }

        public void SetInfo(float distance, bool hasCollision)
        {
            CollisionDistance = distance;
            HasCollision = hasCollision;
        }
    }
}