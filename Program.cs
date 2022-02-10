using GeneratePassword_v3;

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



app.MapGet("/v2/generate", (string? length, string? count, string? numbers, string? upperChars, string? lowerChars, string? specialChars,
    string? mustHave, string? startsWith, string? endsWith, string? include, string? exclude, string? type) =>
{


    var data = PasswordGenerate.GeneratePasswordList(length, count, numbers, upperChars, lowerChars, specialChars, mustHave, startsWith, endsWith, include, exclude);

    if (data != null)
    {
        if (type == "text")
        {
            var response = String.Empty;
            foreach(var password in data)
            {
                response += (password + "\n");
            }            
            return Results.Text(response);
        }
        else
        {
            return Results.Ok(data);
        }
    }

    else
        return Results.NotFound();

})
.WithName("GetNew");

app.Run();
