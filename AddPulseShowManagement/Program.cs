using AddPulseShowManagement;
using Google.Cloud.Firestore;
using Serilog;

public class Program
{
    public static string ENV_PREFIX => "AddPulse_ENVIRONMENT";
    public static string ENV =>
        Environment.GetEnvironmentVariable(ENV_PREFIX) ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    // Development, Production, MyEnvironment
    public static IConfiguration ConfigurationBuilder => new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{ENV}.json", true)
                .AddEnvironmentVariables()
                .Build();

    public static void Main(string[] args)
    {
        Console.WriteLine($"Environment - {ENV}");


        var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .Build();

        Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();


        CreateHostBuilder(args, ConfigurationBuilder).Build().Run();

        
        //can we call firebase listener
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {

                webBuilder.UseStartup(f => new Startup(configuration));
                webBuilder.UseConfiguration(configuration);
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
            }).UseSerilog();


    //public static void FirebaseMethod()
    //{
    //    string filepath = Path.Combine(Directory.GetCurrentDirectory(), @"\add-pulse-show-c0567f71dd35.json");//this.AppConfig.fireBase.FireBaseEnvironment;
    //    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);

    //    FirestoreDb db = FirestoreDb.Create("add-pulse-show");

        
    //    CollectionReference docRef = db.Collection("Players");
    //    Query query = db.Collection("Players").OrderBy("PlayerID");
        
    //    FirestoreChangeListener listener = query.Listen(async snapshot =>
    //    {
    //        foreach (DocumentChange change in snapshot.Changes)
    //        {
    //            if (change.ChangeType.ToString() == "Added")
    //            {
    //                Console.WriteLine("New data: {0}", change.Document.Id);

    //                //added data needed to be read
    //                DocumentReference docRef1 = db.Collection("Players").Document(change.Document.Id);
    //                DocumentSnapshot snapshot1 = await docRef1.GetSnapshotAsync();
    //                if (snapshot1.Exists)
    //                {
    //                    Console.WriteLine("Document data for {0} document:", snapshot1.Id);
    //                    Dictionary<string, object> city = snapshot1.ToDictionary();
    //                    foreach (KeyValuePair<string, object> pair in city)
    //                    {
    //                        Console.WriteLine("Added data {0}: {1}", pair.Key, pair.Value);
    //                    }
    //                }
    //                else
    //                {
    //                    Console.WriteLine("Document {0} does not exist!", snapshot1.Id);
    //                }
    //                //added data needed to be read

    //            }
    //            else if (change.ChangeType.ToString() == "Modified")
    //            {
    //                Console.WriteLine("Modified data: {0}", change.Document.Id);

    //                //modified data needed to be read
    //                DocumentReference docRef1 = db.Collection("Players").Document(change.Document.Id);
    //                DocumentSnapshot snapshot1 = await docRef1.GetSnapshotAsync();
    //                if (snapshot1.Exists)
    //                {
    //                    Console.WriteLine("Document data for {0} document:", snapshot1.Id);
    //                    Dictionary<string, object> city = snapshot1.ToDictionary();
    //                    foreach (KeyValuePair<string, object> pair in city)
    //                    {
    //                        Console.WriteLine("Modified data {0}: {1}", pair.Key, pair.Value);
    //                    }
    //                }
    //                else
    //                {
    //                    Console.WriteLine("Document {0} does not exist!", snapshot1.Id);
    //                }
    //                //modified data needed to be read

    //            }
    //            else if (change.ChangeType.ToString() == "Removed")
    //            {
    //                Console.WriteLine("Removed data: {0}", change.Document.Id);
    //            }
    //        }
    //    });

    //}
}