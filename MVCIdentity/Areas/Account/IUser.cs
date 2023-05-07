namespace MVCIdentity.Areas.Account
{
    public interface IUser
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Id { get; set; }
    }
}
