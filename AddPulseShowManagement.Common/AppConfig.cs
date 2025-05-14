namespace AddPulseShowManagement.Common
{
    public class AppConfig
    {       
        public class JwtConfig
        {
            public string Audience { get; set; }
            public string Issuer { get; set; }
            public string Secret { get; set; }
            public int Expiry { get; set; }
        }

        public class StaticData
        {
            public int SessionTimeout { get; set; }
        }
        public class MailSettings
        {
            public string FromEmail { get; set; }
            public string ReplyEmail { get; set; }
            public string DisplayName { get; set; }           
        }

        public class FileLocations
        {
          
            public string UserProfileImage { get; set; }
          
            public string DefaultUserLogo { get; set; }
            public string UserProfileUploadImage { get; set; }           
            public string ContestantImage { get; set; }           
        }
        public class FireBase
        {
            public string FireBaseEnvironment { get; set; }
        }

        public class ConnectionStrings
        {
            public string Default { get; set; }
         }
         public JwtConfig jwtConfig { get; set; }
        
        public StaticData staticData { get; set; }
        public MailSettings mailSettings { get; set; }
        public FireBase fireBase { get; set; }
        public FileLocations fileLocations { get; set; }
        public ConnectionStrings connectionStrings { get; set; }

    }

}