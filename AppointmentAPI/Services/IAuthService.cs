namespace AppointmentAPI.Services
{
    public interface IAuthService
    {
        string RegisterUser(string username, string password);
        string AuthenticateUser(string username, string password);
    }
}
