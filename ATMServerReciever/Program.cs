var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/center", () =>
{
    return "voce esta no centro";
})
.WithName("com_centerGET")
.WithOpenApi();

app.MapPost("/center", (ReqModel model) => {
    return $"post realizado {model.Name} e {model.Idade}";
}).WithName("com_centerPOST").WithOpenApi();

app.Run();

