namespace JWTAuth.Service.Constants
{
    public class Authorization
    {
        public enum ERoles
        {
            Administrator,
            Moderator,
            User,
            Guest
        }
        public const string default_username = "user";
        public const string default_email = "user@secureapi.com";
        public const string default_password = "Asd123";
        public const ERoles default_role = ERoles.User;
    }
}
