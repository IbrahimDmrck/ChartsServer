using ChartsServer.Hubs;
using ChartsServer.Models;
using Microsoft.AspNetCore.SignalR;
using TableDependency.SqlClient;

namespace ChartsServer.Subscription
{
    public interface IDatabaseSubscription
    {
        void Configure(string tableName);
    }
    public class DatabaseSubscription<T> : IDatabaseSubscription where T : class, new()
    {
        IConfiguration _configuration;
        IHubContext<SatisHub> _hubContext;
        public DatabaseSubscription(IConfiguration configuration, IHubContext<SatisHub> hubContext)
        {
            _configuration = configuration;
            _hubContext = hubContext;
        }
        SqlTableDependency<T> _tableDependency;
        public void Configure(string tableName)
        {
            Console.WriteLine(tableName);
            _tableDependency = new SqlTableDependency<T>(_configuration.GetConnectionString("SQL"), tableName);
            _tableDependency.OnChanged += async (o, e) =>
            {
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                    Console.WriteLine($"{prop.Name} : {prop.GetValue(e.Entity)}");



                SatisDbContext context = new();
                var data = (from personel in context.Personellers
                            join satis in context.Satislars on
                            personel.Id equals satis.PersonelId
                            select new { personel, satis }).ToList();

                List<object> datas = new();
                var personelIsimleri = data.Select(d => d.personel.Adi).Distinct().ToList();
                personelIsimleri.ForEach(p =>
                {
                    datas.Add(new
                    {
                        name = p,
                        data = data.Where(s => s.personel.Adi == p).Select(s => s.satis.Fiyat).ToList()
                    });
                });

                await _hubContext.Clients.All.SendAsync("receiveMessage", datas);
            };
            _tableDependency.OnError += (sender, e) => Console.WriteLine("Hata :)");
            _tableDependency.Start();
        }
        //DeConstructor : Method/Nesne imha edilirken bir işlem yapmak için
        ~DatabaseSubscription()
        {
            _tableDependency.Stop();
        }
    }
}
