namespace Assets.KinectView.Lib
{
    public class ClothingItem
    {
        public ClothingType Type { get; private set; }
        public string Id { get; private set; }

        public ClothingItem(ClothingType type, string id)
        {
            Type = type;
            Id = id;
        }
    }
}