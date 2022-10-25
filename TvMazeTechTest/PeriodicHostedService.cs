namespace TvMazeTechTest
{
    public class PeriodicHostedService : BackgroundService
    {     
        private readonly TimeSpan _period = TimeSpan.FromSeconds(10);    
       
        public PeriodicHostedService()
        {                
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);
            while (
                !stoppingToken.IsCancellationRequested &&
                await timer.WaitForNextTickAsync(stoppingToken))
            {     
                try
                {
                    Scrapper scrapper = new Scrapper();
                    await scrapper.GetDataAsync();
                }
                catch (Exception ex) 
                { }
            }

        }


    }
}
