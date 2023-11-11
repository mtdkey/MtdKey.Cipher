using MtdKey.Cipher;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

#if DEBUG
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

builder.Services.AddAesMangerService(options => {
    options.SecretKey = builder.Configuration.GetValue<string>("AesOptions:SecretKey") ?? string.Empty;
    options.KeySize = builder.Configuration.GetValue<int>("AesOptions:KeySize");
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
