//using AddPulseShowManagement.Business.Core;
using AddPulseShowManagement.Business.Core;
using AddPulseShowManagement.Common;
using AddPulseShowManagement.Data.DataTableModels;
using AddPulseShowManagement.Data.DBModels;
using AddPulseShowManagement.Repo;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Globalization;
using System.Text;


namespace AddPulseShowManagement
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        static IServiceProvider Provider { get; set; }
        public AppConfig AppConfig { get; set; }
       // public par AppConfig { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            this.AppConfig = this.Bind<AppConfig>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddMvc().AddSessionStateTempDataProvider();

            services.AddSession();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = this.AppConfig.jwtConfig.Issuer,
                        ValidateAudience = true,
                        ValidAudience = this.AppConfig.jwtConfig.Audience,
                        ValidateIssuerSigningKey = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.AppConfig.jwtConfig.Secret))
                    };
                }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ReturnUrlParameter = "ReturnUrl";
                });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                //set session timeout,time is coming from config,Currentelly set to 8 hour
                int sessionTimeout = 480;
                options.IdleTimeout = TimeSpan.FromMinutes(sessionTimeout);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

        }

        public virtual void ConfigureContainer(IServiceCollection container)
        {
            container.AddHttpContextAccessor();
            container.AddScoped<AppConfig>(s => this.AppConfig);
            container.AddDbContext<MSSQLDbContext>(options =>
            {
                options.UseSqlServer(this.Configuration.GetConnectionString("Default"));
            });
            #region Repository            
            
            container.AddScoped<UserRepository>();
            container.AddScoped<TeamRepository>();
            container.AddScoped<ShowRepository>();
            container.AddScoped<ContestantRepository>();
            
            #endregion

            #region Services
            
            container.AddScoped<IUserService, UserService>();
            container.AddScoped<ITeamService, TeamService>();
            container.AddScoped<IShowService, ShowService>();
            container.AddScoped<IContestantService, ContestantService>();
            
            #endregion

            container.AddScoped(typeof(IFactory<>), typeof(FactoryProvider<>));            
            container.AddScoped<IUserRepository>(f => f.GetRequiredService<IFactory<IUserRepository>>().CreateInstance<UserRepository>(f));
            container.AddScoped<ITeamRepository>(f => f.GetRequiredService<IFactory<ITeamRepository>>().CreateInstance<TeamRepository>(f));
            container.AddScoped<IShowRepository>(f => f.GetRequiredService<IFactory<IShowRepository>>().CreateInstance<ShowRepository>(f));
            container.AddScoped<IContestantRepository>(f => f.GetRequiredService<IFactory<IContestantRepository>>().CreateInstance<ContestantRepository>(f));
            
        }


        public static T Resolve<T>()
        {
            return Provider.GetRequiredService<T>();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo> { new CultureInfo("en-GB") },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-GB") },
                DefaultRequestCulture = new RequestCulture("en-GB")
            };

            app.UseRequestLocalization(localizationOptions);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = new PathString("/css")
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = new PathString("/fonts")
            });

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                //pattern: "{controller=Account}/{action=Login}/{id?}");
                //  endpoints.MapHub<BookingHub>("/booking-event-stream");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                 );
            });

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

           
           //FirebaseMethod();

        }
        public T Bind<T>(string? section = null)
        {
            var instance = Activator.CreateInstance(typeof(T));
            if (section == null)
                this.Configuration.Bind(instance);
            else
                this.Configuration.GetSection(section).Bind(instance);
            return instance != null ? (T)instance : default(T);
        }

        public void FirebaseMethod()
        {
            string filepath = this.AppConfig.fireBase.FireBaseEnvironment;
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);

            FirestoreDb db = FirestoreDb.Create("add-pulse-show");

            //DocumentReference docRef = db.Collection("Players").Document("SF");
            CollectionReference docRef = db.Collection("Players");
            Query query = db.Collection("Players").OrderBy("PlayerID");
            Query query1 = db.Collection("PlayerHistory");

            try
            {
                FirestoreChangeListener listener = query.Listen(async snapshot =>
                {
                    foreach (DocumentChange change in snapshot.Changes)
                    {
                        if (change.ChangeType.ToString() == "Added")
                        {
                            Console.WriteLine("New data: {0}", change.Document.Id);

                            //added data needed to be read
                            DocumentReference docRef1 = db.Collection("Players").Document(change.Document.Id);
                            DocumentSnapshot snapshot1 = await docRef1.GetSnapshotAsync();
                            if (snapshot1.Exists)
                            {
                                try
                                {
                                    Dictionary<string, object> city = snapshot1.ToDictionary();
                                    FireBaseContestants fireBaseContestant = CovertFunctionClass.GetObject<FireBaseContestants>(city);
                                    ExecuteSprocedure(fireBaseContestant, "APS_01_InsertUpdateFireBaseContestants");
                                    //from above line i'm getting obj

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("New data: {0}", change.Document.Id);
                                }
                                Console.WriteLine("Document data for {0} document:", snapshot1.Id);

                            }
                            else
                            {
                                Console.WriteLine("Document {0} does not exist!", snapshot1.Id);
                            }
                            //added data needed to be read

                        }
                        else if (change.ChangeType.ToString() == "Modified")
                        {
                            Console.WriteLine("Modified data: {0}", change.Document.Id);

                            //modified data needed to be read
                            DocumentReference docRef1 = db.Collection("Players").Document(change.Document.Id);
                            DocumentSnapshot snapshot1 = await docRef1.GetSnapshotAsync();
                            if (snapshot1.Exists)
                            {
                                Console.WriteLine("Document data for {0} document:", snapshot1.Id);
                                Dictionary<string, object> city = snapshot1.ToDictionary();

                                FireBaseContestants contestant = CovertFunctionClass.GetObject<FireBaseContestants>(city) ?? new FireBaseContestants();

                                ExecuteSprocedure(contestant, "APS_01_InsertUpdateFireBaseContestants");
                            }
                            else
                            {
                                Console.WriteLine("Document {0} does not exist!", snapshot1.Id);
                            }
                            //modified data needed to be read

                        }
                        else if (change.ChangeType.ToString() == "Removed")
                        {
                            // modifed with sp
                            DocumentReference docRef1 = db.Collection("Players").Document(change.Document.Id);
                            DocumentSnapshot snapshot1 = await docRef1.GetSnapshotAsync();
                            ExecuteDeleteQuery(snapshot1.Id);
                            //if (snapshot1.Exists)
                            //{
                            //    Console.WriteLine("Document data for {0} document:", snapshot1.Id);
                            //    Dictionary<string, object> city = snapshot1.ToDictionary();

                            //    FireBaseContestants contestant = CovertFunctionClass.GetObject<FireBaseContestants>(city) ?? new FireBaseContestants();


                            //}
                            Console.WriteLine("Removed data: {0}", change.Document.Id);
                        }
                    }
                });
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Offline");
            }

            

            //for history of players

            FirestoreChangeListener listener1 = query1.Listen(async snapshot =>
            {
                try
                {
                    foreach (DocumentChange change in snapshot.Changes)
                    {
                        if (change.ChangeType.ToString() == "Added")
                        {
                            Console.WriteLine("New data: {0}", change.Document.Id);

                            //added data needed to be read
                            DocumentReference docRef1 = db.Collection("PlayerHistory").Document(change.Document.Id);
                            DocumentSnapshot snapshot1 = await docRef1.GetSnapshotAsync();
                            if (snapshot1.Exists)
                            {
                                try
                                {
                                    Dictionary<string, object> city = snapshot1.ToDictionary();
                                    FireBaseContestantsHistory fireBaseContestant = CovertFunctionClass.GetObject<FireBaseContestantsHistory>(city) ?? new FireBaseContestantsHistory();
                                    ExecuteHistoryProcedure(fireBaseContestant, change.Document.Id);
                                    //from above line i'm getting obj
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("New data: {0}", change.Document.Id);
                                }
                                Console.WriteLine("Document data for {0} document:", snapshot1.Id);

                            }
                            else
                            {
                                Console.WriteLine("Document {0} does not exist!", snapshot1.Id);
                            }
                            //added data needed to be read

                        }
                        else if (change.ChangeType.ToString() == "Modified")
                        {
                            Console.WriteLine("Modified data: {0}", change.Document.Id);

                            //modified data needed to be read
                            DocumentReference docRef1 = db.Collection("PlayerHistory").Document(change.Document.Id);
                            DocumentSnapshot snapshot1 = await docRef1.GetSnapshotAsync();
                            if (snapshot1.Exists)
                            {
                                Console.WriteLine("Document data for {0} document:", snapshot1.Id);
                                Dictionary<string, object> city = snapshot1.ToDictionary();

                                FireBaseContestantsHistory contestant = CovertFunctionClass.GetObject<FireBaseContestantsHistory>(city) ?? new FireBaseContestantsHistory();

                                ExecuteHistoryProcedure(contestant, change.Document.Id);
                            }
                            else
                            {
                                Console.WriteLine("Document {0} does not exist!", snapshot1.Id);
                            }
                            //modified data needed to be read

                        }
                        else if (change.ChangeType.ToString() == "Removed")
                        {

                            DocumentReference docRef1 = db.Collection("PlayerHistory").Document(change.Document.Id);
                            DocumentSnapshot snapshot1 = await docRef1.GetSnapshotAsync();
                            ExecuteHistoryDeleteQuery(snapshot1.Id);
                            //            if (snapshot1.Exists)
                            //            {
                            //                Console.WriteLine("Document data for {0} document:", snapshot1.Id);
                            //                //Dictionary<string, object> city = snapshot1.ToDictionary();

                            //                //FireBaseContestantsHistory contestant = CovertFunctionClass.GetObject<FireBaseContestantsHistory>(city) ?? new FireBaseContestantsHistory();

                            //              
                            //            }
                            Console.WriteLine("Removed data: {0}", change.Document.Id);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Offline server");
                }
               
            });
        }       

        public int ExecuteDeleteQuery(string PlayerID="")
        {
            try
            {
               // if (PlayerID > 0)
                if (!string.IsNullOrEmpty(PlayerID))
                {
                    SqlConnection con = new SqlConnection(this.AppConfig.connectionStrings.Default);
                    SqlCommand cmd = new SqlCommand("APS_01_DeleteFireBaseContestants", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PlayerID", PlayerID);

                    con.Open();
                    int isRows = cmd.ExecuteNonQuery();

                    con.Close();
                    return isRows;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }


        public int ExecuteSprocedure(FireBaseContestants fc,string procedureName ="")
        {
            try
            {
                //if (fc.PlayerID > 0)
                if (!string.IsNullOrEmpty(fc.PlayerID))
                {
                    SqlConnection con = new SqlConnection(this.AppConfig.connectionStrings.Default);
                    SqlCommand cmd = new SqlCommand(procedureName, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@PlayerID", fc.PlayerID);
                    //cmd.Parameters.AddWithValue("@PolarID", fc.PolarID);
                    //cmd.Parameters.AddWithValue("@AveragePulse", fc.AveragePulse);
                    //cmd.Parameters.AddWithValue("@Name", fc.Name);
                    //

                    cmd.Parameters.Add("@PlayerID", SqlDbType.VarChar).Value = fc.PlayerID.ToString();
                    cmd.Parameters.Add("@PolarID", SqlDbType.NVarChar).Value = fc.PolarID;
                    cmd.Parameters.Add("@AveragePulse", SqlDbType.Int).Value = fc.AveragePulse;
                    cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = fc.Name;
                    cmd.Parameters.Add("@TimeStamp", SqlDbType.DateTime).Value = fc.TimeStamp;

                    con.Open();
                    int isRows = cmd.ExecuteNonQuery();

                    con.Close();
                    return isRows;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
           
        }



        public int ExecuteHistoryDeleteQuery(string docID="")
        {
            try
            {
                if (!string.IsNullOrEmpty(docID))
                {
                    SqlConnection con = new SqlConnection(this.AppConfig.connectionStrings.Default);
                    SqlCommand cmd = new SqlCommand("APS_01_DeleteFireBaseContestantsHistory", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DocumentID", docID);

                    con.Open();
                    int isRows = cmd.ExecuteNonQuery();

                    con.Close();
                    return isRows;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public int ExecuteHistoryProcedure(FireBaseContestantsHistory fc,string docID="")
        {
            try
            {
                if ((!string.IsNullOrEmpty(fc.PlayerID)) && (!string.IsNullOrEmpty(docID)))
                //if (fc.PlayerID > 0 && (!string.IsNullOrEmpty(docID)))
                {
                    SqlConnection con = new SqlConnection(this.AppConfig.connectionStrings.Default);
                    SqlCommand cmd = new SqlCommand("APS_01_InsertUpdateFireBaseContestantsHistory", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                   
                    cmd.Parameters.Add("@DocumentID", SqlDbType.NVarChar).Value = docID;
                    cmd.Parameters.Add("@PlayerID", SqlDbType.VarChar).Value = fc.PlayerID.ToString();
                    cmd.Parameters.Add("@PolarID", SqlDbType.NVarChar).Value = fc.PolarID;
                    cmd.Parameters.Add("@AveragePulse", SqlDbType.Int).Value = fc.AveragePulse;
                    cmd.Parameters.Add("@LivePulse", SqlDbType.Int).Value = fc.LivePulse;
                    cmd.Parameters.Add("@TimeStamp", SqlDbType.DateTime).Value = fc.TimeStamp;

                    con.Open();
                    int isRows = cmd.ExecuteNonQuery();

                    con.Close();
                    return isRows;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
    }

    

}
