using MtdKey.Cipher;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAesMangerService(options => {
    options.SecretKey = builder.Configuration["AesOptions:SecretKey"] ?? string.Empty;
    options.KeySize = int.Parse(builder.Configuration["AesOptions:KeySize"] ?? "256");
});

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
