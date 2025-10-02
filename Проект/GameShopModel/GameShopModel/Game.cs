namespace GameShopModel
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int Sale { get; set; } = 0;
        public int Quantity { get; set; }
    }
}
