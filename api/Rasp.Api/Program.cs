using Npgsql;
using Microsoft.EntityFrameworkCore;
using Rasp.Api.Data;
using Rasp.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Swagger (OpenAPI + UI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RaspDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// GET /status-rasp
app.MapGet("/status-rasp", async (RaspDbContext db) =>
{
    var itens = await db.StatusRasp
        .OrderBy(s => s.OrdemFluxo)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarStatusRasp");

// GET /status-rasp/{id}
app.MapGet("/status-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.StatusRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Status não encontrado.")
        : Results.Ok(item);
});

// GET /fornecedor-rasp
app.MapGet("/fornecedor-rasp", async (RaspDbContext db) =>
{
    var itens = await db.FornecedorRasp
        .OrderBy(f => f.Nome)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarFornecedorRasp");

// GET /fornecedor-rasp/{id}
app.MapGet("/fornecedor-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.FornecedorRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Fornecedor não encontrado.")
        : Results.Ok(item);
});

// GET /perfil-rasp
app.MapGet("/perfil-rasp", async (RaspDbContext db) =>
{
    var itens = await db.PerfilRasp
        .OrderBy(p => p.Nome)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarPerfilRasp");

// GET /perfil-rasp/{id}
app.MapGet("/perfil-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.PerfilRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Perfil não encontrado.")
        : Results.Ok(item);
});

// GET /usuarios
app.MapGet("/usuarios", async (RaspDbContext db) =>
{
    var itens = await db.Usuarios
        .OrderBy(u => u.Nome)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarUsuarios");

// GET /usuarios/{id}
app.MapGet("/usuarios/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.Usuarios.FindAsync(id);

    return item is null
        ? Results.NotFound("Usuário não encontrado.")
        : Results.Ok(item);
});

// GET /rasp
app.MapGet("/rasp", async (RaspDbContext db) =>
{
    var itens = await db.Set<RaspEntity>()
        .OrderByDescending(r => r.IdRasp)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRasp");

// GET /rasp-pn
app.MapGet("/rasp-pn", async (RaspDbContext db) =>
{
    var itens = await db.Set<RaspPnEntity>()
        .OrderByDescending(rp => rp.IdRaspPn)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspPn");

// POST /rasp -> cria um RASP (rascunho)
app.MapPost("/rasp", async (CriarRaspRequest req, IConfiguration config) =>
{
    if (req.IdFornecedorRasp <= 0)
        return Results.BadRequest("IdFornecedorRasp inválido.");

    if (string.IsNullOrWhiteSpace(req.DescricaoProblema))
        return Results.BadRequest("DescricaoProblema é obrigatória.");

    var connStr = config.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connStr))
        return Results.Problem("Connection string 'DefaultConnection' não encontrada.");

    await using var conn = new NpgsqlConnection(connStr);
    await conn.OpenAsync();

    await using var tx = await conn.BeginTransactionAsync();

    var insertSql = @"
        INSERT INTO rasp
            (numero_rasp, data_criacao, hora_criacao, id_fornecedor_rasp, descricao_problema, id_status_rasp, is_rascunho, percentual_completude)
        VALUES
            ('TEMP', CURRENT_DATE, CURRENT_TIME, @id_fornecedor, @descricao, 1, true, 0)
        RETURNING id_rasp;
    ";

    int idRasp;
    await using (var cmd = new NpgsqlCommand(insertSql, conn, tx))
    {
        cmd.Parameters.AddWithValue("id_fornecedor", req.IdFornecedorRasp);
        cmd.Parameters.AddWithValue("descricao", req.DescricaoProblema.Trim());
        idRasp = (int)(await cmd.ExecuteScalarAsync() ?? 0);
    }

    if (idRasp <= 0)
    {
        await tx.RollbackAsync();
        return Results.Problem("Falha ao gerar id_rasp.");
    }

    var updateSql = @"
        UPDATE rasp
        SET numero_rasp = LPAD(@id::text, 4, '0') || '/' || to_char(CURRENT_DATE, 'YY')
        WHERE id_rasp = @id;
    ";

    await using (var cmd2 = new NpgsqlCommand(updateSql, conn, tx))
    {
        cmd2.Parameters.AddWithValue("id", idRasp);
        await cmd2.ExecuteNonQueryAsync();
    }

    await tx.CommitAsync();

    return Results.Created($"/rasp/{idRasp}", new { id_rasp = idRasp });
})
.WithName("CriarRasp");

// Endpoint exemplo
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/debug-model", (RaspDbContext db) =>
{
    var entidades = db.Model.GetEntityTypes()
        .Select(e => new
        {
            Nome = e.Name,
            Tabela = e.GetTableName()
        })
        .OrderBy(e => e.Nome)
        .ToList();

    return Results.Ok(entidades);
})
.WithName("DebugModel");

app.Run();

public record CriarRaspRequest(int IdFornecedorRasp, string DescricaoProblema);

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}