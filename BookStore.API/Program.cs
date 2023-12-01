using BookStore.API.Data;
using BookStore.API.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDbContext<BookStoreContext>();                      //DI  define connection string

//or

//builder.Services.AddDbContext<BookStoreContext>(                          //define connection string
//    options => options.UseSqlServer("Server=.;Database=BookStoreAPI;Integrated Security=True"));


string connectionString = builder.Configuration.GetConnectionString("BookStoreDB"); //define connection string which is defined in appsettings.json

builder.Services.AddDbContext<BookStoreContext>( //let us get instance of BookStoreContext
    options => options.UseSqlServer(connectionString));


//builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddTransient<IBookRepository, BookRepository>();      //DI

builder.Services.AddAutoMapper(typeof(Program));      //automapper service added

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()); // //resolves Corse error when api is connected to angular //globally
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider("D:\\CsharpProjects\\BookStore.API\\BookStore.API\\product_images"),
    RequestPath = "/product_images"
});


app.UseAuthorization();

app.MapControllers();

app.Run();
