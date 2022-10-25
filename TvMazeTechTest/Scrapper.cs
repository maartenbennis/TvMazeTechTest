using NuGet.Configuration;
using TvMazeTechTest.Data;
using TvMazeTechTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;
using TvMazeTechTest.Controllers;
using Microsoft.Data.SqlClient;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;

namespace TvMazeTechTest
{
    public class Scrapper
    {        
        
        public DbContextOptions<ShowsContext> GetShowsContextOptions()
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            return new DbContextOptionsBuilder<ShowsContext>()
          .UseSqlServer(new SqlConnection(configuration.GetConnectionString("ShowsConnStr"))).Options;
        }

        public async Task GetDataAsync()
        {
            using (ShowsContext db = new ShowsContext(GetShowsContextOptions()))
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        int NextPage = 0;
                        //Get nextpage if exists else nextpage = 0
                        var NextPageRow = db.Settings.Where(x => x.Name == "NextPage").FirstOrDefault();
                        if (NextPageRow != null)
                        { NextPage = Convert.ToInt32(NextPageRow.Value); }

                        //Get data from nextpage
                        HttpClient client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.tvmaze.com/shows?page=" + NextPage.ToString());
                        var response = await client.SendAsync(request);

                        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotFound)
                        {
                            return; // Fetch Did not work, no worry try again next run
                        }

                        var data = await response.Content.ReadAsStringAsync();
                        if (data != null)
                        {
                            var lstShows = JsonConvert.DeserializeObject<List<Shows>>(data);
                            //Exclude shows that premiered for 01-01-2014
                            lstShows = lstShows.Where(x => x.Premiered >= new DateTime(2014, 1, 1)).ToList();

                            //Exclude shows that are already in the database, This will happen at repeating the final page
                            lstShows = lstShows.Where(x => !db.Shows.Any(x2 => x2.ID == x.ID)).ToList();

                            db.Shows.AddRange(lstShows);
                        }

                        //Check if next page exists for next run to see if we are at the last page
                        request = new HttpRequestMessage(HttpMethod.Get, "https://api.tvmaze.com/shows?page=" + (NextPage + 1).ToString());
                        response = await client.SendAsync(request);
                        if (response.IsSuccessStatusCode)// Fetch works, so nextpage exists and update pagevalue in DB
                        {
                            NextPage++;

                            //set nextpage = NextPage + 1 and add to DB                        
                            if (NextPage == 1)//Was run for the first time so no DB value Exists
                            {
                                db.Settings.Add(new Models.Settings() { Name = "NextPage", Value = NextPage.ToString() });
                                await db.SaveChangesAsync();
                            }
                            else//NextString exists so update
                            {
                                NextPageRow.Value = NextPage.ToString();
                                db.Settings.Add(NextPageRow);
                                db.Entry(NextPageRow).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        //Every thing is Ok, so make database changes
                        dbTransaction.Commit();
                    }
                    catch (Exception ex)
                    { }
                }

            }

                        
        }

    }
}
