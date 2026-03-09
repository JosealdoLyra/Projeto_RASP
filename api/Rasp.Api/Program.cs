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

// GET /pn-rasp
app.MapGet("/pn-rasp", async (RaspDbContext db) =>
{
    var itens = await db.PnRasp
        .OrderBy(p => p.CodigoPn)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarPnRasp");

// GET /pn-rasp/codigo/{codigoPn}
app.MapGet("/pn-rasp/codigo/{codigoPn}", async (string codigoPn, RaspDbContext db) =>
{
    var codigoLimpo = (codigoPn ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(codigoLimpo))
        return Results.BadRequest("CodigoPn é obrigatório.");

    var item = await db.PnRasp
        .FirstOrDefaultAsync(p => p.CodigoPn == codigoLimpo);

    return item is null
        ? Results.NotFound("PN não encontrado para o código informado.")
        : Results.Ok(item);
});

// POST /pn-rasp
app.MapPost("/pn-rasp", async (CriarPnRaspRequest req, RaspDbContext db) =>
{
    var codigoPn = (req.CodigoPn ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(codigoPn))
        return Results.BadRequest("CodigoPn é obrigatório.");

    if (codigoPn.Length != 8)
        return Results.BadRequest("CodigoPn deve ter exatamente 8 caracteres.");

    var nomePeca = (req.NomePeca ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(nomePeca))
        return Results.BadRequest("NomePeca é obrigatório.");

    var codigoJaExiste = await db.PnRasp
        .AnyAsync(p => p.CodigoPn == codigoPn);

    if (codigoJaExiste)
        return Results.BadRequest("Já existe PN cadastrado com esse código.");

    var item = new PnRasp
    {
        CodigoPn = codigoPn,
        NomePeca = nomePeca
    };

    db.PnRasp.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/pn-rasp/{item.IdPn}", item);
})
.WithName("CriarPnRasp");

// GET /fornecedor-rasp
app.MapGet("/fornecedor-rasp", async (RaspDbContext db) =>
{
    var itens = await db.FornecedorRasp
        .OrderBy(f => f.Nome)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarFornecedorRasp");

// GET /fornecedor-rasp/duns/{duns}
app.MapGet("/fornecedor-rasp/duns/{duns}", async (string duns, RaspDbContext db) =>
{
    var dunsLimpo = (duns ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(dunsLimpo))
        return Results.BadRequest("Duns é obrigatório.");

    var item = await db.FornecedorRasp
        .FirstOrDefaultAsync(f => f.Duns == dunsLimpo);

    return item is null
        ? Results.NotFound("Fornecedor não encontrado para o DUNS informado.")
        : Results.Ok(item);
});

// GET /fornecedor-rasp/{id}
app.MapGet("/fornecedor-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.FornecedorRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Fornecedor não encontrado.")
        : Results.Ok(item);
});

// POST /fornecedor-rasp
app.MapPost("/fornecedor-rasp", async (CriarFornecedorRaspRequest req, RaspDbContext db) =>
{
    var duns = (req.Duns ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(duns))
        return Results.BadRequest("Duns é obrigatório.");

    if (duns.Length != 8)
        return Results.BadRequest("Duns deve ter exatamente 8 caracteres.");

    var nome = (req.Nome ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(nome))
        return Results.BadRequest("Nome é obrigatório.");

    var tipoFornecedor = (req.TipoFornecedor ?? string.Empty).Trim().ToUpper();
    if (string.IsNullOrWhiteSpace(tipoFornecedor))
        return Results.BadRequest("TipoFornecedor é obrigatório.");

    if (tipoFornecedor != "LOCAL" && tipoFornecedor != "IMPORTADO")
        return Results.BadRequest("TipoFornecedor deve ser LOCAL ou IMPORTADO.");

    if (req.IdPais <= 0)
        return Results.BadRequest("IdPais inválido.");

    var dunsJaExiste = await db.FornecedorRasp
        .AnyAsync(f => f.Duns == duns);

    if (dunsJaExiste)
        return Results.BadRequest("Já existe fornecedor cadastrado com esse DUNS.");

    var item = new FornecedorRasp
    {
        Duns = duns,
        Nome = nome,
        TipoFornecedor = tipoFornecedor,
        Ativo = req.Ativo,
        IdPais = req.IdPais
    };

    db.FornecedorRasp.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/fornecedor-rasp/{item.IdFornecedor}", item);
})
.WithName("CriarFornecedorRasp");

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

// GET /rasp/{id}
app.MapGet("/rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.Rasp.FindAsync(id);

    return item is null
        ? Results.NotFound("RASP não encontrado.")
        : Results.Ok(item);
});

// POST /rasp -> cria um RASP (rascunho)
app.MapPost("/rasp", async (CriarRaspRequest req, RaspDbContext db, IConfiguration config) =>
{
    if (req.IdFornecedorRasp <= 0)
        return Results.BadRequest("IdFornecedorRasp inválido.");

    if (string.IsNullOrWhiteSpace(req.DescricaoProblema))
        return Results.BadRequest("DescricaoProblema é obrigatória.");

    var fornecedorExiste = await db.FornecedorRasp
        .AnyAsync(f => f.IdFornecedor == req.IdFornecedorRasp);

    if (!fornecedorExiste)
        return Results.BadRequest("Fornecedor informado não existe.");

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

// GET /rasp-pn
app.MapGet("/rasp-pn", async (RaspDbContext db) =>
{
    var itens = await db.Set<RaspPnEntity>()
        .OrderByDescending(rp => rp.IdRaspPn)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspPn");

// GET /rasp-pn/{id}
app.MapGet("/rasp-pn/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.RaspPn.FindAsync(id);

    return item is null
        ? Results.NotFound("RASP PN não encontrado.")
        : Results.Ok(item);
});

// POST /rasp-pn
app.MapPost("/rasp-pn", async (CriarRaspPnRequest req, RaspDbContext db) =>
{
    if (req.IdRasp <= 0)
        return Results.BadRequest("IdRasp inválido.");

    var raspExiste = await db.Rasp.AnyAsync(r => r.IdRasp == req.IdRasp);
    if (!raspExiste)
        return Results.BadRequest("RASP informado não existe.");

    var pn = (req.Pn ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(pn))
        return Results.BadRequest("Pn é obrigatório.");

    if (pn.Length != 8)
        return Results.BadRequest("Pn deve ter exatamente 8 caracteres.");

    var duns = (req.Duns ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(duns))
        return Results.BadRequest("Duns é obrigatório.");

    if (duns.Length != 9)
        return Results.BadRequest("Duns deve ter exatamente 9 caracteres.");

    if (req.QuantidadeSuspeita < 0 || req.QuantidadeChecada < 0 || req.QuantidadeRejeitada < 0)
        return Results.BadRequest("Quantidades não podem ser negativas.");

    if (req.OrdemExibicao <= 0)
        return Results.BadRequest("OrdemExibicao deve ser maior que zero.");

    var item = new RaspPnEntity
    {
        IdRasp = req.IdRasp,
        Pn = pn,
        QuantidadeSuspeita = req.QuantidadeSuspeita,
        QuantidadeChecada = req.QuantidadeChecada,
        QuantidadeRejeitada = req.QuantidadeRejeitada,
        EmContencao = req.EmContencao,
        Duns = duns,
        OrdemExibicao = req.OrdemExibicao
    };

    db.RaspPn.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/rasp-pn/{item.IdRaspPn}", item);
})
.WithName("CriarRaspPn");

// Endpoint exemplo
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
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

public record CriarRaspPnRequest(
    int IdRasp,
    string Pn,
    int QuantidadeSuspeita,
    int QuantidadeChecada,
    int QuantidadeRejeitada,
    bool EmContencao,
    string Duns,
    short OrdemExibicao
);

public record CriarFornecedorRaspRequest(
    string Duns,
    string Nome,
    string TipoFornecedor,
    bool Ativo,
    int IdPais
);

public record CriarPnRaspRequest(
    string CodigoPn,
    string NomePeca
);

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}