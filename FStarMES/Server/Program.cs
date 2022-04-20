using FStarMES.Data;
using FStarMES.Shared;
using Microsoft.EntityFrameworkCore;
using ShardingCore;
using ShardingCore.Bootstrapers;
using ShardingCore.TableExists;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddShardingDbContext<MyDbContext>().AddEntityConfig(options =>
{
    //如果您使用code-first建议选择false
    options.CreateShardingTableOnStart = true;
    //如果您使用code-first建议修改为fsle
    options.EnsureCreatedWithOutShardingTable = true;
    //当无法获取路由时会返回默认值而不是报错
    options.ThrowIfQueryRouteNotMatch = false;
    options.AddShardingTableRoute<MachineStateVirtualTableRoute>();
    options.UseShardingQuery((connStr, builder) =>
    {
        //connStr is delegate input param
        builder.UseMySql(connStr, new MySqlServerVersion(new Version(8, 0, 26)));
    });
    options.UseShardingTransaction((connection, builder) =>
    {
        builder.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 26)));
    });
    //options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 26)));
}).AddConfig(op => {
    op.ConfigId = "c1";
    op.AddDefaultDataSource("ds0",
        builder.Configuration.GetConnectionString("DefaultConnection"));

    //op.AddDefaultDataSource("ds0", "server=127.0.0.1;port=3306;database=db2;userid=root;password=L6yBtV6qNENrwBy7;")
    op.ReplaceTableEnsureManager(sp => new MySqlTableEnsureManager<MyDbContext>());
}).EnsureConfig();
//builder.WebHost.UseUrls("https://0.0.0.0:5051;http://0.0.0.0:5050");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.Services.GetRequiredService<IShardingBootstrapper>().Start();
using (var scope = app.Services.CreateScope())
{
    var myDbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    if (!myDbContext.Set<MachineState>().Any())
    {
        MachineState machineState = new MachineState() {
            Line = "1",
            Station = "下料机",
            Count = 10,
            TotalCount = 100,
            StateStatus = StateStatusEnum.Error,
            ProductType = "包装",
            CreationTime = DateTime.Now
        };
        myDbContext.Add<MachineState>(machineState);
        MachineState machineState1 = new MachineState()
        {
            Line = "1",
            Station = "下料机",
            Count = 10,
            TotalCount = 100,
            StateStatus = StateStatusEnum.Wait,
            ProductType = "包装",
            CreationTime = DateTime.Now
        };
        myDbContext.Add<MachineState>(machineState1);
        myDbContext.SaveChanges();
    }
    if (!myDbContext.Set<LineSeting>().Any())
    {
        LineSeting machineLineSeting = new LineSeting() { 
            Line = "1",
            GoalCount = 5000,
            ShiftType = ShiftTypeEnum.Day
        };
        myDbContext.Add<LineSeting>(machineLineSeting);
        myDbContext.SaveChanges();
    }
}
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
