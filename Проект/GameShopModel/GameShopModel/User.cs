namespace GameShopModel
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Balance { get; set; } = 0;
        public List<int> Cart_Games { get; set; } = new List<int>();
        public List<int> Cart_DLCs { get; set; } = new List<int>();
    }
}
