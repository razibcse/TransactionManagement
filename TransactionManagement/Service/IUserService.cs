namespace TransactionManagement.Service
{
    public interface IUserService
    {
        string GetUserId();
        bool isAuthenticated();
        public string FullName();
    }
}
