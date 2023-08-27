using MtdKey.Cipher;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

#if DEBUG
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

builder.Services.AddAesMangerService(options => {
    options.SecretKey = builder.Configuration["AesOptions:SecretKey"] ?? string.Empty;
    options.KeySize = int.Parse(builder.Configuration["AesOptions:KeySize"] ?? "256");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
