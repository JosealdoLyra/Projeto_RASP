using System.Text.Json.Serialization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Rasp.Api.Data;
using Rasp.Api.Models;
using Rasp.Api.Models.Requests;
using Rasp.Api.Models.Auth;
using Rasp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 01. CORS
// -----------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFront", policy =>
    {
        policy
            .WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// -----------------------------------------------------------------------------
// 02. SERVIÇOS DA APLICAÇÃO
// -----------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RaspDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================================
// MÉTODO AUXILIAR - HISTÓRICO DE FLUXO RASP
// ============================================================================
async Task RegistrarHistoricoFluxoAsync(
    RaspDbContext db,
    int idRasp,
    int idUsuario,
    string acao,
    int? statusAnterior,
    int? statusNovo,
    string? observacao,
    string origemEvento,
    string? tipoComplemento = null)
{
    // ------------------------------------------------------------------------
    // BUSCA ÚLTIMO HISTÓRICO DO RASP
    // ------------------------------------------------------------------------
    var ultimoHistorico = await db.Set<RaspHistoricoFluxoEntity>()

        .Where(x => x.IdRasp == idRasp)
        .OrderByDescending(x => x.DataHora)
        .FirstOrDefaultAsync();

    // ------------------------------------------------------------------------
    // DEFINE DATA INICIAL DA FASE
    // ------------------------------------------------------------------------
    DateTime? dataHoraAnterior = null;

    if (ultimoHistorico?.DataHora is DateTime dtAnterior)
    {
        dataHoraAnterior = DateTime.SpecifyKind(dtAnterior, DateTimeKind.Utc);
    }


    // ------------------------------------------------------------------------
    // CALCULA TEMPO DA FASE EM MINUTOS
    // ------------------------------------------------------------------------
    int? tempoFaseMinutos = null;

    if (dataHoraAnterior.HasValue)
    {
        tempoFaseMinutos =
        (int)(DateTime.UtcNow - dataHoraAnterior.Value).TotalMinutes;

if (tempoFaseMinutos < 0)
    tempoFaseMinutos = 0;

    }

    // ------------------------------------------------------------------------
    // CRIA HISTÓRICO
    // ------------------------------------------------------------------------
    var historico = new RaspHistoricoFluxoEntity
    {
        IdRasp = idRasp,
        IdUsuario = idUsuario,
        Acao = acao,
        StatusAnterior = statusAnterior,
        StatusNovo = statusNovo,
        Observacao = observacao,
        DataHora = DateTime.UtcNow,
        DataHoraAnterior = dataHoraAnterior,
        TempoFaseMinutos = tempoFaseMinutos,
        OrigemEvento = origemEvento,
        TipoComplemento = tipoComplemento
    };

    db.Set<RaspHistoricoFluxoEntity>().Add(historico);

    await db.SaveChangesAsync();
}


var app = builder.Build();

//Console.WriteLine(SenhaService.GerarHash("123456"));



app.UseCors("PermitirFront");

// -----------------------------------------------------------------------------
// 03. SWAGGER
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// -----------------------------------------------------------------------------
// 04. LOGIN DO SISTEMA
// -----------------------------------------------------------------------------
app.MapPost("/login", async (
    LoginRequest request,
    RaspDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Login))
        return Results.BadRequest("Login obrigatório.");

    if (string.IsNullOrWhiteSpace(request.Senha))
        return Results.BadRequest("Senha obrigatória.");

    var loginNormalizado = request.Login.Trim().ToLower();

    // -------------------------------------------------------------------------
    // 04.01 LOGIN GM
    // -------------------------------------------------------------------------
    var usuarioGm = await db.Usuarios
        .FirstOrDefaultAsync(u =>
            u.Ativo &&
            (
                u.Gmin.ToLower() == loginNormalizado ||
                (u.Nome + " " + (u.Sobrenome ?? "")).Trim().ToLower() == loginNormalizado
            ));

    if (usuarioGm is not null)
    {
        if (string.IsNullOrWhiteSpace(usuarioGm.SenhaHash))
            return Results.BadRequest("Usuário sem senha cadastrada.");

        var senhaValida = SenhaService.ValidarSenha(
            request.Senha,
            usuarioGm.SenhaHash
        );

        if (!senhaValida)
            return Results.BadRequest("Usuário ou senha inválidos.");

        usuarioGm.UltimoLogin = DateTime.UtcNow;

        await db.SaveChangesAsync();

        var perfil = await db.PerfilRasp
            .Where(p => p.IdPerfil == usuarioGm.IdPerfil)
            .Select(p => p.Nome)
            .FirstOrDefaultAsync();

        return Results.Ok(new LoginResponse
        {
            Sucesso = true,
            Mensagem = "Login realizado com sucesso.",
            TipoUsuario = "GM",
            IdUsuario = usuarioGm.IdUsuario,
            NomeCompleto = $"{usuarioGm.Nome} {usuarioGm.Sobrenome}".Trim(),
            Gmid = usuarioGm.Gmin,
            Perfil = perfil ?? "",
            Empresa = "GM",
            NomeEmpresa = "GM",
            Administrador = usuarioGm.Administrador,
            PrimeiroAcesso = usuarioGm.PrimeiroAcesso,
            TelaInicial = usuarioGm.Administrador ? "admin" : "dashboard"
        });
    }

    // -------------------------------------------------------------------------
    // 04.02 LOGIN TERCEIRO
    // -------------------------------------------------------------------------
    var usuarioTerceiro = await db.UsuariosTerceiros
        .FirstOrDefaultAsync(u =>
            u.Gmid.ToLower() == loginNormalizado &&
            u.Ativo);

    if (usuarioTerceiro is not null)
    {
        var senhaValida = SenhaService.ValidarSenha(
            request.Senha,
            usuarioTerceiro.SenhaHash
        );

        if (!senhaValida)
            return Results.BadRequest("Usuário ou senha inválidos.");

        var empresaTerceiro = await db.EmpresaSelecaoRasp
            .Where(e => e.IdEmpresaSelecao == usuarioTerceiro.IdEmpresaSelecao)
            .Select(e => e.NomeEmpresa)
            .FirstOrDefaultAsync();

        return Results.Ok(new LoginResponse
        {
            Sucesso = true,
            Mensagem = "Login realizado com sucesso.",
            TipoUsuario = "TERCEIRO",
            IdUsuario = usuarioTerceiro.IdUsuarioTerceiro,
            NomeCompleto = $"{usuarioTerceiro.Nome} {usuarioTerceiro.Sobrenome}".Trim(),
            Gmid = usuarioTerceiro.Gmid,
            Perfil = "TERCEIRO_SELECAO",
            Empresa = empresaTerceiro ?? "",
            NomeEmpresa = empresaTerceiro ?? "",
            Administrador = false,
            PrimeiroAcesso = usuarioTerceiro.PrimeiroAcesso,
            TelaInicial = "selecao-operacional"
        });
    }

    return Results.BadRequest("Usuário ou senha inválidos.");
})
.WithName("LoginSistema");




// -----------------------------------------------------------------------------
// 04A. TROCA DE SENHA
// -----------------------------------------------------------------------------

app.MapPost("/login/trocar-senha", async (
    TrocarSenhaRequest request,
    RaspDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Login))
        return Results.BadRequest("Login obrigatório.");

    if (string.IsNullOrWhiteSpace(request.SenhaAtual))
        return Results.BadRequest("Senha atual obrigatória.");

    if (string.IsNullOrWhiteSpace(request.NovaSenha))
        return Results.BadRequest("Nova senha obrigatória.");

    if (string.IsNullOrWhiteSpace(request.ConfirmarNovaSenha))
        return Results.BadRequest("Confirmação da nova senha obrigatória.");

    if (request.NovaSenha != request.ConfirmarNovaSenha)
        return Results.BadRequest("Nova senha e confirmação não conferem.");

    if (request.NovaSenha.Length < 8)
        return Results.BadRequest("A nova senha deve ter no mínimo 8 caracteres.");

    if (!request.NovaSenha.Any(char.IsLetter))
        return Results.BadRequest("A nova senha deve conter pelo menos uma letra.");

    if (!request.NovaSenha.Any(char.IsDigit))
        return Results.BadRequest("A nova senha deve conter pelo menos um número.");

    var loginNormalizado = request.Login.Trim().ToLower();

    var usuarioGm = await db.Usuarios
    .FirstOrDefaultAsync(u =>
        u.Ativo &&
        (
            u.Gmin.ToLower() == loginNormalizado ||
            (u.Nome + " " + (u.Sobrenome ?? "")).Trim().ToLower() == loginNormalizado
        ));


    if (usuarioGm is not null)
    {
        if (string.IsNullOrWhiteSpace(usuarioGm.SenhaHash))
            return Results.BadRequest("Usuário sem senha cadastrada.");

        var senhaAtualValida = SenhaService.ValidarSenha(
            request.SenhaAtual,
            usuarioGm.SenhaHash
        );

        if (!senhaAtualValida)
            return Results.BadRequest("Senha atual inválida.");

        usuarioGm.SenhaHash = SenhaService.GerarHash(request.NovaSenha);
        usuarioGm.PrimeiroAcesso = false;
        usuarioGm.UltimoLogin = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            sucesso = true,
            mensagem = "Senha alterada com sucesso.",
            tipoUsuario = "GM",
            primeiroAcesso = usuarioGm.PrimeiroAcesso
        });
    }

    var usuarioTerceiro = await db.UsuariosTerceiros
        .FirstOrDefaultAsync(u =>
            u.Gmid.ToLower() == loginNormalizado &&
            u.Ativo);

    if (usuarioTerceiro is not null)
    {
        var senhaAtualValida = SenhaService.ValidarSenha(
            request.SenhaAtual,
            usuarioTerceiro.SenhaHash
        );

        if (!senhaAtualValida)
            return Results.BadRequest("Senha atual inválida.");

        usuarioTerceiro.SenhaHash = SenhaService.GerarHash(request.NovaSenha);
        usuarioTerceiro.PrimeiroAcesso = false;

        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            sucesso = true,
            mensagem = "Senha alterada com sucesso.",
            tipoUsuario = "TERCEIRO",
            primeiroAcesso = usuarioTerceiro.PrimeiroAcesso
        });
    }

    return Results.BadRequest("Usuário não encontrado.");
})
.WithName("TrocarSenhaLogin");

// -----------------------------------------------------------------------------
// 04. RASP ANOTACAO
// -----------------------------------------------------------------------------

app.MapGet("/rasp-anotacao", async (RaspDbContext db) =>
{
    var itens = await db.RaspAnotacao
        .OrderBy(a => a.IdRaspAnotacao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspAnotacao");

app.MapGet("/rasp-anotacao/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.RaspAnotacao
        .FirstOrDefaultAsync(a => a.IdRaspAnotacao == id);

    return item is null
        ? Results.NotFound($"Anotação com id {id} não encontrada.")
        : Results.Ok(item);
})
.WithName("ObterRaspAnotacaoPorId");

app.MapGet("/rasp/{id:int}/anotacoes", async (int id, RaspDbContext db) =>
{
    var raspExiste = await db.Rasp.AnyAsync(r => r.IdRasp == id);

    if (!raspExiste)
        return Results.NotFound($"RASP com id {id} não encontrado.");

    var itens = await db.RaspAnotacao
        .Where(a => a.IdRasp == id)
        .OrderBy(a => a.DataHora)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarAnotacoesPorRasp");

app.MapPost("/rasp-anotacao", async (CriarRaspAnotacaoRequest req, RaspDbContext db) =>
{
    if (req.IdRasp <= 0)
        return Results.BadRequest("IdRasp é obrigatório.");

    if (req.IdUsuario <= 0)
        return Results.BadRequest("IdUsuario é obrigatório.");

    if (req.IdPerfilRasp <= 0)
        return Results.BadRequest("IdPerfilRasp é obrigatório.");

    if (string.IsNullOrWhiteSpace(req.TextoAnotacao))
        return Results.BadRequest("TextoAnotacao é obrigatório.");

    var raspExiste = await db.Rasp.AnyAsync(r => r.IdRasp == req.IdRasp);
    if (!raspExiste)
        return Results.BadRequest($"RASP com id {req.IdRasp} não encontrado.");

    var usuarioExiste = await db.Usuarios.AnyAsync(u => u.IdUsuario == req.IdUsuario);
    if (!usuarioExiste)
        return Results.BadRequest($"Usuário com id {req.IdUsuario} não encontrado.");

    var perfilExiste = await db.PerfilRasp.AnyAsync(p => p.IdPerfil == req.IdPerfilRasp);
    if (!perfilExiste)
        return Results.BadRequest($"Perfil com id {req.IdPerfilRasp} não encontrado.");

    var anotacao = new RaspAnotacao
    {
        IdRasp = req.IdRasp,
        IdUsuario = req.IdUsuario,
        IdPerfilRasp = req.IdPerfilRasp,
        DataHora = DateTime.UtcNow,
        TipoAnotacao = string.IsNullOrWhiteSpace(req.TipoAnotacao)
            ? "Complemento"
            : req.TipoAnotacao.Trim(),
        TextoAnotacao = req.TextoAnotacao.Trim()
    };

    db.RaspAnotacao.Add(anotacao);
    await db.SaveChangesAsync();

    return Results.Created($"/rasp-anotacao/{anotacao.IdRaspAnotacao}", anotacao);
})
.WithName("CriarRaspAnotacao");

app.MapPut("/rasp-anotacao/{id:int}", async (int id, AtualizarRaspAnotacaoRequest req, RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id da anotação inválido.");

    var item = await db.RaspAnotacao
        .FirstOrDefaultAsync(a => a.IdRaspAnotacao == id);

    if (item is null)
        return Results.NotFound($"Anotação com id {id} não encontrada.");

    if (string.IsNullOrWhiteSpace(req.TextoAnotacao))
        return Results.BadRequest("TextoAnotacao é obrigatório.");

    item.TipoAnotacao = string.IsNullOrWhiteSpace(req.TipoAnotacao)
        ? "Complemento"
        : req.TipoAnotacao.Trim();

    item.TextoAnotacao = req.TextoAnotacao.Trim();

    await db.SaveChangesAsync();

    return Results.Ok(item);
})
.WithName("AtualizarRaspAnotacao");

app.MapDelete("/rasp-anotacao/{id:int}", async (int id, RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id da anotação inválido.");

    var item = await db.RaspAnotacao
        .FirstOrDefaultAsync(a => a.IdRaspAnotacao == id);

    if (item is null)
        return Results.NotFound($"Anotação com id {id} não encontrada.");

    db.RaspAnotacao.Remove(item);
    await db.SaveChangesAsync();

    return Results.Ok($"Anotação com id {id} removida com sucesso.");
})
.WithName("ExcluirRaspAnotacao");




// -----------------------------------------------------------------------------
// 05. STATUS RASP
// -----------------------------------------------------------------------------

app.MapGet("/status-rasp", async (RaspDbContext db) =>
{
    var itens = await db.StatusRasp
        .OrderBy(s => s.OrdemFluxo)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarStatusRasp");

app.MapGet("/status-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.StatusRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Status não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterStatusRaspPorId");

// -----------------------------------------------------------------------------
// 06. CONTA CR RASP
// -----------------------------------------------------------------------------

app.MapGet("/conta-cr-rasp", async (RaspDbContext db) =>
{
    var itens = await db.ContaCrRasp.ToListAsync();
    return Results.Ok(itens);
});

app.MapGet("/conta-cr-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.ContaCrRasp.FindAsync(id);

    return item is not null
        ? Results.Ok(item)
        : Results.NotFound("Conta CR RASP não encontrada.");
});

// -----------------------------------------------------------------------------
// 07. PN RASP
// -----------------------------------------------------------------------------

app.MapGet("/pn-rasp", async (RaspDbContext db) =>
{
    var itens = await db.PnRasp
        .OrderBy(p => p.CodigoPn)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarPnRasp");

app.MapGet("/pn-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.PnRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("PN não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterPnRaspPorId");

app.MapGet("/pn-rasp/codigo/{codigoPn}", async (string codigoPn, RaspDbContext db) =>
{
    var codigoLimpo = (codigoPn ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(codigoLimpo))
        return Results.BadRequest("CodigoPn é obrigatório.");

    if (codigoLimpo.Length != 8)
        return Results.BadRequest("CodigoPn deve ter exatamente 8 caracteres.");

    if (!codigoLimpo.All(char.IsDigit))
        return Results.BadRequest("CodigoPn deve conter somente números.");

    var item = await db.PnRasp
        .FirstOrDefaultAsync(p => p.CodigoPn == codigoLimpo);

    return item is null
        ? Results.NotFound("PN não encontrado para o código informado.")
        : Results.Ok(item);
})
.WithName("ObterPnRaspPorCodigo");

app.MapPost("/pn-rasp", async (CriarPnRaspRequest req, RaspDbContext db) =>
{
    var codigoPn = (req.CodigoPn ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(codigoPn))
        return Results.BadRequest("CodigoPn é obrigatório.");

    if (codigoPn.Length != 8)
        return Results.BadRequest("CodigoPn deve ter exatamente 8 caracteres.");

    if (!codigoPn.All(char.IsDigit))
        return Results.BadRequest("CodigoPn deve conter somente números.");

    var nomePeca = (req.NomePeca ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(nomePeca))
        return Results.BadRequest("NomePeca é obrigatório.");

    var codigoJaExiste = await db.PnRasp.AnyAsync(p => p.CodigoPn == codigoPn);

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

// -----------------------------------------------------------------------------
// 08. FORNECEDOR RASP
// -----------------------------------------------------------------------------

app.MapGet("/fornecedor-rasp", async (RaspDbContext db) =>
{
    var itens = await db.FornecedorRasp
        .OrderBy(f => f.Nome)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarFornecedorRasp");

app.MapGet("/fornecedor-rasp/duns/{duns}", async (string duns, RaspDbContext db) =>
{
    var dunsLimpo = (duns ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(dunsLimpo))
        return Results.BadRequest("Duns é obrigatório.");

    if (dunsLimpo.Length != 9)
        return Results.BadRequest("Duns deve ter exatamente 9 caracteres.");

    if (!dunsLimpo.All(char.IsDigit))
        return Results.BadRequest("Duns deve conter somente números.");

    var item = await db.FornecedorRasp
        .FirstOrDefaultAsync(f => f.Duns == dunsLimpo);

    return item is null
        ? Results.NotFound("Fornecedor não encontrado para o DUNS informado.")
        : Results.Ok(item);
})
.WithName("ObterFornecedorRaspPorDuns");

app.MapGet("/fornecedor-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.FornecedorRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Fornecedor não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterFornecedorRaspPorId");

app.MapPost("/fornecedor-rasp", async (CriarFornecedorRaspRequest req, RaspDbContext db) =>
{
    var duns = (req.Duns ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(duns))
        return Results.BadRequest("Duns é obrigatório.");

    if (duns.Length != 9)
        return Results.BadRequest("Duns deve ter exatamente 9 caracteres.");

    if (!duns.All(char.IsDigit))
        return Results.BadRequest("Duns deve conter somente números.");

    var nome = (req.Nome ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(nome))
        return Results.BadRequest("Nome é obrigatório.");

    var tipoFornecedor = (req.TipoFornecedor ?? string.Empty).Trim().ToUpperInvariant();

    if (string.IsNullOrWhiteSpace(tipoFornecedor))
        return Results.BadRequest("TipoFornecedor é obrigatório.");

    if (tipoFornecedor != "LOCAL" && tipoFornecedor != "IMPORTADO")
        return Results.BadRequest("TipoFornecedor deve ser LOCAL ou IMPORTADO.");

    var dunsJaExiste = await db.FornecedorRasp.AnyAsync(f => f.Duns == duns);

    if (dunsJaExiste)
        return Results.BadRequest("Já existe fornecedor cadastrado com esse DUNS.");

    var item = new FornecedorRasp
    {
        Duns = duns,
        Nome = nome,
        TipoFornecedor = tipoFornecedor,
        Ativo = req.Ativo
    };

    db.FornecedorRasp.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/fornecedor-rasp/{item.IdFornecedor}", item);
})
.WithName("CriarFornecedorRasp");

// -----------------------------------------------------------------------------
// 09. PERFIL RASP
// -----------------------------------------------------------------------------

app.MapGet("/perfil-rasp", async (RaspDbContext db) =>
{
    var itens = await db.PerfilRasp
        .OrderBy(p => p.Nome)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarPerfilRasp");

app.MapGet("/perfil-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.PerfilRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Perfil não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterPerfilRaspPorId");

// -----------------------------------------------------------------------------
// 10. DOMÍNIOS AUXILIARES DO RASP
// -----------------------------------------------------------------------------

app.MapGet("/modelo-veiculo-rasp", async (RaspDbContext db) =>
{
    var itens = await db.ModeloVeiculoRasp
        .OrderBy(m => m.OrdemExibicao)
        .ThenBy(m => m.NomeModelo)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarModeloVeiculoRasp");

app.MapGet("/modelo-veiculo-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.ModeloVeiculoRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Modelo de veículo não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterModeloVeiculoRaspPorId");

app.MapGet("/setor-rasp", async (RaspDbContext db) =>
{
    var itens = await db.SetorRasp
        .OrderBy(s => s.OrdemExibicao)
        .ThenBy(s => s.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarSetorRasp");

app.MapGet("/setor-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.SetorRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Setor não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterSetorRaspPorId");

app.MapGet("/turno-rasp", async (RaspDbContext db) =>
{
    var itens = await db.TurnoRasp
        .OrderBy(t => t.OrdemExibicao)
        .ThenBy(t => t.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarTurnoRasp");

app.MapGet("/turno-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.TurnoRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Turno não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterTurnoRaspPorId");

app.MapGet("/origem-fabricacao-rasp", async (RaspDbContext db) =>
{
    var itens = await db.OrigemFabricacaoRasp
        .OrderBy(o => o.OrdemExibicao)
        .ThenBy(o => o.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarOrigemFabricacaoRasp");

app.MapGet("/origem-fabricacao-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.OrigemFabricacaoRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Origem de fabricação não encontrada.")
        : Results.Ok(item);
})
.WithName("ObterOrigemFabricacaoRaspPorId");

app.MapGet("/piloto-rasp", async (RaspDbContext db) =>
{
    var itens = await db.PilotoRasp
        .OrderBy(p => p.OrdemExibicao)
        .ThenBy(p => p.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarPilotoRasp");

app.MapGet("/piloto-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.PilotoRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Piloto não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterPilotoRaspPorId");

app.MapGet("/impacto-cliente-rasp", async (RaspDbContext db) =>
{
    var itens = await db.ImpactoClienteRasp
        .OrderBy(i => i.OrdemExibicao)
        .ThenBy(i => i.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarImpactoClienteRasp");

app.MapGet("/impacto-cliente-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.ImpactoClienteRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Impacto cliente não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterImpactoClienteRaspPorId");

app.MapGet("/impacto-qualidade-rasp", async (RaspDbContext db) =>
{
    var itens = await db.ImpactoQualidadeRasp
        .OrderBy(i => i.OrdemExibicao)
        .ThenBy(i => i.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarImpactoQualidadeRasp");

app.MapGet("/impacto-qualidade-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.ImpactoQualidadeRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Impacto qualidade não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterImpactoQualidadeRaspPorId");

app.MapGet("/maior-impacto-rasp", async (RaspDbContext db) =>
{
    var itens = await db.MaiorImpactoRasp
        .OrderBy(i => i.OrdemExibicao)
        .ThenBy(i => i.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarMaiorImpactoRasp");

app.MapGet("/maior-impacto-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.MaiorImpactoRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Maior impacto não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterMaiorImpactoRaspPorId");

app.MapGet("/major-rasp", async (RaspDbContext db) =>
{
    var itens = await db.MajorRasp
        .OrderBy(i => i.OrdemExibicao)
        .ThenBy(i => i.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarMajorRasp");

app.MapGet("/major-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.MajorRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Major não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterMajorRaspPorId");

app.MapGet("/empresa-selecao-rasp", async (RaspDbContext db) =>
{
    var itens = await db.EmpresaSelecaoRasp
        .OrderBy(e => e.NomeEmpresa)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarEmpresaSelecaoRasp");

app.MapGet("/empresa-selecao-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.EmpresaSelecaoRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Empresa de seleção não encontrada.")
        : Results.Ok(item);
})
.WithName("ObterEmpresaSelecaoRaspPorId");

app.MapGet("/conta-cr-subconta-rasp", async (RaspDbContext db) =>
{
    var itens = await db.ContaCrSubcontaRasp
        .OrderBy(c => c.CodigoSubconta)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarContaCrSubcontaRasp");

app.MapGet("/conta-cr-subconta-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.ContaCrSubcontaRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Subconta CR não encontrada.")
        : Results.Ok(item);
})
.WithName("ObterContaCrSubcontaRaspPorId");

app.MapGet("/gm-aliado-rasp", async (RaspDbContext db) =>
{
    var itens = await db.GmAliadoRasp
        .OrderBy(g => g.OrdemExibicao)
        .ThenBy(g => g.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarGmAliadoRasp");

app.MapGet("/gm-aliado-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.GmAliadoRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("GM aliado não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterGmAliadoRaspPorId");

app.MapGet("/spps-classificacao-rasp", async (RaspDbContext db) =>
{
    var itens = await db.SppsClassificacaoRasp
        .OrderBy(s => s.OrdemExibicao)
        .ThenBy(s => s.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarSppsClassificacaoRasp");

app.MapGet("/spps-classificacao-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.SppsClassificacaoRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Classificação SPPS não encontrada.")
        : Results.Ok(item);
})
.WithName("ObterSppsClassificacaoRaspPorId");

app.MapGet("/spps-status-rasp", async (RaspDbContext db) =>
{
    var itens = await db.SppsStatusRasp
        .OrderBy(s => s.OrdemExibicao)
        .ThenBy(s => s.NomeStatus)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarSppsStatusRasp");

app.MapGet("/spps-status-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.SppsStatusRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Status SPPS não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterSppsStatusRaspPorId");

app.MapGet("/indice-operacional-rasp", async (RaspDbContext db) =>
{
    var itens = await db.IndiceOperacionalRasp
        .OrderBy(i => i.CodigoOpcao)
        .ThenBy(i => i.Descricao)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarIndiceOperacionalRasp");

app.MapGet("/indice-operacional-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.IndiceOperacionalRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Índice operacional não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterIndiceOperacionalRaspPorId");

// -----------------------------------------------------------------------------
// 11. USUÁRIOS
// CORREÇÃO IMPORTANTE:
// Este endpoint agora projeta manualmente e retorna IdTurnoRasp,
// necessário para o front localizar o FT por turno.
// -----------------------------------------------------------------------------

app.MapGet("/usuarios", async (RaspDbContext db) =>
{
    var itens = await db.Usuarios
        .OrderBy(u => u.Nome)
        .Select(u => new
        {
            u.IdUsuario,
            u.Nome,
            u.Gmin,
            u.Email,
            u.Cargo,
            u.Ativo,
            u.IdPerfil,
            u.IdTurnoRasp
        })
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarUsuarios");

app.MapGet("/usuarios/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.Usuarios
        .Where(u => u.IdUsuario == id)
        .Select(u => new
        {
            u.IdUsuario,
            u.Nome,
            u.Gmin,
            u.Email,
            u.Cargo,
            u.Ativo,
            u.IdPerfil,
            u.IdTurnoRasp
        })
        .FirstOrDefaultAsync();

    return item is null
        ? Results.NotFound("Usuário não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterUsuarioPorId");

// -----------------------------------------------------------------------------
// 11A. ADMIN - USUÁRIOS GM
// -----------------------------------------------------------------------------

app.MapGet("/admin/usuarios-gm", async (RaspDbContext db) =>
{
    var itens = await (
        from u in db.Usuarios.AsNoTracking()
        join pJoin in db.PerfilRasp.AsNoTracking()
            on u.IdPerfil equals pJoin.IdPerfil into perfilLeft
        from p in perfilLeft.DefaultIfEmpty()
        join tJoin in db.TurnoRasp.AsNoTracking()
            on u.IdTurnoRasp equals tJoin.IdTurnoRasp into turnoLeft
        from t in turnoLeft.DefaultIfEmpty()
        orderby u.Nome, u.Sobrenome
        select new
        {
            u.IdUsuario,
            u.Nome,
            u.Sobrenome,
            NomeCompleto = (u.Nome + " " + (u.Sobrenome ?? "")).Trim(),
            u.Gmin,
            u.Email,
            u.Cargo,
            u.Ativo,
            u.IdPerfil,
            Perfil = p != null ? p.Nome : null,
            u.IdTurnoRasp,
            Turno = t != null ? t.Descricao : null,
            u.PrimeiroAcesso,
            u.Administrador,
            u.DataCriacao,
            u.UltimoLogin
        }
    ).ToListAsync();

    return Results.Ok(itens);
})
.WithName("AdminListarUsuariosGm")
.WithTags("Admin - Usuários");

app.MapPost("/admin/usuarios-gm", async (
    CriarUsuarioGmAdminRequest req,
    RaspDbContext db) =>
{
    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor é obrigatório.");

    var executor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor && u.Ativo);

    if (executor is null)
        return Results.BadRequest("Usuário executor não encontrado ou inativo.");

    if (!executor.Administrador && executor.IdPerfil != 1)
        return Results.StatusCode(StatusCodes.Status403Forbidden);

    var nome = (req.Nome ?? string.Empty).Trim();
    var sobrenome = (req.Sobrenome ?? string.Empty).Trim();
    var gmin = (req.Gmin ?? string.Empty).Trim().ToLower();
    var email = (req.Email ?? string.Empty).Trim().ToLower();
    var cargo = (req.Cargo ?? string.Empty).Trim();
    var senhaInicial = (req.SenhaInicial ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(nome))
        return Results.BadRequest("Nome é obrigatório.");

    if (string.IsNullOrWhiteSpace(sobrenome))
        return Results.BadRequest("Sobrenome é obrigatório.");

    if (string.IsNullOrWhiteSpace(gmin))
        return Results.BadRequest("GMIN é obrigatório.");

    if (string.IsNullOrWhiteSpace(email))
        return Results.BadRequest("Email é obrigatório.");

    if (string.IsNullOrWhiteSpace(cargo))
        return Results.BadRequest("Cargo é obrigatório.");

    if (req.IdPerfil <= 0)
        return Results.BadRequest("Perfil é obrigatório.");

    if (string.IsNullOrWhiteSpace(senhaInicial))
        return Results.BadRequest("Senha inicial é obrigatória.");

    if (senhaInicial.Length < 8)
        return Results.BadRequest("A senha inicial deve ter no mínimo 8 caracteres.");

    var perfilExiste = await db.PerfilRasp
        .AnyAsync(p => p.IdPerfil == req.IdPerfil);

    if (!perfilExiste)
        return Results.BadRequest("Perfil informado não existe.");

    if (req.IdTurnoRasp.HasValue && req.IdTurnoRasp.Value > 0)
    {
        var turnoExiste = await db.TurnoRasp
            .AnyAsync(t => t.IdTurnoRasp == req.IdTurnoRasp.Value);

        if (!turnoExiste)
            return Results.BadRequest("Turno informado não existe.");
    }

    var gminExiste = await db.Usuarios
        .AnyAsync(u => u.Gmin.ToLower() == gmin);

    if (gminExiste)
        return Results.BadRequest("Já existe usuário GM cadastrado com esse GMIN.");

    var emailExiste = await db.Usuarios
        .AnyAsync(u => u.Email.ToLower() == email);

    if (emailExiste)
        return Results.BadRequest("Já existe usuário GM cadastrado com esse email.");

    var usuario = new Usuario
    {
        Nome = nome,
        Sobrenome = sobrenome,
        Gmin = gmin,
        Email = email,
        Cargo = cargo,
        Ativo = true,
        SenhaHash = SenhaService.GerarHash(senhaInicial),
        IdPerfil = req.IdPerfil,
        IdTurnoRasp = req.IdTurnoRasp.HasValue && req.IdTurnoRasp.Value > 0
            ? req.IdTurnoRasp.Value
            : null,
        PrimeiroAcesso = true,
        Administrador = req.Administrador,
        DataCriacao = DateTime.UtcNow
    };

    db.Usuarios.Add(usuario);
    await db.SaveChangesAsync();

    return Results.Created($"/admin/usuarios-gm/{usuario.IdUsuario}", new
    {
        mensagem = "Usuário GM cadastrado com sucesso.",
        usuario.IdUsuario,
        usuario.Nome,
        usuario.Sobrenome,
        usuario.Gmin,
        usuario.Email,
        usuario.IdPerfil,
        usuario.IdTurnoRasp,
        usuario.Administrador,
        usuario.PrimeiroAcesso,
        usuario.Ativo
    });
})
.WithName("AdminCriarUsuarioGm")
.WithTags("Admin - Usuários");

app.MapPut("/admin/usuarios-gm/{id:int}", async (
    int id,
    AtualizarUsuarioGmAdminRequest req,
    RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do usuário inválido.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor é obrigatório.");

    var executor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor && u.Ativo);

    if (executor is null)
        return Results.BadRequest("Usuário executor não encontrado ou inativo.");

    if (!executor.Administrador && executor.IdPerfil != 1)
        return Results.StatusCode(StatusCodes.Status403Forbidden);

    var usuario = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == id);

    if (usuario is null)
        return Results.NotFound("Usuário GM não encontrado.");

    if (req.Nome is not null)
    {
        var nome = req.Nome.Trim();
        if (string.IsNullOrWhiteSpace(nome))
            return Results.BadRequest("Nome não pode ficar vazio.");

        usuario.Nome = nome;
    }

    if (req.Sobrenome is not null)
    {
        var sobrenome = req.Sobrenome.Trim();
        if (string.IsNullOrWhiteSpace(sobrenome))
            return Results.BadRequest("Sobrenome não pode ficar vazio.");

        usuario.Sobrenome = sobrenome;
    }

    
    if (req.Email is not null)
    {
        var email = req.Email.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(email))
            return Results.BadRequest("Email não pode ficar vazio.");

        var emailExiste = await db.Usuarios
            .AnyAsync(u => u.IdUsuario != id && u.Email.ToLower() == email);

        if (emailExiste)
            return Results.BadRequest("Já existe outro usuário GM com esse email.");

        usuario.Email = email;
    }

    if (req.Cargo is not null)
    {
        var cargo = req.Cargo.Trim();

        if (string.IsNullOrWhiteSpace(cargo))
            return Results.BadRequest("Cargo não pode ficar vazio.");

        usuario.Cargo = cargo;
    }

    if (req.IdPerfil.HasValue)
    {
        if (req.IdPerfil.Value <= 0)
            return Results.BadRequest("Perfil inválido.");

        var perfilExiste = await db.PerfilRasp
            .AnyAsync(p => p.IdPerfil == req.IdPerfil.Value);

        if (!perfilExiste)
            return Results.BadRequest("Perfil informado não existe.");

        usuario.IdPerfil = req.IdPerfil.Value;
    }

    if (req.IdTurnoRasp.HasValue)
    {
        if (req.IdTurnoRasp.Value == 0)
        {
            usuario.IdTurnoRasp = null;
        }
        else
        {
            var turnoExiste = await db.TurnoRasp
                .AnyAsync(t => t.IdTurnoRasp == req.IdTurnoRasp.Value);

            if (!turnoExiste)
                return Results.BadRequest("Turno informado não existe.");

            usuario.IdTurnoRasp = req.IdTurnoRasp.Value;
        }
    }

    if (req.Ativo.HasValue)
        usuario.Ativo = req.Ativo.Value;

    if (req.Administrador.HasValue)
        usuario.Administrador = req.Administrador.Value;

    if (!string.IsNullOrWhiteSpace(req.NovaSenha))
    {
        var novaSenha = req.NovaSenha.Trim();

        if (novaSenha.Length < 8)
            return Results.BadRequest("A nova senha deve ter no mínimo 8 caracteres.");

        usuario.SenhaHash = SenhaService.GerarHash(novaSenha);
        usuario.PrimeiroAcesso = true;
    }

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        mensagem = "Usuário GM atualizado com sucesso.",
        usuario.IdUsuario,
        usuario.Nome,
        usuario.Sobrenome,
        usuario.Gmin,
        usuario.Email,
        usuario.Cargo,
        usuario.Ativo,
        usuario.IdPerfil,
        usuario.IdTurnoRasp,
        usuario.Administrador,
        usuario.PrimeiroAcesso
    });
})
.WithName("AdminAtualizarUsuarioGm")
.WithTags("Admin - Usuários");

app.MapGet("/admin/usuarios-terceiros", async (RaspDbContext db) =>
{
    var itens = await (
        from u in db.UsuariosTerceiros.AsNoTracking()
        join e in db.EmpresaSelecaoRasp.AsNoTracking()
            on u.IdEmpresaSelecao equals e.IdEmpresaSelecao
        orderby u.Nome, u.Sobrenome
        select new
        {
            u.IdUsuarioTerceiro,
            u.Nome,
            u.Sobrenome,
            NomeCompleto = (u.Nome + " " + u.Sobrenome).Trim(),
            u.Gmid,
            u.Ativo,
            u.PrimeiroAcesso,
            u.DataCriacao,
            u.IdEmpresaSelecao,
            Empresa = e.NomeEmpresa,
            TipoEmpresa = e.TipoEmpresa
        }
    ).ToListAsync();

    return Results.Ok(itens);
})
.WithName("AdminListarUsuariosTerceiros")
.WithTags("Admin - Usuários");


app.MapPut("/admin/usuarios-terceiros/{id:int}", async (
    int id,
    AtualizarUsuarioTerceiroAdminRequest req,
    RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do usuário terceiro inválido.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor é obrigatório.");

    var executor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor && u.Ativo);

    if (executor is null)
        return Results.BadRequest("Usuário executor não encontrado ou inativo.");

    if (!executor.Administrador && executor.IdPerfil != 1)
        return Results.StatusCode(StatusCodes.Status403Forbidden);

    var usuario = await db.UsuariosTerceiros
        .FirstOrDefaultAsync(u => u.IdUsuarioTerceiro == id);

    if (usuario is null)
        return Results.NotFound("Usuário terceiro não encontrado.");

    if (req.Nome is not null)
    {
        var nome = req.Nome.Trim();

        if (string.IsNullOrWhiteSpace(nome))
            return Results.BadRequest("Nome não pode ficar vazio.");

        usuario.Nome = nome;
    }

    if (req.Sobrenome is not null)
    {
        var sobrenome = req.Sobrenome.Trim();

        if (string.IsNullOrWhiteSpace(sobrenome))
            return Results.BadRequest("Sobrenome não pode ficar vazio.");

        usuario.Sobrenome = sobrenome;
    }

        if (req.IdEmpresaSelecao.HasValue)
    {
        if (req.IdEmpresaSelecao.Value <= 0)
            return Results.BadRequest("Empresa de seleção inválida.");

        var empresaExiste = await db.EmpresaSelecaoRasp
            .AnyAsync(e => e.IdEmpresaSelecao == req.IdEmpresaSelecao.Value && e.Ativo);

        if (!empresaExiste)
            return Results.BadRequest("Empresa de seleção não encontrada ou inativa.");

        usuario.IdEmpresaSelecao = req.IdEmpresaSelecao.Value;
    }

    if (req.Ativo.HasValue)
        usuario.Ativo = req.Ativo.Value;

    if (!string.IsNullOrWhiteSpace(req.NovaSenha))
    {
        var novaSenha = req.NovaSenha.Trim();

        if (novaSenha.Length < 8)
            return Results.BadRequest("A nova senha deve ter no mínimo 8 caracteres.");

        usuario.SenhaHash = SenhaService.GerarHash(novaSenha);
        usuario.PrimeiroAcesso = true;
    }

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        mensagem = "Usuário terceiro atualizado com sucesso.",
        usuario.IdUsuarioTerceiro,
        usuario.Nome,
        usuario.Sobrenome,
        usuario.Gmid,
        usuario.IdEmpresaSelecao,
        usuario.Ativo,
        usuario.PrimeiroAcesso
    });
})
.WithName("AdminAtualizarUsuarioTerceiro")
.WithTags("Admin - Usuários");


// -----------------------------------------------------------------------------
// 12. RASP
// -----------------------------------------------------------------------------

app.MapGet("/rasp", async (RaspDbContext db) =>
{
    var itens = await db.Rasp
        .OrderByDescending(r => r.IdRasp)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRasp");

app.MapGet("/rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.Rasp.FindAsync(id);

    return item is null
        ? Results.NotFound("RASP não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterRaspPorId");

app.MapPost("/rasp", async (CriarRaspRequest req, RaspDbContext db, IConfiguration config) =>
{
    if (req.IdFornecedorRasp <= 0)
        return Results.BadRequest("IdFornecedorRasp inválido.");

    if (req.IdUsuarioCriador <= 0)
        return Results.BadRequest("IdUsuarioCriador inválido.");

    var descricaoProblema = (req.DescricaoProblema ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(descricaoProblema))
        return Results.BadRequest("DescricaoProblema é obrigatória.");

    var fornecedorExiste = await db.FornecedorRasp
        .AnyAsync(f => f.IdFornecedor == req.IdFornecedorRasp);

    if (!fornecedorExiste)
        return Results.BadRequest("Fornecedor informado não existe.");

    var usuarioCriador = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioCriador);

    if (usuarioCriador is null)
        return Results.BadRequest("Usuário criador não existe.");

    if (!usuarioCriador.Ativo)
        return Results.BadRequest("Usuário criador está inativo.");

    var connStr = config.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connStr))
        return Results.Problem("Connection string 'DefaultConnection' não encontrada.");

    await using var conn = new NpgsqlConnection(connStr);
    await conn.OpenAsync();

    await using var tx = await conn.BeginTransactionAsync();

    var agora = DateTime.UtcNow;
    var dataCriacao = DateOnly.FromDateTime(agora);
    var horaCriacao = TimeOnly.FromDateTime(agora);

    var insertSql = """
        INSERT INTO rasp
            (numero_rasp, data_criacao, hora_criacao, id_fornecedor_rasp, descricao_problema, id_status_rasp, is_rascunho, percentual_completude, id_analista_mt)
        VALUES
            ('TEMP', @data_criacao, @hora_criacao, @id_fornecedor, @descricao, 1, true, 0, @id_usuario_criador)
        RETURNING id_rasp;
        """;

    int idRasp;

    await using (var cmd = new NpgsqlCommand(insertSql, conn, tx))
    {
        cmd.Parameters.AddWithValue("data_criacao", dataCriacao);
        cmd.Parameters.AddWithValue("hora_criacao", horaCriacao);
        cmd.Parameters.AddWithValue("id_fornecedor", req.IdFornecedorRasp);
        cmd.Parameters.AddWithValue("descricao", descricaoProblema);
        cmd.Parameters.AddWithValue("id_usuario_criador", req.IdUsuarioCriador);

        idRasp = (int)(await cmd.ExecuteScalarAsync() ?? 0);
    }

    if (idRasp <= 0)
    {
        await tx.RollbackAsync();
        return Results.Problem("Falha ao gerar id_rasp.");
    }

    var numeroRasp = idRasp.ToString("D4") + "/" + DateTime.Today.ToString("yy");

    var updateSql = """
        UPDATE rasp
        SET numero_rasp = @numero_rasp
        WHERE id_rasp = @id;
        """;

    await using (var cmd2 = new NpgsqlCommand(updateSql, conn, tx))
    {
        cmd2.Parameters.AddWithValue("id", idRasp);
        cmd2.Parameters.AddWithValue("numero_rasp", numeroRasp);
        await cmd2.ExecuteNonQueryAsync();
    }

    await tx.CommitAsync();

    return Results.Created($"/rasp/{idRasp}", new
    {
        id_rasp = idRasp,
        numero_rasp = numeroRasp,
        mensagem = "RASP criado com sucesso.",
        data_criacao = dataCriacao,
        hora_criacao = horaCriacao
    });
})
.WithName("CriarRasp");

   // -----------------------------------------------------------------------------
// 13. BP DO RASP
// -----------------------------------------------------------------------------

app.MapPost("/rasp-bp", async (RaspDbContext db, RaspBp bp) =>
{
    if (bp.DataBp == default || bp.HoraBp == default)
        return Results.BadRequest("Data e hora do BP são obrigatórias.");

    if (string.IsNullOrWhiteSpace(bp.TipoReferenciaBp))
        return Results.BadRequest("Tipo de referência é obrigatório.");

    if (string.IsNullOrWhiteSpace(bp.ComoIdentificado))
        return Results.BadRequest("Campo 'Como identificado' é obrigatório.");

    var tipo = bp.TipoReferenciaBp.Trim().ToUpperInvariant();

    if (tipo != "VIN" && tipo != "LOCAL")
        return Results.BadRequest("Tipo de referência deve ser VIN ou LOCAL.");

    if (tipo == "VIN")
    {
        if (string.IsNullOrWhiteSpace(bp.Vin))
            return Results.BadRequest("VIN é obrigatório quando o tipo for VIN.");

        bp.Vin = bp.Vin.Trim().ToUpperInvariant();

        if (!System.Text.RegularExpressions.Regex.IsMatch(bp.Vin, "^[A-HJ-NPR-Z0-9]{17}$"))
            return Results.BadRequest("VIN inválido. Deve conter 17 caracteres alfanuméricos válidos.");

        bp.LocalCelula = null;
    }

    if (tipo == "LOCAL")
    {
        if (string.IsNullOrWhiteSpace(bp.LocalCelula))
            return Results.BadRequest("Local/Célula é obrigatório quando o tipo for LOCAL.");

        bp.LocalCelula = bp.LocalCelula.Trim();
        bp.Vin = null;
    }

    bp.TipoReferenciaBp = tipo;
    bp.CriadoEm = DateTime.UtcNow;
    bp.AtualizadoEm = DateTime.UtcNow;

    db.RaspBp.Add(bp);
    await db.SaveChangesAsync();

    return Results.Created($"/rasp-bp/{bp.IdRaspBp}", bp);
});


app.MapPost("/rasp/{id:int}/retornar-analise", async (int id, AcaoFluxoRaspRequest req, RaspDbContext db, IConfiguration config) =>
{
    var item = await db.Rasp.FindAsync(id);

    if (item is null)
        return Results.NotFound("RASP não encontrado.");

    if (item.IdStatusRasp != 2 && item.IdStatusRasp != 3)
        return Results.BadRequest("Somente RASP em avaliação FT ou LG pode retornar para análise.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    var isAdmin = usuarioExecutor.IdPerfil == 1;

    if (!isAdmin)
        return Results.BadRequest("Somente ADMIN pode retornar este registro para análise.");

    var connStr = config.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connStr))
        return Results.Problem("Connection string 'DefaultConnection' não encontrada.");

    await using var conn = new NpgsqlConnection(connStr);
    await conn.OpenAsync();
    await using var tx = await conn.BeginTransactionAsync();

    try
    {
        var updateSql = """
            UPDATE rasp
            SET is_rascunho = false,
                id_status_rasp = 1
            WHERE id_rasp = @id;
            """;

        await using (var cmd = new NpgsqlCommand(updateSql, conn, tx))
        {
            cmd.Parameters.AddWithValue("id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        await tx.CommitAsync();

        db.Entry(item).State = EntityState.Detached;

        var atualizado = await db.Rasp
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.IdRasp == id);

        return Results.Ok(atualizado);
    }
    catch (PostgresException ex)
    {
        await tx.RollbackAsync();
        return Results.BadRequest(ex.MessageText);
    }
})
.WithName("RetornarRaspParaAnalise");

app.MapPost("/rasp/{id:int}/registrar-spps", async (int id, RegistrarSppsRequest req, RaspDbContext db) =>
{
    var item = await db.Rasp.FindAsync(id);

    if (item is null)
        return Results.NotFound("RASP não encontrado.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    var isAdmin = usuarioExecutor.IdPerfil == 1;
    var isFt = usuarioExecutor.IdPerfil == 3;

    if (!isAdmin && !isFt)
        return Results.BadRequest("Somente FT ou ADMIN pode registrar SPPS.");

    var sppsNumero = (req.SppsNumero ?? string.Empty).Trim();

    if (string.IsNullOrWhiteSpace(sppsNumero))
        return Results.BadRequest("SppsNumero é obrigatório.");

    if (sppsNumero.Length != 6)
        return Results.BadRequest("SppsNumero deve ter exatamente 6 dígitos.");

    if (!sppsNumero.All(char.IsDigit))
        return Results.BadRequest("SppsNumero deve conter somente números.");

    if (!isAdmin && !string.IsNullOrWhiteSpace(item.SppsNumero))
        return Results.BadRequest("Este RASP já possui SPPS registrado.");

    item.SppsNumero = sppsNumero;
    item.IdSppsClassificacaoRasp = req.IdSppsClassificacaoRasp;
    item.IdSppsStatusRasp = req.IdSppsStatusRasp;

    try
    {
        await db.SaveChangesAsync();
        return Results.Ok(item);
    }
    catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
    {
        if (pgEx.SqlState == "23503")
        {
            return Results.BadRequest(
                $"Um dos IDs informados não existe em tabela auxiliar. Constraint: {pgEx.ConstraintName}");
        }

        if (pgEx.SqlState == "23505")
        {
            return Results.BadRequest(
                $"Violação de unicidade no banco. Constraint: {pgEx.ConstraintName}");
        }

        return Results.BadRequest(
            $"Erro de banco ao registrar SPPS. Constraint: {pgEx.ConstraintName}");
    }
})
.WithName("RegistrarSppsNoRasp");

// -----------------------------------------------------------------------------
// 16. RASP_PN
// -----------------------------------------------------------------------------

app.MapGet("/rasp-pn", async (RaspDbContext db) =>
{
    var itens = await db.RaspPn
        .OrderByDescending(rp => rp.IdRaspPn)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspPn");

app.MapGet("/rasp-pn/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.RaspPn.FindAsync(id);

    return item is null
        ? Results.NotFound("RASP PN não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterRaspPnPorId");

app.MapGet("/rasp/{id:int}/pns", async (int id, RaspDbContext db) =>
{
    var raspExiste = await db.Rasp.AnyAsync(r => r.IdRasp == id);

    if (!raspExiste)
        return Results.NotFound($"RASP com id {id} não encontrado.");

    var itens = await db.RaspPn
        .Where(rp => rp.IdRasp == id)
        .OrderBy(rp => rp.OrdemExibicao)
        .ThenBy(rp => rp.IdRaspPn)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarPnsPorRasp");

app.MapPut("/rasp-pn/{idRaspPn:int}/entrar-selecao", async (
    int idRaspPn,
    EntrarSelecaoRequest request,
    RaspDbContext db) =>
{
    var raspPn = await db.RaspPn
        .FirstOrDefaultAsync(x => x.IdRaspPn == idRaspPn);

    if (raspPn is null)
    {
        return Results.NotFound(new { mensagem = "RASP_PN não encontrado." });
    }

    if (raspPn.StatusSelecao == 2)
    {
        return Results.BadRequest(new
        {
            mensagem = "Este PN já teve a seleção encerrada neste RASP. É necessário abrir um novo RASP."
        });
    }

    if (raspPn.StatusSelecao == 1)
    {
        return Results.BadRequest(new
        {
            mensagem = "Este PN já está em seleção."
        });
    }

    var agora = DateTime.UtcNow;

    raspPn.EntrouSelecao = true;
    raspPn.StatusSelecao = 1;
    raspPn.DataHoraEntradaSelecao = agora;
    raspPn.DataHoraSaidaSelecao = null;

    raspPn.TravaAtiva = request.TravaAtiva;

    if (request.TravaAtiva)
    {
        raspPn.DataHoraSolicitacaoTrava = agora;
        raspPn.DataHoraRemocaoTrava = null;
    }
    else
    {
        raspPn.DataHoraSolicitacaoTrava = null;
        raspPn.DataHoraRemocaoTrava = null;
    }

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        mensagem = "PN colocado em seleção com sucesso.",
        idRaspPn = raspPn.IdRaspPn,
        entrouSelecao = raspPn.EntrouSelecao,
        statusSelecao = raspPn.StatusSelecao,
        datahoraEntradaSelecao = raspPn.DataHoraEntradaSelecao,
        travaAtiva = raspPn.TravaAtiva,
        datahoraSolicitacaoTrava = raspPn.DataHoraSolicitacaoTrava
    });
})
.WithName("EntrarSelecaoRaspPn")
.WithTags("RASP Seleção");

app.MapPut("/rasp-pn/{idRaspPn:int}/sair-selecao", async (
    int idRaspPn,
    SairSelecaoRequest request,
    RaspDbContext db) =>
{
    var raspPn = await db.RaspPn
        .FirstOrDefaultAsync(x => x.IdRaspPn == idRaspPn);

    if (raspPn is null)
    {
        return Results.NotFound(new { mensagem = "RASP_PN não encontrado." });
    }

    if (raspPn.StatusSelecao == 0)
    {
        return Results.BadRequest(new
        {
            mensagem = "Este PN ainda não entrou em seleção."
        });
    }

    if (raspPn.StatusSelecao == 2)
    {
        return Results.BadRequest(new
        {
            mensagem = "Este PN já está com a seleção encerrada."
        });
    }

    var agora = DateTime.UtcNow;

    raspPn.StatusSelecao = 2;
    raspPn.DataHoraSaidaSelecao = agora;

    if (raspPn.TravaAtiva && request.RemoverTrava)
    {
        raspPn.TravaAtiva = false;
        raspPn.DataHoraRemocaoTrava = agora;
    }

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        mensagem = "Seleção encerrada com sucesso.",
        idRaspPn = raspPn.IdRaspPn,
        statusSelecao = raspPn.StatusSelecao,
        datahoraSaidaSelecao = raspPn.DataHoraSaidaSelecao,
        travaAtiva = raspPn.TravaAtiva,
        datahoraRemocaoTrava = raspPn.DataHoraRemocaoTrava
    });
})
.WithName("SairSelecaoRaspPn")
.WithTags("RASP Seleção");



app.MapGet("/rasp/{id:int}/detalhe", async (int id, RaspDbContext db) =>
{
    var rasp = await db.Rasp
        .AsNoTracking()
        .FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp is null)
        return Results.NotFound($"RASP com id {id} não encontrado.");

    var pns = await db.RaspPn
        .AsNoTracking()
        .Where(rp => rp.IdRasp == id)
        .OrderBy(rp => rp.OrdemExibicao)
        .ThenBy(rp => rp.IdRaspPn)
        .ToListAsync();

    var arquivos = await db.RaspArquivo
        .AsNoTracking()
        .Where(a => a.IdRasp == id)
        .OrderBy(a => a.IdArquivoRasp)
        .ToListAsync();

    var anotacoes = await db.RaspAnotacao
        .AsNoTracking()
        .Where(a => a.IdRasp == id)
        .OrderBy(a => a.DataHora)
        .ToListAsync();

    return Results.Ok(new
    {
        rasp,
        pns,
        arquivos,
        anotacoes
    });
})
.WithName("ObterRaspDetalhe");

app.MapGet("/rasp/numero/{numeroRasp}", async (string numeroRasp, RaspDbContext db) =>
{
    var numeroLimpo = System.Net.WebUtility.UrlDecode((numeroRasp ?? string.Empty).Trim());

    if (string.IsNullOrWhiteSpace(numeroLimpo))
        return Results.BadRequest("NumeroRasp é obrigatório.");

    // remove espaços
    numeroLimpo = numeroLimpo.Replace(" ", "");

    // se vier no formato completo, busca exata
    if (numeroLimpo.Contains("/"))
    {
        var itemCompleto = await db.Rasp
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.NumeroRasp == numeroLimpo);

        return itemCompleto is null
            ? Results.NotFound("RASP não encontrado para o número informado.")
            : Results.Ok(itemCompleto);
    }

    // se vier só a parte inicial, ex: 0056
    var candidatos = await db.Rasp
        .AsNoTracking()
        .Where(r => r.NumeroRasp.StartsWith(numeroLimpo + "/"))
        .OrderByDescending(r => r.IdRasp)
        .ToListAsync();

    if (candidatos.Count == 0)
        return Results.NotFound("RASP não encontrado para o número informado.");

    if (candidatos.Count == 1)
        return Results.Ok(candidatos[0]);

    // se houver mais de um com a mesma base, retorna o mais recente
    return Results.Ok(candidatos[0]);
})
.WithName("ObterRaspPorNumero");


app.MapGet("/rasp/resumo", async (RaspDbContext db) =>
{
    var itens = await db.Rasp
        .AsNoTracking()
        .OrderByDescending(r => r.IdRasp)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.IdFornecedorRasp,
            r.DescricaoProblema,
            r.IsRascunho
        })
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspResumo");

app.MapGet("/rasp/status/{idStatus:int}", async (int idStatus, RaspDbContext db) =>
{
    if (idStatus <= 0)
        return Results.BadRequest("IdStatus inválido.");

    var statusExiste = await db.StatusRasp.AnyAsync(s => s.IdStatusRasp == idStatus);

    if (!statusExiste)
        return Results.NotFound($"Status com id {idStatus} não encontrado.");

    var itens = await db.Rasp
        .AsNoTracking()
        .Where(r => r.IdStatusRasp == idStatus)
        .OrderByDescending(r => r.IdRasp)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.IdFornecedorRasp,
            r.DescricaoProblema,
            r.IsRascunho
        })
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspPorStatus");

app.MapGet("/rasp/fornecedor/{idFornecedor:int}", async (int idFornecedor, RaspDbContext db) =>
{
    if (idFornecedor <= 0)
        return Results.BadRequest("IdFornecedor inválido.");

    var fornecedorExiste = await db.FornecedorRasp.AnyAsync(f => f.IdFornecedor == idFornecedor);

    if (!fornecedorExiste)
        return Results.NotFound($"Fornecedor com id {idFornecedor} não encontrado.");

    var itens = await db.Rasp
        .AsNoTracking()
        .Where(r => r.IdFornecedorRasp == idFornecedor)
        .OrderByDescending(r => r.IdRasp)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.IdFornecedorRasp,
            r.DescricaoProblema,
            r.IsRascunho
        })
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspPorFornecedor");

app.MapGet("/rasp/rascunhos", async (RaspDbContext db) =>
{
    var itens = await db.Rasp
        .AsNoTracking()
        .Where(r => r.IsRascunho)
        .OrderByDescending(r => r.IdRasp)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.IdFornecedorRasp,
            r.DescricaoProblema,
            r.IsRascunho
        })
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRascunhosRasp");

app.MapPut("/rasp/{id:int}/trocar-ft", async (int id, TrocarFtRaspRequest req, RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    if (req.IdNovoAprovadorFt <= 0)
        return Results.BadRequest("IdNovoAprovadorFt inválido.");

    var item = await db.Rasp
        .FirstOrDefaultAsync(r => r.IdRasp == id);

    if (item is null)
        return Results.NotFound("RASP não encontrado.");

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    var isAdmin = usuarioExecutor.IdPerfil == 1;

    if (!isAdmin)
        return Results.BadRequest("Somente ADMIN pode trocar o aprovador FT do RASP.");

    if (item.IdStatusRasp == 4)
        return Results.BadRequest("RASP concluído não pode ter o aprovador FT trocado por esta rota.");

    var novoFt = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdNovoAprovadorFt);

    if (novoFt is null)
        return Results.BadRequest("Novo aprovador FT não existe.");

    if (!novoFt.Ativo)
        return Results.BadRequest("Novo aprovador FT está inativo.");

    if (novoFt.IdPerfil != 3)
        return Results.BadRequest("O novo responsável deve possuir perfil FT.");

    if (item.IdAprovadorFt.HasValue && item.IdAprovadorFt.Value == req.IdNovoAprovadorFt)
        return Results.BadRequest("Este RASP já está vinculado a esse aprovador FT.");

    var idFtAnterior = item.IdAprovadorFt;

    item.IdAprovadorFt = req.IdNovoAprovadorFt;

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        idRasp = item.IdRasp,
        numeroRasp = item.NumeroRasp,
        idFtAnterior,
        idNovoAprovadorFt = item.IdAprovadorFt,
        alteradoPor = usuarioExecutor.IdUsuario
    });
})
.WithName("TrocarAprovadorFtRasp");

app.MapGet("/rasp/concluidos", async (RaspDbContext db) =>
{
    var itens = await db.Rasp
        .AsNoTracking()
        .Where(r => r.IdStatusRasp == 4)
        .OrderByDescending(r => r.IdRasp)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.IdFornecedorRasp,
            r.DescricaoProblema,
            r.IsRascunho
        })
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspConcluidos");

// -----------------------------------------------------------------------------
// 17. DASHBOARD / INDICADORES OPERACIONAIS
// -----------------------------------------------------------------------------

app.MapGet("/rasp/contadores", async (RaspDbContext db) =>
{
    var total = await db.Rasp.CountAsync();
    var emAberto = await db.Rasp.CountAsync(r => r.IdStatusRasp != 4);
    var rascunhos = await db.Rasp.CountAsync(r => r.IsRascunho);
    var emAnalise = await db.Rasp.CountAsync(r => r.IdStatusRasp == 1);
    var emAvaliacaoFt = await db.Rasp.CountAsync(r => r.IdStatusRasp == 2);
    var emAvaliacaoLg = await db.Rasp.CountAsync(r => r.IdStatusRasp == 3);
    var concluidos = await db.Rasp.CountAsync(r => r.IdStatusRasp == 4);

    return Results.Ok(new
    {
        total,
        emAberto,
        rascunhos,
        emAnalise,
        emAvaliacaoFt,
        emAvaliacaoLg,
        concluidos
    });
})
.WithName("ObterContadoresRasp");

app.MapGet("/rasp/aging", async (RaspDbContext db) =>
{
    var hoje = DateOnly.FromDateTime(DateTime.Today);

    var itens = await db.Rasp
        .AsNoTracking()
        .Where(r => r.IdStatusRasp != 4)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.IdFornecedorRasp,
            r.IdAnalistaMt,
            r.DescricaoProblema,
            r.IsRascunho
        })
        .ToListAsync();

    var resultado = itens
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.IdFornecedorRasp,
            r.IdAnalistaMt,
            r.DescricaoProblema,
            r.IsRascunho,
            diasEmAbertoTotal = hoje.DayNumber - r.DataCriacao.DayNumber
        })
        .OrderByDescending(r => r.diasEmAbertoTotal)
        .ThenBy(r => r.IdRasp)
        .ToList();

    return Results.Ok(resultado);
})
.WithName("ListarAgingRasp");

app.MapGet("/rasp/analista/{idUsuario:int}/backlog", async (int idUsuario, RaspDbContext db) =>
{
    if (idUsuario <= 0)
        return Results.BadRequest("IdUsuario inválido.");

    var usuario = await db.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

    if (usuario is null)
        return Results.NotFound($"Usuário com id {idUsuario} não encontrado.");

    var hoje = DateOnly.FromDateTime(DateTime.Today);

    var itens = await db.Rasp
        .AsNoTracking()
        .Where(r => r.IdAnalistaMt == idUsuario && r.IdStatusRasp != 4)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.IdFornecedorRasp,
            r.DescricaoProblema,
            r.IsRascunho
        })
        .ToListAsync();

    var resultado = itens
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.IdFornecedorRasp,
            r.DescricaoProblema,
            r.IsRascunho,
            diasEmAbertoTotal = hoje.DayNumber - r.DataCriacao.DayNumber
        })
        .OrderByDescending(r => r.diasEmAbertoTotal)
        .ThenBy(r => r.IdRasp)
        .ToList();

    return Results.Ok(new
    {
        idUsuario = usuario.IdUsuario,
        nomeUsuario = usuario.Nome,
        totalItens = resultado.Count,
        itens = resultado
    });
})
.WithName("ObterBacklogAnalistaRasp");

app.MapGet("/rasp/fornecedor/{idFornecedor:int}/backlog", async (int idFornecedor, RaspDbContext db) =>
{
    if (idFornecedor <= 0)
        return Results.BadRequest("IdFornecedor inválido.");

    var fornecedor = await db.FornecedorRasp
        .AsNoTracking()
        .FirstOrDefaultAsync(f => f.IdFornecedor == idFornecedor);

    if (fornecedor is null)
        return Results.NotFound($"Fornecedor com id {idFornecedor} não encontrado.");

    var hoje = DateOnly.FromDateTime(DateTime.Today);

    var itens = await db.Rasp
        .AsNoTracking()
        .Where(r => r.IdFornecedorRasp == idFornecedor && r.IdStatusRasp != 4)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.DescricaoProblema,
            r.IsRascunho,
            r.IdAnalistaMt
        })
        .ToListAsync();

    var resultado = itens
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.IdStatusRasp,
            r.DescricaoProblema,
            r.IsRascunho,
            r.IdAnalistaMt,
            diasEmAbertoTotal = hoje.DayNumber - r.DataCriacao.DayNumber
        })
        .OrderByDescending(r => r.diasEmAbertoTotal)
        .ThenBy(r => r.IdRasp)
        .ToList();

    return Results.Ok(new
    {
        idFornecedor = fornecedor.IdFornecedor,
        nomeFornecedor = fornecedor.Nome,
        totalItens = resultado.Count,
        itens = resultado
    });
})
.WithName("ObterBacklogFornecedorRasp");

app.MapPut("/rasp/{id:int}/trocar-analista", async (int id, TrocarAnalistaRaspRequest req, RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    if (req.IdNovoAnalistaMt <= 0)
        return Results.BadRequest("IdNovoAnalistaMt inválido.");

    var item = await db.Rasp
        .FirstOrDefaultAsync(r => r.IdRasp == id);

    if (item is null)
        return Results.NotFound("RASP não encontrado.");

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    var isAdmin = usuarioExecutor.IdPerfil == 1;

    if (!isAdmin)
        return Results.BadRequest("Somente ADMIN pode trocar o analista MT do RASP.");

    if (item.IdStatusRasp == 4)
        return Results.BadRequest("RASP concluído não pode ter o analista MT trocado por esta rota.");

    var novoAnalista = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdNovoAnalistaMt);

    if (novoAnalista is null)
        return Results.BadRequest("Novo analista não existe.");

    if (!novoAnalista.Ativo)
        return Results.BadRequest("Novo analista está inativo.");

    if (novoAnalista.IdPerfil != 2)
        return Results.BadRequest("O novo responsável deve possuir perfil ANALISTA.");

    if (item.IdAnalistaMt.HasValue && item.IdAnalistaMt.Value == req.IdNovoAnalistaMt)
        return Results.BadRequest("Este RASP já está vinculado a esse analista.");

    var idAnalistaAnterior = item.IdAnalistaMt;

    item.IdAnalistaMt = req.IdNovoAnalistaMt;

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        idRasp = item.IdRasp,
        numeroRasp = item.NumeroRasp,
        idAnalistaAnterior,
        idNovoAnalistaMt = item.IdAnalistaMt,
        alteradoPor = usuarioExecutor.IdUsuario
    });
})
.WithName("TrocarAnalistaMtRasp");

// -----------------------------------------------------------------------------
// 18. RASP PN - CRIAÇÃO / EDIÇÃO / EXCLUSÃO
// -----------------------------------------------------------------------------
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
    if (!pn.All(char.IsDigit))
        return Results.BadRequest("Pn deve conter somente números.");

    var pnExiste = await db.PnRasp.AnyAsync(p => p.CodigoPn == pn);
    if (!pnExiste)
        return Results.BadRequest("PN informado não existe no cadastro.");

    var duns = (req.Duns ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(duns))
        return Results.BadRequest("Duns é obrigatório.");
    if (duns.Length != 9)
        return Results.BadRequest("Duns deve ter exatamente 9 caracteres.");
    if (!duns.All(char.IsDigit))
        return Results.BadRequest("Duns deve conter somente números.");

    var dunsExiste = await db.FornecedorRasp.AnyAsync(f => f.Duns == duns);
    if (!dunsExiste)
        return Results.BadRequest("DUNS informado não existe no cadastro de fornecedor.");

    if (req.QuantidadeSuspeita < 0 || req.QuantidadeChecada < 0 || req.QuantidadeRejeitada < 0)
        return Results.BadRequest("Quantidades não podem ser negativas.");

    if (req.OrdemExibicao <= 0)
        return Results.BadRequest("OrdemExibicao deve ser maior que zero.");

    var raspPnJaExiste = await db.RaspPn
        .AnyAsync(rp => rp.IdRasp == req.IdRasp && rp.Pn == pn);
    if (raspPnJaExiste)
        return Results.BadRequest("Este PN já está vinculado a este RASP.");

    DateTime? dataLoteInicial = null;
    if (!string.IsNullOrWhiteSpace(req.DataLoteInicial))
    {
        if (!DateTime.TryParseExact(
            req.DataLoteInicial,
            "dd/MM/yy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out var dataConvertida))
        {
            return Results.BadRequest("DataLoteInicial inválida. Use o formato DD/MM/AA.");
        }

        dataLoteInicial = DateTime.SpecifyKind(dataConvertida, DateTimeKind.Utc);
    }

    // -------------------------------------------------------------------------
    // CONTROLE DE SELEÇÃO / TRAVA / QHD
    // -------------------------------------------------------------------------
    var entrouSelecao = req.EntrouSelecao;
    var statusSelecao = req.StatusSelecao;
    var dataHoraEntradaSelecao = req.DataHoraEntradaSelecao;
    var dataHoraSaidaSelecao = req.DataHoraSaidaSelecao;

    var travaAtiva = req.TravaAtiva;
    var dataHoraSolicitacaoTrava = req.DataHoraSolicitacaoTrava;
    var dataHoraRemocaoTrava = req.DataHoraRemocaoTrava;

    var qhdAtivo = req.QhdAtivo;
    var dataHoraQhd = req.DataHoraQhd;

    if (statusSelecao < 0)
        return Results.BadRequest("StatusSelecao inválido.");

    if (dataHoraEntradaSelecao.HasValue && dataHoraSaidaSelecao.HasValue &&
        dataHoraSaidaSelecao.Value < dataHoraEntradaSelecao.Value)
    {
        return Results.BadRequest("DataHoraSaidaSelecao não pode ser menor que DataHoraEntradaSelecao.");
    }

    if (dataHoraSolicitacaoTrava.HasValue && dataHoraRemocaoTrava.HasValue &&
        dataHoraRemocaoTrava.Value < dataHoraSolicitacaoTrava.Value)
    {
        return Results.BadRequest("DataHoraRemocaoTrava não pode ser menor que DataHoraSolicitacaoTrava.");
    }

    // Se entrou em seleção e não veio status, assume Em seleção = 1
    if (entrouSelecao && statusSelecao == 0)
        statusSelecao = 1;

    // Se não entrou em seleção e não veio status, assume Fora da seleção = 0
    if (!entrouSelecao && statusSelecao == 0)
        statusSelecao = 0;

    var item = new RaspPnEntity
    {
        IdRasp = req.IdRasp,
        Pn = pn,
        DataLoteInicial = dataLoteInicial,
        QuantidadeSuspeita = req.QuantidadeSuspeita,
        QuantidadeChecada = req.QuantidadeChecada,
        QuantidadeRejeitada = req.QuantidadeRejeitada,
        EmContencao = req.EmContencao,
        Duns = duns,
        OrdemExibicao = req.OrdemExibicao,

        EntrouSelecao = entrouSelecao,
        StatusSelecao = statusSelecao,
        DataHoraEntradaSelecao = dataHoraEntradaSelecao,
        DataHoraSaidaSelecao = dataHoraSaidaSelecao,

        TravaAtiva = travaAtiva,
        DataHoraSolicitacaoTrava = dataHoraSolicitacaoTrava,
        DataHoraRemocaoTrava = dataHoraRemocaoTrava,

        QhdAtivo = qhdAtivo,
        DataHoraQhd = dataHoraQhd
    };

    db.RaspPn.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/rasp-pn/{item.IdRaspPn}", item);
})
.WithName("CriarRaspPn");

app.MapPut("/rasp-pn/{id:int}", async (int id, AtualizarRaspPnRequest req, RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do vínculo RASP PN inválido.");

    var item = await db.RaspPn
        .FirstOrDefaultAsync(rp => rp.IdRaspPn == id);

    if (item is null)
        return Results.NotFound("RASP PN não encontrado.");

    var duns = (req.Duns ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(duns))
        return Results.BadRequest("Duns é obrigatório.");
    if (duns.Length != 9)
        return Results.BadRequest("Duns deve ter exatamente 9 caracteres.");
    if (!duns.All(char.IsDigit))
        return Results.BadRequest("Duns deve conter somente números.");

    var dunsExiste = await db.FornecedorRasp.AnyAsync(f => f.Duns == duns);
    if (!dunsExiste)
        return Results.BadRequest("DUNS informado não existe no cadastro de fornecedor.");

    if (req.QuantidadeSuspeita < 0 || req.QuantidadeChecada < 0 || req.QuantidadeRejeitada < 0)
        return Results.BadRequest("Quantidades não podem ser negativas.");

    if (req.OrdemExibicao <= 0)
        return Results.BadRequest("OrdemExibicao deve ser maior que zero.");

    DateTime? dataLoteInicial = null;
    if (!string.IsNullOrWhiteSpace(req.DataLoteInicial))
    {
        if (!DateTime.TryParseExact(
            req.DataLoteInicial,
            "dd/MM/yy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out var dataConvertida))
        {
            return Results.BadRequest("DataLoteInicial inválida. Use o formato DD/MM/AA.");
        }

        dataLoteInicial = DateTime.SpecifyKind(dataConvertida, DateTimeKind.Utc);
    }

    // -------------------------------------------------------------------------
    // CONTROLE DE SELEÇÃO / TRAVA / QHD
    // -------------------------------------------------------------------------
    var entrouSelecao = req.EntrouSelecao;
    var statusSelecao = req.StatusSelecao;
    var dataHoraEntradaSelecao = req.DataHoraEntradaSelecao;
    var dataHoraSaidaSelecao = req.DataHoraSaidaSelecao;

    var travaAtiva = req.TravaAtiva;
    var dataHoraSolicitacaoTrava = req.DataHoraSolicitacaoTrava;
    var dataHoraRemocaoTrava = req.DataHoraRemocaoTrava;

    var qhdAtivo = req.QhdAtivo;
    var dataHoraQhd = req.DataHoraQhd;

    if (statusSelecao < 0)
        return Results.BadRequest("StatusSelecao inválido.");

    if (dataHoraEntradaSelecao.HasValue && dataHoraSaidaSelecao.HasValue &&
        dataHoraSaidaSelecao.Value < dataHoraEntradaSelecao.Value)
    {
        return Results.BadRequest("DataHoraSaidaSelecao não pode ser menor que DataHoraEntradaSelecao.");
    }

    if (dataHoraSolicitacaoTrava.HasValue && dataHoraRemocaoTrava.HasValue &&
        dataHoraRemocaoTrava.Value < dataHoraSolicitacaoTrava.Value)
    {
        return Results.BadRequest("DataHoraRemocaoTrava não pode ser menor que DataHoraSolicitacaoTrava.");
    }

    if (entrouSelecao && statusSelecao == 0)
        statusSelecao = 1;

    if (!entrouSelecao && statusSelecao == 0)
        statusSelecao = 0;

    item.DataLoteInicial = dataLoteInicial;
    item.QuantidadeSuspeita = req.QuantidadeSuspeita;
    item.QuantidadeChecada = req.QuantidadeChecada;
    item.QuantidadeRejeitada = req.QuantidadeRejeitada;
    item.EmContencao = req.EmContencao;
    item.Duns = duns;
    item.OrdemExibicao = req.OrdemExibicao;

    item.EntrouSelecao = entrouSelecao;
    item.StatusSelecao = statusSelecao;
    item.DataHoraEntradaSelecao = dataHoraEntradaSelecao;
    item.DataHoraSaidaSelecao = dataHoraSaidaSelecao;

    item.TravaAtiva = travaAtiva;
    item.DataHoraSolicitacaoTrava = dataHoraSolicitacaoTrava;
    item.DataHoraRemocaoTrava = dataHoraRemocaoTrava;

    item.QhdAtivo = qhdAtivo;
    item.DataHoraQhd = dataHoraQhd;

    await db.SaveChangesAsync();

    return Results.Ok(item);
})
.WithName("AtualizarRaspPn");

app.MapDelete("/rasp-pn/{id:int}", async (int id, RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do vínculo RASP PN inválido.");

    var item = await db.RaspPn
        .FirstOrDefaultAsync(rp => rp.IdRaspPn == id);

    if (item is null)
        return Results.NotFound("RASP PN não encontrado.");

    db.RaspPn.Remove(item);
    await db.SaveChangesAsync();

    return Results.Ok($"RASP PN com id {id} removido com sucesso.");
})
.WithName("ExcluirRaspPn");


// -----------------------------------------------------------------------------
// 18A. SELECAO OPERACIONAL - ITENS ATIVOS
// Finalidade:
// - fornecer para a tela operacional uma visão pronta dos itens em seleção ativa
// - evitar que o front precise juntar várias APIs
// - devolver dados já enriquecidos com:
//   • PN real
//   • número do RASP
//   • fornecedor
//   • MT responsável
//   • turno
//   • data de entrada em seleção
//   • duração real em seleção
//   • último apontamento operacional do item
//   • dias sem apontamento
//
// Regras desta visão:
// - considera ativo todo RASP_PN com StatusSelecao = 1
// - busca apontamentos em rasp_contencao por IdRaspPn
// - duração em seleção:
//     1º tenta usar a menor DataLoteVerificada do histórico
//     2º se não houver histórico, usa DataHoraEntradaSelecao
// - dias sem apontamento:
//     usa o último apontamento registrado
// -----------------------------------------------------------------------------
app.MapGet("/selecao-operacional/itens-ativos", async (RaspDbContext db) =>
{
    // -------------------------------------------------------------------------
    // 01. BUSCA BASE DOS ITENS EM SELEÇÃO ATIVA
    // -------------------------------------------------------------------------
    var itensBase = await (
        from rp in db.RaspPn.AsNoTracking()
        join r in db.Rasp.AsNoTracking()
            on rp.IdRasp equals r.IdRasp

        join f in db.FornecedorRasp.AsNoTracking()
            on r.IdFornecedorRasp equals f.IdFornecedor

        join mtJoin in db.Usuarios.AsNoTracking()
            on r.IdAnalistaMt equals mtJoin.IdUsuario into mtLeft
        from mt in mtLeft.DefaultIfEmpty()

        join turnoJoin in db.TurnoRasp.AsNoTracking()
            on r.IdTurnoRasp equals turnoJoin.IdTurnoRasp into turnoLeft
        from turno in turnoLeft.DefaultIfEmpty()

        where rp.StatusSelecao == 1

        select new
        {
            rp.IdRaspPn,
            Pn = rp.Pn,
            rp.IdRasp,
            r.NumeroRasp,
            Fornecedor = f.Nome,
            MtResponsavel = mt != null ? mt.Nome : null,
            Turno = turno != null ? turno.Descricao : null,
            rp.DataHoraEntradaSelecao,
            rp.TravaAtiva,

            // QHD é uma condição separada da trava comum
            rp.QhdAtivo,
            rp.DataHoraQhd,

            rp.StatusSelecao

        }
    )
    .OrderBy(x => x.NumeroRasp)
    .ThenBy(x => x.Pn)
    .ToListAsync();

    // -------------------------------------------------------------------------
    // 02. SE NÃO HOUVER ITENS ATIVOS, DEVOLVE LISTA VAZIA
    // -------------------------------------------------------------------------
    if (itensBase.Count == 0)
        return Results.Ok(Array.Empty<object>());

    // -------------------------------------------------------------------------
    // 03. MONTA LISTA DE IDS DOS ITENS ATIVOS
    // -------------------------------------------------------------------------
    var idsRaspPn = itensBase
        .Select(x => x.IdRaspPn)
        .ToList();

    // -------------------------------------------------------------------------
    // 04. BUSCA TODOS OS APONTAMENTOS DOS ITENS ATIVOS
    // -------------------------------------------------------------------------
    var apontamentos = await db.Set<RaspContencao>()
        .AsNoTracking()
        .Where(x => idsRaspPn.Contains(x.IdRaspPn))
        .Select(x => new
        {
            x.IdRaspContencao,
            x.IdRaspPn,
            x.DataAtualizacao,
            x.DataLoteVerificada,
            x.DataHoraInicioAtividade,
            x.DataHoraFimAtividade,
            x.IdOperadorSelecaoTerceiro,
            x.OrigemRegistro,
            x.QuantidadeVerificada,
            x.QuantidadeRejeitada,
            x.QuantidadeOk,
            x.Observacao
        })
        .ToListAsync();

    // -------------------------------------------------------------------------
    // 05. PEGA O ÚLTIMO APONTAMENTO DE CADA ITEM
    // -------------------------------------------------------------------------
    var ultimoApontamentoPorRaspPn = apontamentos
        .GroupBy(x => x.IdRaspPn)
        .ToDictionary(
            g => g.Key,
            g => g
                .OrderByDescending(x => x.DataHoraFimAtividade ?? x.DataHoraInicioAtividade ?? x.DataAtualizacao)
                .First()
        );

    // -------------------------------------------------------------------------
    // 06. PEGA DATA OFICIAL DE ENTRADA EM SELEÇÃO
    // REGRA:
    // - Usa a data registrada quando o analista ativou a seleção
    // - NÃO usa data do lote
    // - NÃO usa apontamentos operacionais
    // -------------------------------------------------------------------------
    var dataEntradaSelecaoPorRaspPn = itensBase
        .Where(x => x.DataHoraEntradaSelecao.HasValue)
        .ToDictionary(
            x => x.IdRaspPn,
            x => x.DataHoraEntradaSelecao!.Value.Date
        );



    // -------------------------------------------------------------------------
    // 07. MONTA LISTA DE IDS DE OPERADORES TERCEIROS USADOS
    // -------------------------------------------------------------------------
    var idsOperadores = ultimoApontamentoPorRaspPn.Values
        .Where(x => x.IdOperadorSelecaoTerceiro.HasValue)
        .Select(x => x.IdOperadorSelecaoTerceiro!.Value)
        .Distinct()
        .ToList();

    // -------------------------------------------------------------------------
    // 08. BUSCA DADOS DOS OPERADORES TERCEIROS
    // -------------------------------------------------------------------------
    var operadores = await db.OperadorSelecaoTerceiro
        .AsNoTracking()
        .Where(x => idsOperadores.Contains(x.IdOperadorSelecaoTerceiro))
        .Select(x => new
        {
            x.IdOperadorSelecaoTerceiro,
            x.Nome,
            x.Empresa
        })
        .ToListAsync();

    var operadorPorId = operadores.ToDictionary(
        x => x.IdOperadorSelecaoTerceiro,
        x => x
    );

    // -------------------------------------------------------------------------
    // 09. DATA BASE PARA CÁLCULOS
    // -------------------------------------------------------------------------
    var hoje = DateTime.UtcNow.Date;

    // -------------------------------------------------------------------------
    // 10. MONTA O RETORNO FINAL DA TELA OPERACIONAL
    // -------------------------------------------------------------------------
    var resultado = itensBase.Select(itemBase =>
    {
        // ---------------------------------------------------------------------
        // 10.1. Recupera último apontamento do item
        // ---------------------------------------------------------------------
        ultimoApontamentoPorRaspPn.TryGetValue(itemBase.IdRaspPn, out var ultimo);

        // ---------------------------------------------------------------------
        // 10.2. Define data real de entrada em seleção
        // Regra:
        // - Se houver histórico, usa a primeira DataLoteVerificada
        // - Caso contrário, usa DataHoraEntradaSelecao
        // ---------------------------------------------------------------------
        DateTime? dataEntradaSelecao = null;

        if (dataEntradaSelecaoPorRaspPn.TryGetValue(itemBase.IdRaspPn, out var dataEntrada))
        {
            dataEntradaSelecao = dataEntrada;
        }


        // ---------------------------------------------------------------------
        // 10.3. Calcula duração em seleção
        // ---------------------------------------------------------------------
        int? duracaoEmSelecaoDias = null;

        if (dataEntradaSelecao.HasValue)
        {
            duracaoEmSelecaoDias = (hoje - dataEntradaSelecao.Value.Date).Days;

            if (duracaoEmSelecaoDias < 0)
                duracaoEmSelecaoDias = 0;
        }

        // ---------------------------------------------------------------------
        // 10.4. Calcula dias sem apontamento
        // ---------------------------------------------------------------------
        DateTime? dataUltimoApontamento =
            ultimo?.DataHoraFimAtividade ??
            ultimo?.DataHoraInicioAtividade ??
            ultimo?.DataAtualizacao;

        int? diasSemApontamento = null;

        if (dataUltimoApontamento.HasValue)
        {
            diasSemApontamento = (hoje - dataUltimoApontamento.Value.Date).Days;

            if (diasSemApontamento < 0)
                diasSemApontamento = 0;
        }

        // ---------------------------------------------------------------------
        // 10.5. Recupera dados do operador terceiro, se houver
        // ---------------------------------------------------------------------
        string? nomeOperador = null;
        string? empresaOperador = null;

        if (ultimo?.IdOperadorSelecaoTerceiro is int idOperador &&
            operadorPorId.TryGetValue(idOperador, out var operador))
        {
            nomeOperador = operador.Nome;
            empresaOperador = operador.Empresa;
        }

        // ---------------------------------------------------------------------
        // 10.6. Retorno final da linha operacional
        // ---------------------------------------------------------------------
        return new
        {
            // -----------------------------------------------------
            // IDENTIFICAÇÃO DO ITEM EM SELEÇÃO
            // -----------------------------------------------------
            itemBase.IdRaspPn,
            Pn = itemBase.Pn,
            itemBase.IdRasp,
            itemBase.NumeroRasp,
            itemBase.Fornecedor,
            itemBase.MtResponsavel,
            itemBase.Turno,

            // -----------------------------------------------------
            // CONTROLE DA ENTRADA EM SELEÇÃO
            // -----------------------------------------------------
            DataEntradaSelecao = dataEntradaSelecao,
            DuracaoEmSelecaoDias = duracaoEmSelecaoDias,
            DiasSemApontamento = diasSemApontamento,

            // -----------------------------------------------------
            // ÚLTIMO APONTAMENTO OPERACIONAL
            // -----------------------------------------------------
            IdRaspContencao = ultimo?.IdRaspContencao,
            DataApontamento = dataUltimoApontamento.HasValue
                ? dataUltimoApontamento.Value.Date
                : (DateTime?)null,
            HoraInicio = ultimo?.DataHoraInicioAtividade,
            HoraFim = ultimo?.DataHoraFimAtividade,
            ultimo?.OrigemRegistro,
            ultimo?.QuantidadeVerificada,
            ultimo?.QuantidadeRejeitada,
            ultimo?.QuantidadeOk,
            ultimo?.Observacao,

            // -----------------------------------------------------
            // EXECUTOR TERCEIRO, QUANDO HOUVER
            // -----------------------------------------------------
            ultimo?.IdOperadorSelecaoTerceiro,
            NomeOperadorSelecaoTerceiro = nomeOperador,
            EmpresaOperadorSelecaoTerceiro = empresaOperador,

            // -----------------------------------------------------
            // STATUS FIXO DA VISÃO OPERACIONAL
            // -----------------------------------------------------
            Status = "Em seleção",

            // Trava operacional comum
            Trava = itemBase.TravaAtiva ? "Ativa" : "Sem trava",

            // QHD separado da trava
            QhdAtivo = itemBase.QhdAtivo,
            Qhd = itemBase.QhdAtivo ? "Sim" : "Não",
            DataHoraQhd = itemBase.DataHoraQhd

        };
    })
    .OrderBy(x => x.QhdAtivo ? 1:0)
    .ThenBy(x => x.NumeroRasp)
    .ThenBy(x => x.Pn)
    .ToList();

    return Results.Ok(resultado);
})
.WithName("ListarItensAtivosSelecaoOperacional")
.WithTags("RASP Seleção");

// -----------------------------------------------------------------------------
// 18B. SELECAO OPERACIONAL - HISTÓRICO GERAL
// -----------------------------------------------------------------------------
app.MapGet("/selecao-operacional/historico-geral", async (RaspDbContext db) =>
{
    var hoje = DateTime.UtcNow.Date;

    var itensBase = await (
        from rp in db.RaspPn.AsNoTracking()
        join r in db.Rasp.AsNoTracking()
            on rp.IdRasp equals r.IdRasp
        join f in db.FornecedorRasp.AsNoTracking()
            on r.IdFornecedorRasp equals f.IdFornecedor
        join mtJoin in db.Usuarios.AsNoTracking()
            on r.IdAnalistaMt equals mtJoin.IdUsuario into mtLeft
        from mt in mtLeft.DefaultIfEmpty()
        join turnoJoin in db.TurnoRasp.AsNoTracking()
            on r.IdTurnoRasp equals turnoJoin.IdTurnoRasp into turnoLeft
        from turno in turnoLeft.DefaultIfEmpty()
        where rp.EntrouSelecao == true
        select new
        {
            rp.IdRaspPn,
            Pn = rp.Pn,
            rp.IdRasp,
            r.NumeroRasp,
            Fornecedor = f.Nome,
            MtResponsavel = mt != null ? mt.Nome : null,
            Turno = turno != null ? turno.Descricao : null,
            rp.StatusSelecao,
            rp.EntrouSelecao,
            rp.DataHoraEntradaSelecao,
            rp.DataHoraSaidaSelecao,
            rp.TravaAtiva,
            rp.DataHoraSolicitacaoTrava,
            rp.DataHoraRemocaoTrava,
            rp.QhdAtivo,
            rp.DataHoraQhd
        }
    )
    .OrderByDescending(x => x.DataHoraEntradaSelecao)
    .ThenBy(x => x.NumeroRasp)
    .ThenBy(x => x.Pn)
    .ToListAsync();

    var itens = itensBase.Select(x =>
    {
        int? duracaoTotalDias = null;

        if (x.DataHoraEntradaSelecao.HasValue)
        {
            var dataFim = x.DataHoraSaidaSelecao.HasValue
                ? x.DataHoraSaidaSelecao.Value.Date
                : hoje;

            duracaoTotalDias = (dataFim - x.DataHoraEntradaSelecao.Value.Date).Days;

            if (duracaoTotalDias < 0)
                duracaoTotalDias = 0;
        }

        return new
        {
            x.IdRaspPn,
            x.Pn,
            x.IdRasp,
            x.NumeroRasp,
            x.Fornecedor,
            x.MtResponsavel,
            x.Turno,
            x.StatusSelecao,
            x.EntrouSelecao,
            x.DataHoraEntradaSelecao,
            x.DataHoraSaidaSelecao,
            x.TravaAtiva,
            x.DataHoraSolicitacaoTrava,
            x.DataHoraRemocaoTrava,
            x.QhdAtivo,
            x.DataHoraQhd,
            StatusDescricao =
                x.StatusSelecao == 1
                    ? "Em seleção"
                    : x.StatusSelecao == 2
                        ? "Encerrado"
                        : "Fora da seleção",
            DuracaoTotalDias = duracaoTotalDias
        };
    }).ToList();

    return Results.Ok(itens);
})
.WithName("HistoricoGeralSelecaoOperacional")
.WithTags("RASP Seleção");





// -----------------------------------------------------------------------------
// 18A. RASP CONTENCAO - LEITURA
// Retorno limpo:
// - evita expor campos legados/confusos
// - mantém somente os campos válidos do modelo atual
// -----------------------------------------------------------------------------
app.MapGet("/rasp-contencao", async (RaspDbContext db) =>
{
    var itens = await db.Set<RaspContencao>()
        .OrderByDescending(x => x.IdRaspContencao)
        .Select(x => new
        {
            x.IdRaspContencao,
            x.IdRaspPn,
            x.DataLoteVerificada,
            x.QuantidadeVerificada,
            x.QuantidadeRejeitada,
            x.QuantidadeOk,
            x.Observacao,
            x.DataAtualizacao,
            x.IdUsuarioExecucao,
            x.IdOperadorSelecaoTerceiro,
            x.OrigemRegistro,
            x.IdTurnoRasp,
            x.DataHoraInicioAtividade,
            x.DataHoraFimAtividade
        })
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspContencao")
.WithTags("RASP Seleção");

app.MapGet("/rasp-contencao/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.Set<RaspContencao>()
        .Where(x => x.IdRaspContencao == id)
        .Select(x => new
        {
            x.IdRaspContencao,
            x.IdRaspPn,
            x.DataLoteVerificada,
            x.QuantidadeVerificada,
            x.QuantidadeRejeitada,
            x.QuantidadeOk,
            x.Observacao,
            x.DataAtualizacao,
            x.IdUsuarioExecucao,
            x.IdOperadorSelecaoTerceiro,
            x.OrigemRegistro,
            x.IdTurnoRasp,
            x.DataHoraInicioAtividade,
            x.DataHoraFimAtividade
        })
        .FirstOrDefaultAsync();

    return item is null
        ? Results.NotFound("Apontamento de contenção não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterRaspContencaoPorId")
.WithTags("RASP Seleção");

app.MapGet("/rasp-pn/{idRaspPn:int}/contencoes", async (int idRaspPn, RaspDbContext db) =>
{
    if (idRaspPn <= 0)
        return Results.BadRequest("IdRaspPn inválido.");

    var raspPnExiste = await db.RaspPn.AnyAsync(x => x.IdRaspPn == idRaspPn);
    if (!raspPnExiste)
        return Results.NotFound("RASP_PN não encontrado.");

    var itens = await db.Set<RaspContencao>()
        .Where(x => x.IdRaspPn == idRaspPn)
        .OrderByDescending(x => x.DataAtualizacao)
        .Select(x => new
        {
            x.IdRaspContencao,
            x.IdRaspPn,
            x.DataLoteVerificada,
            x.QuantidadeVerificada,
            x.QuantidadeRejeitada,
            x.QuantidadeOk,
            x.Observacao,
            x.DataAtualizacao,
            x.IdUsuarioExecucao,
            x.IdOperadorSelecaoTerceiro,
            x.OrigemRegistro,
            x.IdTurnoRasp,
            x.DataHoraInicioAtividade,
            x.DataHoraFimAtividade
        })
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarContencoesPorRaspPn")
.WithTags("RASP Seleção");

// -----------------------------------------------------------------------------
// 18C. SELEÇÃO OPERACIONAL - VALIDAÇÃO DE TERCEIRO
// -----------------------------------------------------------------------------
app.MapPost("/selecao-operacional/validar-terceiro", async (
    ValidarTerceiroSelecaoRequest req,
    RaspDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.Gmid))
        return Results.BadRequest("GMID é obrigatório.");

    if (string.IsNullOrWhiteSpace(req.Senha))
        return Results.BadRequest("Senha é obrigatória.");

    var gmid = req.Gmid.Trim().ToLower();

    var usuario = await db.UsuariosTerceiros
        .FirstOrDefaultAsync(u =>
            u.Gmid.ToLower() == gmid &&
            u.Ativo);

    if (usuario is null)
        return Results.BadRequest("Usuário terceiro não encontrado ou inativo.");

    var senhaValida = SenhaService.ValidarSenha(
        req.Senha,
        usuario.SenhaHash
    );

    if (!senhaValida)
        return Results.BadRequest("GMID ou senha inválidos.");

    return Results.Ok(new
    {
        sucesso = true,
        mensagem = "Terceiro validado com sucesso.",
        idUsuarioTerceiro = usuario.IdUsuarioTerceiro,
        nomeCompleto = $"{usuario.Nome} {usuario.Sobrenome}".Trim(),
        gmid = usuario.Gmid,
        idEmpresaSelecao = usuario.IdEmpresaSelecao,
        validadeMinutos = 3
    });
})
.WithName("ValidarTerceiroSelecaoOperacional")
.WithTags("RASP Seleção");


// -----------------------------------------------------------------------------
// 18D. SELEÇÃO OPERACIONAL - VALIDAÇÃO DE EXECUTOR
// Valida tanto terceiro quanto usuário GM para apontamento operacional.
// -----------------------------------------------------------------------------
app.MapPost("/selecao-operacional/validar-executor", async (
    ValidarExecutorSelecaoRequest req,
    RaspDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.TipoExecutor))
        return Results.BadRequest("TipoExecutor é obrigatório.");

    if (string.IsNullOrWhiteSpace(req.Login))
        return Results.BadRequest("Login é obrigatório.");

    if (string.IsNullOrWhiteSpace(req.Senha))
        return Results.BadRequest("Senha é obrigatória.");

    var tipoExecutor = req.TipoExecutor.Trim().ToUpperInvariant();
    var login = req.Login.Trim().ToLower();

    // -------------------------------------------------------------------------
    // TERCEIRO
    // -------------------------------------------------------------------------
    if (tipoExecutor == "TERCEIRO")
    {
        var usuarioTerceiro = await db.UsuariosTerceiros
            .FirstOrDefaultAsync(u =>
                u.Gmid.ToLower() == login &&
                u.Ativo);

        if (usuarioTerceiro is null)
            return Results.BadRequest("Terceiro não encontrado ou inativo.");

        var senhaValida = SenhaService.ValidarSenha(
            req.Senha,
            usuarioTerceiro.SenhaHash
        );

        if (!senhaValida)
            return Results.BadRequest("Login ou senha inválidos.");

        return Results.Ok(new
        {
            sucesso = true,
            tipoExecutor = "TERCEIRO",
            idUsuarioTerceiro = usuarioTerceiro.IdUsuarioTerceiro,
            idUsuarioInterno = (int?)null,
            nomeCompleto = $"{usuarioTerceiro.Nome} {usuarioTerceiro.Sobrenome}".Trim(),
            login = usuarioTerceiro.Gmid,
            origemRegistro = 1,
            validadeMinutos = 3
        });
    }

    // -------------------------------------------------------------------------
    // GM INTERNO
    // -------------------------------------------------------------------------
    if (tipoExecutor == "GM")
    {
        var usuarioGm = await db.Usuarios
            .FirstOrDefaultAsync(u =>
                u.Ativo &&
                u.Gmin.ToLower() == login);

        if (usuarioGm is null)
            return Results.BadRequest("Usuário GM não encontrado ou inativo.");

        if (string.IsNullOrWhiteSpace(usuarioGm.SenhaHash))
            return Results.BadRequest("Usuário GM sem senha cadastrada.");

        var senhaValida = SenhaService.ValidarSenha(
            req.Senha,
            usuarioGm.SenhaHash
        );

        if (!senhaValida)
            return Results.BadRequest("Login ou senha inválidos.");

        return Results.Ok(new
        {
            sucesso = true,
            tipoExecutor = "GM",
            idUsuarioTerceiro = (int?)null,
            idUsuarioInterno = usuarioGm.IdUsuario,
            nomeCompleto = $"{usuarioGm.Nome} {usuarioGm.Sobrenome}".Trim(),
            login = usuarioGm.Gmin,
            origemRegistro = 2,
            validadeMinutos = 3
        });
    }

    return Results.BadRequest("TipoExecutor inválido. Use TERCEIRO ou GM.");
})
.WithName("ValidarExecutorSelecaoOperacional")
.WithTags("RASP Seleção");


// -----------------------------------------------------------------------------
// 18A. RASP CONTENCAO
// Finalidade:
// - registrar um apontamento operacional dentro de uma seleção já aberta
// - NÃO abre seleção
// - NÃO fecha seleção
// - NÃO decide trava
//
// Regras principais:
// - valida vínculo com RASP_PN
// - valida turno
// - valida origem do registro (terceiro ou usuário interno)
// - valida quantidades
// - valida coerência entre início e fim da atividade real
// - grava data/hora de auditoria no sistema
// -----------------------------------------------------------------------------
app.MapPost("/rasp-contencao", async (
    CriarRaspContencaoRequest req,
    RaspDbContext db) =>
{
    // -------------------------------------------------------------------------
    // 01. VALIDAÇÕES BÁSICAS
    // -------------------------------------------------------------------------
    if (req.IdRaspPn <= 0)
        return Results.BadRequest("IdRaspPn inválido.");

    if (req.IdTurnoRasp <= 0)
        return Results.BadRequest("IdTurnoRasp inválido.");

    if (req.QuantidadeVerificada < 0)
        return Results.BadRequest("QuantidadeVerificada não pode ser negativa.");

    if (req.QuantidadeRejeitada < 0)
        return Results.BadRequest("QuantidadeRejeitada não pode ser negativa.");

    if (req.QuantidadeRejeitada > req.QuantidadeVerificada)
        return Results.BadRequest("QuantidadeRejeitada não pode ser maior que QuantidadeVerificada.");

    if (req.OrigemRegistro != 1 && req.OrigemRegistro != 2)
        return Results.BadRequest("OrigemRegistro deve ser 1 (terceiro) ou 2 (usuário interno).");

    // -------------------------------------------------------------------------
    // 02. VALIDAÇÃO DA ATIVIDADE REAL
    // Observação:
    // - estes campos representam o início e o fim do trabalho executado
    // - NÃO representam entrada/saída da seleção macro do PN
    // -------------------------------------------------------------------------
    if (req.DataHoraInicioAtividade.HasValue && req.DataHoraFimAtividade.HasValue)
    {
        if (req.DataHoraInicioAtividade.Value > req.DataHoraFimAtividade.Value)
            return Results.BadRequest(
                "DataHoraInicioAtividade não pode ser maior que DataHoraFimAtividade.");
    }

    // -------------------------------------------------------------------------
    // 03. VALIDAÇÃO DO RASP_PN
    // -------------------------------------------------------------------------
    var raspPn = await db.RaspPn
        .FirstOrDefaultAsync(rp => rp.IdRaspPn == req.IdRaspPn);

    if (raspPn is null)
        return Results.BadRequest("RaspPn não encontrado.");

    // -------------------------------------------------------------------------
    // 03.1. DEFINE A DATA REAL DA SELEÇÃO
    // Regra:
    // - usa a data da atividade executada
    // - NÃO usa DataLoteVerificada
    // -------------------------------------------------------------------------
    DateOnly dataSelecao;

    if (req.DataHoraInicioAtividade.HasValue)
    {
        dataSelecao = DateOnly.FromDateTime(
            req.DataHoraInicioAtividade.Value);
    }
    else
    {
        dataSelecao = DateOnly.FromDateTime(DateTime.UtcNow);
    }


    // -------------------------------------------------------------------------
    // 03.2. VALIDA SE O APONTAMENTO NÃO É ANTERIOR
    // À ENTRADA OFICIAL EM SELEÇÃO DO PN
    // -------------------------------------------------------------------------
    if (raspPn.DataHoraEntradaSelecao.HasValue)
    {
        var dataEntradaSelecaoPn =
            DateOnly.FromDateTime(
                raspPn.DataHoraEntradaSelecao.Value);

        if (dataSelecao < dataEntradaSelecaoPn)
        {
            return Results.BadRequest(
                $"A data da seleção ({dataSelecao:dd/MM/yyyy}) não pode ser anterior à entrada oficial do item em seleção ({dataEntradaSelecaoPn:dd/MM/yyyy}).");
        }
    }


    // -------------------------------------------------------------------------
    // 04. VALIDAÇÃO DO TURNO
    // -------------------------------------------------------------------------
    var turnoExiste = await db.TurnoRasp
        .AnyAsync(t => t.IdTurnoRasp == req.IdTurnoRasp);

    if (!turnoExiste)
        return Results.BadRequest("Turno informado não existe.");

    // -------------------------------------------------------------------------
    // 05. VALIDAÇÃO DA ORIGEM DO REGISTRO
    // OrigemRegistro = 1 -> terceiro
    // OrigemRegistro = 2 -> usuário interno
    // -------------------------------------------------------------------------
    
    // -------------------------------------------------------------------------
// 05. VALIDAÇÃO DA ORIGEM DO REGISTRO
// OrigemRegistro = 1 -> terceiro validado por credencial rápida
// OrigemRegistro = 2 -> usuário interno
// -------------------------------------------------------------------------
if (req.OrigemRegistro == 1)
{
    if (!req.IdUsuarioTerceiro.HasValue || req.IdUsuarioTerceiro.Value <= 0)
        return Results.BadRequest("IdUsuarioTerceiro é obrigatório quando OrigemRegistro = 1.");

    if (req.IdUsuarioInterno.HasValue)
        return Results.BadRequest("IdUsuarioInterno deve ficar vazio quando OrigemRegistro = 1.");

    if (req.IdOperadorSelecaoTerceiro.HasValue)
        return Results.BadRequest("IdOperadorSelecaoTerceiro não deve mais ser usado para novos apontamentos.");

    var usuarioTerceiro = await db.UsuariosTerceiros
        .FirstOrDefaultAsync(u =>
            u.IdUsuarioTerceiro == req.IdUsuarioTerceiro.Value &&
            u.Ativo);

    if (usuarioTerceiro is null)
        return Results.BadRequest("Usuário terceiro não encontrado ou inativo.");

    var empresaAtiva = await db.EmpresaSelecaoRasp
        .AnyAsync(e =>
            e.IdEmpresaSelecao == usuarioTerceiro.IdEmpresaSelecao &&
            e.Ativo);

    if (!empresaAtiva)
        return Results.BadRequest("Empresa de seleção do terceiro está inativa ou não encontrada.");
}
else
{
    if (!req.IdUsuarioInterno.HasValue || req.IdUsuarioInterno.Value <= 0)
        return Results.BadRequest("IdUsuarioInterno é obrigatório quando OrigemRegistro = 2.");

    if (req.IdUsuarioTerceiro.HasValue)
        return Results.BadRequest("IdUsuarioTerceiro deve ficar vazio quando OrigemRegistro = 2.");

    if (req.IdOperadorSelecaoTerceiro.HasValue)
        return Results.BadRequest("IdOperadorSelecaoTerceiro deve ficar vazio quando OrigemRegistro = 2.");

    var usuarioExiste = await db.Usuarios
        .AnyAsync(u =>
            u.IdUsuario == req.IdUsuarioInterno.Value &&
            u.Ativo);

    if (!usuarioExiste)
        return Results.BadRequest("Usuário interno não encontrado ou inativo.");
}

    // -------------------------------------------------------------------------
    // 06. CÁLCULO DA QUANTIDADE OK
    // -------------------------------------------------------------------------
    var quantidadeOk = req.QuantidadeVerificada - req.QuantidadeRejeitada;

// -------------------------------------------------------------------------
// 07. MONTAGEM DO REGISTRO DE CONTENÇÃO
// Importante:
// - DataHoraInicioAtividade e DataHoraFimAtividade = atividade real
// - DataAtualizacao = momento do registro no sistema (auditoria)
// - PostgreSQL com "timestamp with time zone" exige DateTime em UTC
// -------------------------------------------------------------------------

DateTime? dataHoraInicioUtc = null;
if (req.DataHoraInicioAtividade.HasValue)
{
    var inicio = req.DataHoraInicioAtividade.Value;

    dataHoraInicioUtc = inicio.Kind == DateTimeKind.Utc
        ? inicio
        : inicio.Kind == DateTimeKind.Local
            ? inicio.ToUniversalTime()
            : DateTime.SpecifyKind(inicio, DateTimeKind.Utc);
}

DateTime? dataHoraFimUtc = null;
if (req.DataHoraFimAtividade.HasValue)
{
    var fim = req.DataHoraFimAtividade.Value;

    dataHoraFimUtc = fim.Kind == DateTimeKind.Utc
        ? fim
        : fim.Kind == DateTimeKind.Local
            ? fim.ToUniversalTime()
            : DateTime.SpecifyKind(fim, DateTimeKind.Utc);
}

var item = new RaspContencao
{
    IdRaspPn = req.IdRaspPn,
    DataSelecao = dataSelecao,
    DataLoteVerificada = req.DataLoteVerificada,
    QuantidadeVerificada = req.QuantidadeVerificada,
    QuantidadeRejeitada = req.QuantidadeRejeitada,
    QuantidadeOk = quantidadeOk,
    Observacao = string.IsNullOrWhiteSpace(req.Observacao) ? null : req.Observacao.Trim(),
    IdTurnoRasp = req.IdTurnoRasp,
    IdOperadorSelecaoTerceiro = req.IdOperadorSelecaoTerceiro,
    IdUsuarioTerceiro = req.IdUsuarioTerceiro,
    IdUsuarioExecucao = req.IdUsuarioInterno,
    OrigemRegistro = req.OrigemRegistro,
    

    // NOVOS CAMPOS DA ATIVIDADE REAL
    DataHoraInicioAtividade = dataHoraInicioUtc,
    DataHoraFimAtividade = dataHoraFimUtc,

    // AUDITORIA DO SISTEMA
    DataAtualizacao = DateTime.UtcNow
};



    // -------------------------------------------------------------------------
    // 08. GRAVAÇÃO
    // -------------------------------------------------------------------------
    db.Set<RaspContencao>().Add(item);
    await db.SaveChangesAsync();

    // -------------------------------------------------------------------------
    // 09. RETORNO
    // -------------------------------------------------------------------------
    return Results.Created($"/rasp-contencao/{item.IdRaspContencao}", item);
})
.WithName("CriarRaspContencao")
.WithTags("RASP Seleção");




// -----------------------------------------------------------------------------
// 19. RASP ARQUIVO
// -----------------------------------------------------------------------------

app.MapGet("/rasp-arquivo", async (RaspDbContext db) =>
{
    var itens = await db.RaspArquivo
        .OrderBy(a => a.IdArquivoRasp)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspArquivo");

app.MapGet("/rasp-arquivo/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.RaspArquivo
        .FirstOrDefaultAsync(a => a.IdArquivoRasp == id);

    return item is null
        ? Results.NotFound($"Arquivo do RASP com id {id} não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterRaspArquivoPorId");

app.MapGet("/rasp/{id:int}/arquivos", async (int id, RaspDbContext db) =>
{
    var raspExiste = await db.Rasp.AnyAsync(r => r.IdRasp == id);

    if (!raspExiste)
        return Results.NotFound($"RASP com id {id} não encontrado.");

    var itens = await db.RaspArquivo
        .Where(a => a.IdRasp == id)
        .OrderBy(a => a.IdArquivoRasp)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarArquivosPorRasp");

app.MapPost("/rasp-arquivo", async (CriarRaspArquivoRequest req, RaspDbContext db) =>
{
    if (req.IdRasp <= 0)
        return Results.BadRequest("IdRasp é obrigatório.");

    if (string.IsNullOrWhiteSpace(req.TipoArquivo))
        return Results.BadRequest("TipoArquivo é obrigatório.");

    if (string.IsNullOrWhiteSpace(req.CaminhoArquivo))
        return Results.BadRequest("CaminhoArquivo é obrigatório.");

    if (req.IdUsuarioUpload <= 0)
        return Results.BadRequest("IdUsuarioUpload é obrigatório.");

    var raspExiste = await db.Rasp.AnyAsync(r => r.IdRasp == req.IdRasp);
    if (!raspExiste)
        return Results.BadRequest($"RASP com id {req.IdRasp} não encontrado.");

    var usuarioExiste = await db.Usuarios.AnyAsync(u => u.IdUsuario == req.IdUsuarioUpload);
    if (!usuarioExiste)
        return Results.BadRequest($"Usuário com id {req.IdUsuarioUpload} não encontrado.");

    var item = new RaspArquivo
    {
        IdRasp = req.IdRasp,
        TipoArquivo = req.TipoArquivo.Trim(),
        Descricao = string.IsNullOrWhiteSpace(req.Descricao) ? null : req.Descricao.Trim(),
        CaminhoArquivo = req.CaminhoArquivo.Trim(),
        DataUpload = DateOnly.FromDateTime(DateTime.Today),
        IdUsuarioUpload = req.IdUsuarioUpload
    };

    db.RaspArquivo.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"/rasp-arquivo/{item.IdArquivoRasp}", item);
})
.WithName("CriarRaspArquivo");

app.MapPut("/rasp-arquivo/{id:int}", async (int id, AtualizarRaspArquivoRequest req, RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do arquivo inválido.");

    var item = await db.RaspArquivo
        .FirstOrDefaultAsync(a => a.IdArquivoRasp == id);

    if (item is null)
        return Results.NotFound($"Arquivo do RASP com id {id} não encontrado.");

    if (string.IsNullOrWhiteSpace(req.TipoArquivo))
        return Results.BadRequest("TipoArquivo é obrigatório.");

    if (string.IsNullOrWhiteSpace(req.CaminhoArquivo))
        return Results.BadRequest("CaminhoArquivo é obrigatório.");

    item.TipoArquivo = req.TipoArquivo.Trim();
    item.Descricao = string.IsNullOrWhiteSpace(req.Descricao) ? null : req.Descricao.Trim();
    item.CaminhoArquivo = req.CaminhoArquivo.Trim();

    await db.SaveChangesAsync();

    return Results.Ok(item);
})
.WithName("AtualizarRaspArquivo");

app.MapDelete("/rasp-arquivo/{id:int}", async (int id, RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do arquivo inválido.");

    var item = await db.RaspArquivo
        .FirstOrDefaultAsync(a => a.IdArquivoRasp == id);

    if (item is null)
        return Results.NotFound($"Arquivo do RASP com id {id} não encontrado.");

    db.RaspArquivo.Remove(item);
    await db.SaveChangesAsync();

    return Results.Ok($"Arquivo do RASP com id {id} removido com sucesso.");
})
.WithName("ExcluirRaspArquivo");

// -----------------------------------------------------------------------------
// 20. RASP - ATUALIZAÇÃO DE RASCUNHO
//
// Atualiza um RASP ainda em rascunho sem perder os dados já salvos.
//
// Regras desta versão:
// - só atualiza se o RASP existir
// - só atualiza se ainda estiver em rascunho
// - só atualiza se estiver no status 1 (Em análise)
// - valida o usuário executor
// - ADMIN pode atualizar
// - não ADMIN só pode atualizar se for o autor MT do RASP
// - campos enviados como null não são alterados
// - campos string enviados como "" limpam o valor
// - campos FK opcionais enviados como 0 limpam o valor
// -----------------------------------------------------------------------------
app.MapPut("/rasp/{id:int}/rascunho", async (
    int id,
    AtualizarRaspRascunhoRequest req,
    RaspDbContext db) =>
{
    // -------------------------------------------------------------------------
    // 01. VALIDAÇÃO INICIAL DO RASP E DO EXECUTOR
    // -------------------------------------------------------------------------
    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    var rasp = await db.Rasp.FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp is null)
{
    return Results.NotFound(new
    {
        erro = "RASP não encontrado."
    });
}

if (rasp.IdStatusRasp != StatusRaspConstantes.AguardandoLg)
{
    return Results.BadRequest(new
    {
        erro = "Este RASP não está aguardando aprovação LG."
    });
}


    if (rasp is null)
        return Results.NotFound("RASP não encontrado.");

    if (!rasp.IsRascunho)
        return Results.BadRequest("Este RASP não está mais em rascunho.");

    if (rasp.IdStatusRasp != 1)
        return Results.BadRequest("Somente RASP em análise pode ser atualizado por esta rota.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor é obrigatório.");

    var usuarioExecutorRascunho = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor);

    if (usuarioExecutorRascunho is null)
        return Results.BadRequest("Usuário executor não encontrado.");

    var isAdminRascunho = usuarioExecutorRascunho.IdPerfil == 1;

    if (!isAdminRascunho)
    {
        if (!rasp.IdAnalistaMt.HasValue)
            return Results.BadRequest("Este RASP não possui autor MT vinculado.");

        if (rasp.IdAnalistaMt.Value != req.IdUsuarioExecutor)
            return Results.StatusCode(StatusCodes.Status403Forbidden);
    }

    bool alterou = false;

    // -------------------------------------------------------------------------
    // 02. FORNECEDOR (OBRIGATÓRIO NO REGISTRO, OPCIONAL NO UPDATE)
    // -------------------------------------------------------------------------
    if (req.IdFornecedorRasp.HasValue)
    {
        if (req.IdFornecedorRasp.Value <= 0)
            return Results.BadRequest("IdFornecedorRasp inválido.");

        var fornecedorExiste = await db.FornecedorRasp
            .AnyAsync(f => f.IdFornecedor == req.IdFornecedorRasp.Value);

        if (!fornecedorExiste)
            return Results.BadRequest("Fornecedor informado não existe.");

        rasp.IdFornecedorRasp = req.IdFornecedorRasp.Value;
        alterou = true;
    }

    // -------------------------------------------------------------------------
    // 03. RESUMO DA OCORRÊNCIA
    // -------------------------------------------------------------------------
    if (req.ResumoOcorrencia is not null)
    {
        var resumo = req.ResumoOcorrencia.Trim();

        rasp.ResumoOcorrencia = string.IsNullOrWhiteSpace(resumo)
            ? null
            : resumo;

        alterou = true;
    }

    // -------------------------------------------------------------------------
    // 04. DESCRIÇÃO PRINCIPAL
    // -------------------------------------------------------------------------
    if (req.DescricaoProblema is not null)
    {
        var descricao = req.DescricaoProblema.Trim();

        if (string.IsNullOrWhiteSpace(descricao))
            return Results.BadRequest("DescricaoProblema não pode ficar vazia.");

        rasp.DescricaoProblema = descricao;
        alterou = true;
    }

    // -------------------------------------------------------------------------
    // 05. DOMÍNIOS / FKs OPCIONAIS
    // null = não altera
    // 0    = limpa
    // >0   = valida e grava
    // -------------------------------------------------------------------------
    if (req.IdModeloVeiculoRasp.HasValue)
    {
        if (req.IdModeloVeiculoRasp.Value == 0)
        {
            rasp.IdModeloVeiculoRasp = null;
        }
        else
        {
            var existe = await db.ModeloVeiculoRasp
                .AnyAsync(x => x.IdModeloVeiculoRasp == req.IdModeloVeiculoRasp.Value);

            if (!existe)
                return Results.BadRequest("IdModeloVeiculoRasp não existe.");

            rasp.IdModeloVeiculoRasp = req.IdModeloVeiculoRasp.Value;
        }

        alterou = true;
    }

    if (req.IdSetorRasp.HasValue)
    {
        if (req.IdSetorRasp.Value == 0)
        {
            rasp.IdSetorRasp = null;
        }
        else
        {
            var existe = await db.SetorRasp
                .AnyAsync(x => x.IdSetorRasp == req.IdSetorRasp.Value);

            if (!existe)
                return Results.BadRequest("IdSetorRasp não existe.");

            rasp.IdSetorRasp = req.IdSetorRasp.Value;
        }

        alterou = true;
    }

    if (req.IdTurnoRasp.HasValue)
    {
        if (req.IdTurnoRasp.Value == 0)
        {
            rasp.IdTurnoRasp = null;
        }
        else
        {
            var existe = await db.TurnoRasp
                .AnyAsync(x => x.IdTurnoRasp == req.IdTurnoRasp.Value);

            if (!existe)
                return Results.BadRequest("IdTurnoRasp não existe.");

            rasp.IdTurnoRasp = req.IdTurnoRasp.Value;
        }

        alterou = true;
    }

    if (req.IdIndiceOperacionalRasp.HasValue)
    {
        if (req.IdIndiceOperacionalRasp.Value == 0)
        {
            rasp.IdIndiceOperacionalRasp = null;
        }
        else
        {
            var existe = await db.IndiceOperacionalRasp
                .AnyAsync(x => x.IdIndiceOperacionalRasp == req.IdIndiceOperacionalRasp.Value);

            if (!existe)
                return Results.BadRequest("IdIndiceOperacionalRasp não existe.");

            rasp.IdIndiceOperacionalRasp = req.IdIndiceOperacionalRasp.Value;
        }

        alterou = true;
    }

    if (req.IdPilotoRasp.HasValue)
    {
        if (req.IdPilotoRasp.Value == 0)
        {
            rasp.IdPilotoRasp = null;
        }
        else
        {
            var existe = await db.PilotoRasp
                .AnyAsync(x => x.IdPilotoRasp == req.IdPilotoRasp.Value);

            if (!existe)
                return Results.BadRequest("IdPilotoRasp não existe.");

            rasp.IdPilotoRasp = req.IdPilotoRasp.Value;
        }

        alterou = true;
    }

    if (req.IdImpactoClienteRasp.HasValue)
    {
        if (req.IdImpactoClienteRasp.Value == 0)
        {
            rasp.IdImpactoClienteRasp = null;
        }
        else
        {
            var existe = await db.ImpactoClienteRasp
                .AnyAsync(x => x.IdImpactoCliente == req.IdImpactoClienteRasp.Value);

            if (!existe)
                return Results.BadRequest("IdImpactoClienteRasp não existe.");

            rasp.IdImpactoClienteRasp = req.IdImpactoClienteRasp.Value;
        }

        alterou = true;
    }

    if (req.IdImpactoQualidadeRasp.HasValue)
    {
        if (req.IdImpactoQualidadeRasp.Value == 0)
        {
            rasp.IdImpactoQualidadeRasp = null;
        }
        else
        {
            var existe = await db.ImpactoQualidadeRasp
                .AnyAsync(x => x.IdImpactoQualidadeRasp == req.IdImpactoQualidadeRasp.Value);

            if (!existe)
                return Results.BadRequest("IdImpactoQualidadeRasp não existe.");

            rasp.IdImpactoQualidadeRasp = req.IdImpactoQualidadeRasp.Value;
        }

        alterou = true;
    }

    if (req.IdMaiorImpactoRasp.HasValue)
    {
        if (req.IdMaiorImpactoRasp.Value == 0)
        {
            rasp.IdMaiorImpactoRasp = null;
        }
        else
        {
            var existe = await db.MaiorImpactoRasp
                .AnyAsync(x => x.IdMaiorImpactoRasp == req.IdMaiorImpactoRasp.Value);

            if (!existe)
                return Results.BadRequest("IdMaiorImpactoRasp não existe.");

            rasp.IdMaiorImpactoRasp = req.IdMaiorImpactoRasp.Value;
        }

        alterou = true;
    }

    if (req.IdMajorRasp.HasValue)
    {
        if (req.IdMajorRasp.Value == 0)
        {
            rasp.IdMajorRasp = null;
        }
        else
        {
            var existe = await db.MajorRasp
                .AnyAsync(x => x.IdMajorRasp == req.IdMajorRasp.Value);

            if (!existe)
                return Results.BadRequest("IdMajorRasp não existe.");

            rasp.IdMajorRasp = req.IdMajorRasp.Value;
        }

        alterou = true;
    }

    if (req.IdSppsClassificacaoRasp.HasValue)
    {
        if (req.IdSppsClassificacaoRasp.Value == 0)
        {
            rasp.IdSppsClassificacaoRasp = null;
        }
        else
        {
            var existe = await db.SppsClassificacaoRasp
                .AnyAsync(x => x.IdSppsClassificacaoRasp == req.IdSppsClassificacaoRasp.Value);

            if (!existe)
                return Results.BadRequest("IdSppsClassificacaoRasp não existe.");

            rasp.IdSppsClassificacaoRasp = req.IdSppsClassificacaoRasp.Value;
        }

        alterou = true;
    }

    if (req.IdSppsStatusRasp.HasValue)
    {
        if (req.IdSppsStatusRasp.Value == 0)
        {
            rasp.IdSppsStatusRasp = null;
        }
        else
        {
            var existe = await db.SppsStatusRasp
                .AnyAsync(x => x.IdSppsStatusRasp == req.IdSppsStatusRasp.Value);

            if (!existe)
                return Results.BadRequest("IdSppsStatusRasp não existe.");

            rasp.IdSppsStatusRasp = req.IdSppsStatusRasp.Value;
        }

        alterou = true;
    }

    if (req.IdEmpresaSelecaoRasp.HasValue)
    {
        if (req.IdEmpresaSelecaoRasp.Value == 0)
        {
            rasp.IdEmpresaSelecaoRasp = null;
        }
        else
        {
            var existe = await db.EmpresaSelecaoRasp
                .AnyAsync(x => x.IdEmpresaSelecao == req.IdEmpresaSelecaoRasp.Value);

            if (!existe)
                return Results.BadRequest("IdEmpresaSelecaoRasp não existe.");

            rasp.IdEmpresaSelecaoRasp = req.IdEmpresaSelecaoRasp.Value;
        }

        alterou = true;
    }

    if (req.IdContaCrRasp.HasValue)
    {
        if (req.IdContaCrRasp.Value == 0)
        {
            rasp.IdContaCrRasp = null;
        }
        else
        {
            var existe = await db.ContaCrRasp
                .AnyAsync(x => x.IdContaCr == req.IdContaCrRasp.Value);

            if (!existe)
                return Results.BadRequest("IdContaCrRasp não existe.");

            rasp.IdContaCrRasp = req.IdContaCrRasp.Value;
        }

        alterou = true;
    }

    if (req.IdContaCrSubcontaRasp.HasValue)
    {
        if (req.IdContaCrSubcontaRasp.Value == 0)
        {
            rasp.IdContaCrSubcontaRasp = null;
        }
        else
        {
            var existe = await db.ContaCrSubcontaRasp
                .AnyAsync(x => x.IdSubcontaCr == req.IdContaCrSubcontaRasp.Value);

            if (!existe)
                return Results.BadRequest("IdContaCrSubcontaRasp não existe.");

            rasp.IdContaCrSubcontaRasp = req.IdContaCrSubcontaRasp.Value;
        }

        alterou = true;
    }

    if (req.IdGmAliadoRasp.HasValue)
    {
        if (req.IdGmAliadoRasp.Value == 0)
        {
            rasp.IdGmAliadoRasp = null;
        }
        else
        {
            var existe = await db.GmAliadoRasp
                .AnyAsync(x => x.IdGmAliadoRasp == req.IdGmAliadoRasp.Value);

            if (!existe)
                return Results.BadRequest("IdGmAliadoRasp não existe.");

            rasp.IdGmAliadoRasp = req.IdGmAliadoRasp.Value;
        }

        alterou = true;
    }

    if (req.IdOrigemFabricacaoRasp.HasValue)
    {
        if (req.IdOrigemFabricacaoRasp.Value == 0)
        {
            rasp.IdOrigemFabricacaoRasp = null;
        }
        else
        {
            var existe = await db.OrigemFabricacaoRasp
                .AnyAsync(x => x.IdOrigemFabricacaoRasp == req.IdOrigemFabricacaoRasp.Value);

            if (!existe)
                return Results.BadRequest("IdOrigemFabricacaoRasp não existe.");

            rasp.IdOrigemFabricacaoRasp = req.IdOrigemFabricacaoRasp.Value;
        }

        alterou = true;
    }

    // -------------------------------------------------------------------------
    // 06. CAMPOS LIVRES DE CONTATO / COMPLEMENTO
    // -------------------------------------------------------------------------
    if (req.RdNumero is not null)
    {
        var valor = req.RdNumero.Trim();
        rasp.RdNumero = string.IsNullOrWhiteSpace(valor) ? null : valor;
        alterou = true;
    }

    if (req.CampanhaNumero is not null)
    {
        var valor = req.CampanhaNumero.Trim();
        rasp.CampanhaNumero = string.IsNullOrWhiteSpace(valor) ? null : valor;
        alterou = true;
    }

    if (req.NomeContato is not null)
    {
        var valor = req.NomeContato.Trim();
        rasp.NomeContato = string.IsNullOrWhiteSpace(valor) ? null : valor;
        alterou = true;
    }

    if (req.DataContato.HasValue)
    {
        rasp.DataContato = req.DataContato.Value;
        alterou = true;
    }

    // -------------------------------------------------------------------------
    // 07. APROVADOR FT
    // -------------------------------------------------------------------------
    if (req.IdAprovadorFt.HasValue || req.IdTurnoRasp.HasValue)
    {
        int? idAprovadorFtFinal = null;

        if (req.IdAprovadorFt.HasValue && req.IdAprovadorFt.Value > 0)
        {
            idAprovadorFtFinal = req.IdAprovadorFt.Value;
        }
        else if (req.IdTurnoRasp.HasValue && req.IdTurnoRasp.Value > 0)
        {
            idAprovadorFtFinal = await db.Usuarios
                .Where(u =>
                    u.Ativo &&
                    u.IdPerfil == 3 &&
                    u.IdTurnoRasp == req.IdTurnoRasp.Value)
                .Select(u => (int?)u.IdUsuario)
                .FirstOrDefaultAsync();
        }

        if (req.IdAprovadorFt.HasValue && req.IdAprovadorFt.Value == 0)
        {
            rasp.IdAprovadorFt = null;
            alterou = true;
        }
        else if (idAprovadorFtFinal.HasValue)
        {
            var ftExiste = await db.Usuarios.AnyAsync(u =>
                u.IdUsuario == idAprovadorFtFinal.Value &&
                u.Ativo &&
                u.IdPerfil == 3);

            if (!ftExiste)
                return Results.BadRequest("Aprovador FT inválido.");

            rasp.IdAprovadorFt = idAprovadorFtFinal.Value;
            alterou = true;
        }
    }

    // -------------------------------------------------------------------------
    // 08. CAMPOS STRING OPCIONAIS
    // -------------------------------------------------------------------------
    if (req.SppsNumero is not null)
    {
        var valor = req.SppsNumero.Trim();

        if (string.IsNullOrWhiteSpace(valor))
        {
            rasp.SppsNumero = null;
        }
        else
        {
            if (valor.Length != 6 || !valor.All(char.IsDigit))
                return Results.BadRequest("SppsNumero deve conter exatamente 6 dígitos numéricos.");

            rasp.SppsNumero = valor;
        }

        alterou = true;
    }

    if (req.Procedencia is not null)
    {
        var valor = req.Procedencia.Trim();
        rasp.Procedencia = string.IsNullOrWhiteSpace(valor) ? null : valor;
        alterou = true;
    }

    if (req.BpTexto is not null)
    {
        var valor = req.BpTexto.Trim();
        rasp.BpTexto = string.IsNullOrWhiteSpace(valor) ? null : valor;
        alterou = true;
    }

    if (req.BpSerie is not null)
    {
        var valor = req.BpSerie.Trim();

        if (string.IsNullOrWhiteSpace(valor))
        {
            rasp.BpSerie = null;
        }
        else
        {
            if (valor.Length != 17)
                return Results.BadRequest("BpSerie deve ter exatamente 17 caracteres.");

            rasp.BpSerie = valor;
        }

        alterou = true;
    }

    if (req.BreakpointTexto is not null)
    {
        var valor = req.BreakpointTexto.Trim();
        rasp.BreakpointTexto = string.IsNullOrWhiteSpace(valor) ? null : valor;
        alterou = true;
    }

    if (req.BreakpointCodigo is not null)
    {
        var valor = req.BreakpointCodigo.Trim();
        rasp.BreakpointCodigo = string.IsNullOrWhiteSpace(valor) ? null : valor;
        alterou = true;
    }

    if (req.ObservacaoGeral is not null)
    {
        var valor = req.ObservacaoGeral.Trim();
        rasp.ObservacaoGeral = string.IsNullOrWhiteSpace(valor) ? null : valor;
        alterou = true;
    }

    // -------------------------------------------------------------------------
    // 09. DATAS OPCIONAIS
    // -------------------------------------------------------------------------
    if (req.BpDatahora.HasValue)
    {
        rasp.BpDatahora = req.BpDatahora.Value;
        alterou = true;
    }

    if (req.BreakpointDatahora.HasValue)
    {
        rasp.BreakpointDatahora = req.BreakpointDatahora.Value;
        alterou = true;
    }

    // -------------------------------------------------------------------------
    // 10. FLAGS OPCIONAIS
    // -------------------------------------------------------------------------
    if (req.IniciativaFornecedor.HasValue)
    {
        rasp.IniciativaFornecedor = req.IniciativaFornecedor.Value;
        alterou = true;
    }

    if (req.SupplierAlert.HasValue)
    {
        rasp.SupplierAlert = req.SupplierAlert.Value;
        alterou = true;
    }

    if (req.Reversao.HasValue)
    {
        rasp.Reversao = req.Reversao.Value;
        alterou = true;
    }

    if (req.Safety.HasValue)
    {
        rasp.Safety = req.Safety.Value;
        alterou = true;
    }

    if (req.EmitiuPrr.HasValue)
    {
        rasp.EmitiuPrr = req.EmitiuPrr.Value;
        alterou = true;
    }

    if (req.AprovadoLg.HasValue)
    {
        rasp.AprovadoLg = req.AprovadoLg.Value;
        alterou = true;
    }

    if (req.IsSupplierAlert.HasValue)
    {
        rasp.IsSupplierAlert = req.IsSupplierAlert.Value;
        alterou = true;
    }

    if (req.IsSafety.HasValue)
    {
        rasp.IsSafety = req.IsSafety.Value;
        alterou = true;
    }

    if (req.IsReversao.HasValue)
    {
        rasp.IsReversao = req.IsReversao.Value;
        alterou = true;
    }

    if (req.GeraPrr.HasValue)
    {
        rasp.GeraPrr = req.GeraPrr.Value;
        alterou = true;
    }

    // -------------------------------------------------------------------------
    // 11. REGRA AUTOMÁTICA DE APROVADOR FT PELO TURNO JÁ DEFINIDO NO RASP
    // -------------------------------------------------------------------------
    if (!rasp.IdAprovadorFt.HasValue && rasp.IdTurnoRasp.HasValue)
    {
        var ftDoTurno = await db.Usuarios
            .Where(u =>
                u.Ativo &&
                u.IdPerfil == 3 &&
                u.IdTurnoRasp == rasp.IdTurnoRasp.Value)
            .OrderBy(u => u.IdUsuario)
            .FirstOrDefaultAsync();

        if (ftDoTurno is not null)
        {
            rasp.IdAprovadorFt = ftDoTurno.IdUsuario;
            alterou = true;
        }
    }

    // -------------------------------------------------------------------------
    // 12. ENCERRAMENTO
    // -------------------------------------------------------------------------
    if (!alterou)
        return Results.BadRequest("Nenhum campo válido foi informado para atualização.");

    Console.WriteLine($"ResumoOcorrencia recebido: {req.ResumoOcorrencia}");
    Console.WriteLine($"IdOrigemFabricacaoRasp recebido: {req.IdOrigemFabricacaoRasp}");
    Console.WriteLine($"IdOrigemFabricacaoRasp no rasp antes de salvar: {rasp.IdOrigemFabricacaoRasp}");

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        id_rasp = rasp.IdRasp,
        atualizado = true
    });
})
.WithName("AtualizarRaspRascunho");

// -----------------------------------------------------------------------------
// 20A. RASP - SUBMETER PARA ANÁLISE FT
//
// Regra:
// - Rascunho NÃO pode ir para o FT incompleto.
// - Somente RASP com campos mínimos preenchidos pode ser submetido.
// - Ao submeter:
//   • is_rascunho = false
//   • id_status_rasp = 2
//   • data_envio_ft = agora
//   • percentual_completude = 100
// -----------------------------------------------------------------------------
app.MapPost("/rasp/{id:int}/submeter-ft", async (
    int id,
    AcaoFluxoRaspRequest req,
    RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    var rasp = await db.Rasp
        .FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp is null)
        return Results.NotFound("RASP não encontrado.");

    if (rasp.IdStatusRasp != 1)
        return Results.BadRequest("Somente RASP em preenchimento e ainda não submetido pode ser enviado ao FT.");

    if (!rasp.IsRascunho)
        return Results.BadRequest("Este RASP já foi submetido ao fluxo de aprovação e não pode ser reenviado ao FT por esta rota.");

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    var isAdmin = usuarioExecutor.IdPerfil == 1;

    if (!isAdmin)
    {
        if (!rasp.IdAnalistaMt.HasValue)
            return Results.BadRequest("Este RASP não possui MT responsável vinculado.");

        if (rasp.IdAnalistaMt.Value != req.IdUsuarioExecutor)
            return Results.StatusCode(StatusCodes.Status403Forbidden);
    }

    // -------------------------------------------------------------------------
    // VALIDAÇÃO DOS CAMPOS MÍNIMOS PARA SUBMISSÃO AO FT
    // -------------------------------------------------------------------------
    var pendencias = new List<string>();

// ------------------------------------------------------
// SEÇÕES 1 A 4 - OBRIGATÓRIAS
// Exceção: iniciativa_fornecedor não é obrigatória,
// pois representa uma condição.
// ------------------------------------------------------

if (rasp.IdFornecedorRasp <= 0)
    pendencias.Add("Fornecedor");

if (string.IsNullOrWhiteSpace(rasp.ResumoOcorrencia))
    pendencias.Add("Resumo da ocorrência");

if (string.IsNullOrWhiteSpace(rasp.DescricaoProblema))
    pendencias.Add("Descrição do problema");

if (!rasp.IdModeloVeiculoRasp.HasValue)
    pendencias.Add("Modelo de veículo");

if (!rasp.IdSetorRasp.HasValue)
    pendencias.Add("Setor");

if (!rasp.IdTurnoRasp.HasValue)
    pendencias.Add("Turno");

if (!rasp.IdOrigemFabricacaoRasp.HasValue)
    pendencias.Add("Origem de fabricação");

if (!rasp.IdPilotoRasp.HasValue)
    pendencias.Add("Piloto");

if (!rasp.IdImpactoClienteRasp.HasValue)
    pendencias.Add("Impacto cliente");

if (!rasp.IdImpactoQualidadeRasp.HasValue)
    pendencias.Add("Impacto qualidade");

if (!rasp.IdMaiorImpactoRasp.HasValue)
    pendencias.Add("Maior impacto");

if (!rasp.IdMajorRasp.HasValue)
    pendencias.Add("Major");

if (!rasp.IdIndiceOperacionalRasp.HasValue)
    pendencias.Add("Índice operacional");

if (!rasp.IdGmAliadoRasp.HasValue)
    pendencias.Add("GM aliado");

// ------------------------------------------------------
// SEÇÃO 5 - OBRIGATÓRIA COM EXCEÇÕES
// Exceções: RD Nº e Campanha Nº não são obrigatórios.
// ------------------------------------------------------

if (string.IsNullOrWhiteSpace(rasp.NomeContato))
    pendencias.Add("Nome da pessoa contatada");

if (!rasp.DataContato.HasValue)
    pendencias.Add("Data do contato");

// RD Nº opcional
// Campanha Nº opcional

// ------------------------------------------------------
// SEÇÃO 6 - OBRIGATÓRIA
// ------------------------------------------------------


if (!rasp.IdAprovadorFt.HasValue)
    pendencias.Add("Aprovador FT");

// ------------------------------------------------------
// PN obrigatório
// ------------------------------------------------------

var possuiPn = await db.RaspPn
    .AnyAsync(rp => rp.IdRasp == id);

    if (!possuiPn)
        pendencias.Add("Pelo menos um PN vinculado ao RASP");

    if (pendencias.Count > 0)
    {
        return Results.BadRequest(new
        {
            mensagem = "RASP ainda não possui os requisitos mínimos para submissão ao FT.",
            pendencias
        });
    }


    // -------------------------------------------------------------------------
    // SUBMISSÃO AO FT
    // -------------------------------------------------------------------------
    var statusAnterior = rasp.IdStatusRasp;
    rasp.IsRascunho = false;
    rasp.IdStatusRasp = StatusRaspConstantes.AguardandoFt;
    rasp.DataEnvioFt = DateTime.UtcNow;
    rasp.PercentualCompletude = 100;

    await db.SaveChangesAsync();

    await RegistrarHistoricoFluxoAsync(
    db,
    rasp.IdRasp,
    req.IdUsuarioExecutor,
    "SUBMETIDO_FT",
    statusAnterior,
    rasp.IdStatusRasp,
    "RASP submetido pelo MT para análise do FT.",
    "MT"
);


    return Results.Ok(new
    {
        mensagem = "RASP submetido ao FT com sucesso.",
        rasp.IdRasp,
        rasp.NumeroRasp,
        rasp.IdStatusRasp,
        rasp.IsRascunho,
        rasp.DataEnvioFt,
        rasp.IdAprovadorFt
    });
})
.WithName("SubmeterRaspAoFt");


// ==========================================================
// 20A. RASP - APROVAÇÃO FT
// ==========================================================
app.MapPost("/rasp/{id:int}/aprovar-ft", async (
    int id,
    AprovarFtRequest request,
    RaspDbContext db) =>
{
    var rasp = await db.Rasp
        .FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp == null)
    {
        return Results.NotFound(new
        {
            mensagem = "RASP não encontrado."
        });
    }

    if (rasp.IsRascunho)
    {
        return Results.BadRequest(new
        {
            mensagem = "Rascunho não pode ser aprovado pelo FT."
        });
    }

    if (string.IsNullOrWhiteSpace(request.ObservacaoFt))
    {
        return Results.BadRequest(new
        {
            mensagem = "Observação FT obrigatória."
        });
    }

    var usuario = await db.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u =>
            u.IdUsuario == request.IdUsuarioExecutor);

    if (usuario == null)
    {
        return Results.BadRequest(new
        {
            mensagem = "Usuário não encontrado."
        });
    }

    var perfilFt = usuario.IdPerfil == 3;
    var administrador = usuario.Administrador == true;

    if (!perfilFt && !administrador)
    {
        return Results.BadRequest(new
        {
            mensagem = "Acesso permitido somente para FT ou administrador."
        });
    }

    if (!administrador)
    {
        if (usuario.IdTurnoRasp != rasp.IdTurnoRasp)
        {
            return Results.BadRequest(new
            {
                mensagem = "FT sem permissão para este turno."
            });
        }
    }

    if (rasp.IdStatusRasp != 2 && rasp.IdStatusRasp != 7)
    {
        return Results.BadRequest(new
        {
            mensagem = "RASP não está aguardando análise FT.",
            statusAtual = rasp.IdStatusRasp
        });
    }

    var statusAnterior = rasp.IdStatusRasp;

    rasp.IdStatusRasp = 3;
    rasp.DataAprovacaoFt = DateTime.UtcNow;
    rasp.DataEnvioLg = DateTime.UtcNow;
    rasp.IdUsuarioAprovacaoFt = request.IdUsuarioExecutor;
    rasp.ObservacaoFt = request.ObservacaoFt;

    await db.SaveChangesAsync();

    await RegistrarHistoricoFluxoAsync(
        db,
        rasp.IdRasp,
        request.IdUsuarioExecutor,
        "APROVADO_FT",
        statusAnterior,
        rasp.IdStatusRasp,
        request.ObservacaoFt,
        "FT"
    );

    return Results.Ok(new
    {
        mensagem = "RASP aprovado pelo FT e enviado ao LG.",
        rasp.IdRasp,
        rasp.NumeroRasp,
        rasp.DataAprovacaoFt,
        rasp.IdStatusRasp
    });
})
.WithName("AprovarFt")
.WithTags("FT");


/// ==========================================================
// 20B. RASP - DECISÃO / APROVAÇÃO LG
// ==========================================================
app.MapPost("/rasp/{id:int}/aprovar-lg", async (
    int id,
    AprovarLgRequest request,
    RaspDbContext db) =>
{

var usuario = await db.Usuarios
    .AsNoTracking()
    .FirstOrDefaultAsync(u =>
        u.IdUsuario == request.IdUsuarioExecutor);

if (usuario is null)
{
    return Results.BadRequest(new
    {
        erro = "Usuário não encontrado."
    });
}

var perfilLg =
    usuario.IdPerfil == 3;

var administrador =
    usuario.Administrador == true;

if (!perfilLg && !administrador)
{
    return Results.StatusCode(403);
}


    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    if (request.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    if (request.DecisaoLg < 1 || request.DecisaoLg > 3)
        return Results.BadRequest("Decisão LG inválida. Use: 1 = Não emitir SPPS, 2 = Supplier Alert, 3 = SPPS.");

    if (request.DecisaoLg == 2 && string.IsNullOrWhiteSpace(request.ObservacaoLg))
    {
        return Results.BadRequest(new
        {
            mensagem = "Para aprovar Supplier Alert, a justificativa do LG é obrigatória."
        });
    }

    var rasp = await db.Rasp
        .FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp is null)
        return Results.NotFound(new { mensagem = "RASP não encontrado." });

    if (rasp.IsRascunho)
        return Results.BadRequest(new { mensagem = "Rascunho não pode ser aprovado pelo LG." });

    if (rasp.IdStatusRasp != StatusRaspConstantes.AguardandoLg)
    {
        return Results.BadRequest(new
        {
            mensagem = "Somente RASP aprovado pelo FT e aguardando análise LG pode receber decisão do LG.",
            statusAtual = rasp.IdStatusRasp
        });
    }

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    var isAdmin = usuarioExecutor.IdPerfil == 1;
    var isLg = usuarioExecutor.IdPerfil == 4;

    if (!isAdmin && !isLg)
        return Results.StatusCode(StatusCodes.Status403Forbidden);

    var statusAnterior = rasp.IdStatusRasp;
    var agora = DateTime.UtcNow;

    string resultadoLg;
    string decisaoHistorico;

    switch (request.DecisaoLg)
    {
        case 1:
            rasp.IdStatusRasp = StatusRaspConstantes.SppsNaoEmitido;
            rasp.AprovadoLg = true;
            resultadoLg = "Não emitir SPPS";
            decisaoHistorico = "NAO_EMITIR_SPPS";
            break;

        case 2:
            rasp.IdStatusRasp = StatusRaspConstantes.SupplierAlertEmitido;
            rasp.SupplierAlert = true;
            rasp.IsSupplierAlert = true;
            rasp.AprovadoLg = true;
            resultadoLg = "Emitir Supplier Alert";
            decisaoHistorico = "SUPPLIER_ALERT";
            break;

        case 3:
            rasp.IdStatusRasp = StatusRaspConstantes.SppsEmitido;
            rasp.AprovadoLg = true;
            resultadoLg = "Emitir SPPS";
            decisaoHistorico = "EMITIR_SPPS";
            break;

        default:
            return Results.BadRequest("Decisão LG inválida.");
    }

    rasp.DataAprovacaoLg = agora;
    rasp.IdUsuarioAprovacaoLg = usuarioExecutor.IdUsuario;
    rasp.IdAprovadorLg = usuarioExecutor.IdUsuario;
    rasp.JustificativaLg = request.ObservacaoLg;
    rasp.ObservacaoLg = request.ObservacaoLg;

    db.Set<RaspLgHistoricoEntity>().Add(new RaspLgHistoricoEntity
    {
        IdRasp = rasp.IdRasp,
        IdUsuarioLg = usuarioExecutor.IdUsuario,
        Decisao = decisaoHistorico,
        Justificativa = request.ObservacaoLg,
        DataHora = agora
    });

    await db.SaveChangesAsync();

    await RegistrarHistoricoFluxoAsync(
        db,
        rasp.IdRasp,
        usuarioExecutor.IdUsuario,
        "DECISAO_LG",
        statusAnterior,
        rasp.IdStatusRasp,
        request.ObservacaoLg,
        "LG"
    );

    return Results.Ok(new
    {
        mensagem = "Decisão do LG registrada com sucesso.",
        rasp.IdRasp,
        rasp.NumeroRasp,
        rasp.IdStatusRasp,
        resultadoLg,
        decisaoHistorico,
        rasp.DataAprovacaoLg,
        rasp.IdUsuarioAprovacaoLg,
        rasp.IdAprovadorLg,
        rasp.ObservacaoLg
    });
})
.WithName("AprovarRaspLg");

// ==========================================================
// 20C. HISTÓRICO COMPLETO FT / LG
// ==========================================================
app.MapGet("/rasp/{id:int}/historico-fluxo", async (
    int id,
    RaspDbContext db) =>
{
    var rasp = await db.Rasp
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.IdRasp == id);

    if (rasp is null)
    {
        return Results.NotFound(new
        {
            erro = "RASP não encontrado."
        });
    }

    var historico = await db.RaspHistoricoFluxo
        .AsNoTracking()
        .Where(x => x.IdRasp == id)
        .OrderBy(x => x.DataHora)
        .Select(x => new
        {
            x.IdHistoricoFluxo,
            x.Acao,
            x.StatusAnterior,
            x.StatusNovo,
            x.Observacao,
            x.DataHora,
            x.TempoFaseMinutos,
            x.OrigemEvento,
            x.TipoComplemento,
            x.IdUsuario
        })
        .ToListAsync();

    return Results.Ok(new
    {
        rasp.IdRasp,
        rasp.NumeroRasp,
        historico
    });
})
.WithName("HistoricoFluxoRasp");






// ==========================================================
// 20C. RASP - FT SOLICITAR COMPLEMENTO
// ==========================================================
app.MapPost("/rasp/{id:int}/solicitar-complemento-ft", async (
    int id,
    SolicitarComplementoFtRequest request,
    RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    if (request.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    if (string.IsNullOrWhiteSpace(request.MotivoComplemento))
        return Results.BadRequest(new { mensagem = "Motivo do complemento é obrigatório." });

    var rasp = await db.Rasp
        .FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp is null)
        return Results.NotFound(new { mensagem = "RASP não encontrado." });

    if (rasp.IsRascunho)
        return Results.BadRequest(new { mensagem = "Rascunho não pode receber análise FT." });

    if (rasp.IdStatusRasp != StatusRaspConstantes.AguardandoFt)
    {
        return Results.BadRequest(new
        {
            mensagem = "Somente RASP aguardando FT pode receber solicitação de complemento.",
            statusAtual = rasp.IdStatusRasp
        });
    }
    var statusAnterior = rasp.IdStatusRasp;

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    rasp.IdStatusRasp = StatusRaspConstantes.ComplementoSolicitadoFt;
    rasp.ObservacaoFt = request.MotivoComplemento;

    await db.SaveChangesAsync();

    await RegistrarHistoricoFluxoAsync(
    db,
    rasp.IdRasp,
    request.IdUsuarioExecutor,
    "COMPLEMENTO_SOLICITADO_FT",
    statusAnterior,
    rasp.IdStatusRasp,
    request.MotivoComplemento,
    "FT",
    "FT_PARA_MT"
    );


    return Results.Ok(new
    {
        mensagem = "FT solicitou complemento do RASP.",
        rasp.IdRasp,
        rasp.NumeroRasp,
        rasp.IdStatusRasp,
        motivo = request.MotivoComplemento
    });
})
.WithName("SolicitarComplementoFt");

// ==========================================================
// 20D. RASP - REENVIAR COMPLEMENTO AO FT
// ==========================================================
app.MapPost("/rasp/{id:int}/reenviar-complemento-ft", async (
    int id,
    ReenviarComplementoFtRequest request,
    RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    if (request.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    var rasp = await db.Rasp
        .FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp is null)
        return Results.NotFound(new { mensagem = "RASP não encontrado." });

    if (rasp.IsRascunho)
        return Results.BadRequest(new { mensagem = "Rascunho não pode ser reenviado ao FT." });

    if (rasp.IdStatusRasp != StatusRaspConstantes.ComplementoSolicitadoFt)
    {
        return Results.BadRequest(new
        {
            mensagem = "Somente RASP com complemento solicitado pelo FT pode ser reenviado ao FT.",
            statusAtual = rasp.IdStatusRasp
        });
    }

    var statusAnterior = rasp.IdStatusRasp;

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    var isAdmin = usuarioExecutor.IdPerfil == 1;

    if (!isAdmin)
    {
        if (!rasp.IdAnalistaMt.HasValue)
            return Results.BadRequest("Este RASP não possui MT responsável vinculado.");

        if (rasp.IdAnalistaMt.Value != request.IdUsuarioExecutor)
            return Results.StatusCode(StatusCodes.Status403Forbidden);
    }

    var observacaoReenvio = string.IsNullOrWhiteSpace(request.ObservacaoReenvio)
        ? null
        : request.ObservacaoReenvio.Trim();

    rasp.IdStatusRasp = StatusRaspConstantes.AguardandoFt;
    rasp.DataEnvioFt = DateTime.UtcNow;

    if (!string.IsNullOrWhiteSpace(observacaoReenvio))
    {
        rasp.ObservacaoGeral = observacaoReenvio;
    }

    await db.SaveChangesAsync();

    await RegistrarHistoricoFluxoAsync(
        db,
        rasp.IdRasp,
        request.IdUsuarioExecutor,
        "REENVIO_COMPLEMENTO_FT",
        statusAnterior,
        rasp.IdStatusRasp,
        observacaoReenvio,
        "MT",
        "MT_PARA_FT"
    );

    return Results.Ok(new
    {
        mensagem = "Complemento reenviado ao FT com sucesso.",
        rasp.IdRasp,
        rasp.NumeroRasp,
        rasp.IdStatusRasp,
        rasp.DataEnvioFt,
        observacaoReenvio
    });
})
.WithName("ReenviarComplementoFt");


// ==========================================================
// 20E. RASP - LG SOLICITAR COMPLEMENTO AO FT
// ==========================================================
app.MapPost("/rasp/{id:int}/solicitar-complemento-lg", async (
    int id,
    SolicitarComplementoLgRequest request,
    RaspDbContext db) =>
{

var usuario = await db.Usuarios
    .AsNoTracking()
    .FirstOrDefaultAsync(u =>
        u.IdUsuario == request.IdUsuarioExecutor);

if (usuario is null)
{
    return Results.BadRequest(new
    {
        erro = "Usuário não encontrado."
    });
}

var perfilLg =
    usuario.IdPerfil == 4;

var administrador =
    usuario.Administrador == true;

if (!perfilLg && !administrador)
{
    return Results.StatusCode(403);
}

    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    if (request.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    if (string.IsNullOrWhiteSpace(request.MotivoComplemento))
        return Results.BadRequest(new { mensagem = "Motivo do complemento é obrigatório." });

    var rasp = await db.Rasp.FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp is null)
        return Results.NotFound(new { mensagem = "RASP não encontrado." });

    if (rasp.IsRascunho)
        return Results.BadRequest(new { mensagem = "Rascunho não pode receber análise LG." });

    if (rasp.IdStatusRasp != StatusRaspConstantes.AguardandoLg)
    {
        return Results.BadRequest(new
        {
            mensagem = "Somente RASP aguardando análise LG pode receber solicitação de complemento.",
            statusAtual = rasp.IdStatusRasp
        });
    }

    var statusAnterior = rasp.IdStatusRasp;

    var usuarioExecutor = await db.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    rasp.IdStatusRasp = StatusRaspConstantes.ComplementoSolicitadoLg;
    rasp.ObservacaoLg = request.MotivoComplemento;

    await db.SaveChangesAsync();

    await RegistrarHistoricoFluxoAsync(
        db,
        rasp.IdRasp,
        request.IdUsuarioExecutor,
        "COMPLEMENTO_SOLICITADO_LG",
        statusAnterior,
        rasp.IdStatusRasp,
        request.MotivoComplemento,
        "LG",
        "LG_PARA_FT"
    );

    return Results.Ok(new
    {
        mensagem = "LG solicitou complemento ao FT.",
        rasp.IdRasp,
        rasp.NumeroRasp,
        rasp.IdStatusRasp,
        motivo = request.MotivoComplemento
    });
})
.WithName("SolicitarComplementoLg");


// ==========================================================
// 20F. RASP - FT REENVIAR COMPLEMENTO AO LG
// ==========================================================
app.MapPost("/rasp/{id:int}/reenviar-complemento-lg", async (
    int id,
    ReenviarComplementoLgRequest request,
    RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    if (request.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    var rasp = await db.Rasp.FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp is null)
        return Results.NotFound(new { mensagem = "RASP não encontrado." });

    if (rasp.IsRascunho)
        return Results.BadRequest(new { mensagem = "Rascunho não pode ser reenviado ao LG." });

    if (rasp.IdStatusRasp != StatusRaspConstantes.ComplementoSolicitadoLg)
    {
        return Results.BadRequest(new
        {
            mensagem = "Somente RASP com complemento solicitado pelo LG pode ser reenviado ao LG.",
            statusAtual = rasp.IdStatusRasp
        });
    }

    var statusAnterior = rasp.IdStatusRasp;

    var usuarioExecutor = await db.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    var isAdmin = usuarioExecutor.IdPerfil == 1;
    var isFt = usuarioExecutor.IdPerfil == 3;

    if (!isAdmin && !isFt)
        return Results.BadRequest("Somente FT ou ADMIN pode reenviar complemento ao LG.");

    var observacaoReenvio = string.IsNullOrWhiteSpace(request.ObservacaoReenvio)
        ? null
        : request.ObservacaoReenvio.Trim();

    rasp.IdStatusRasp = StatusRaspConstantes.AguardandoLg;
    rasp.DataEnvioLg = DateTime.UtcNow;

    if (!string.IsNullOrWhiteSpace(observacaoReenvio))
        rasp.ObservacaoFt = observacaoReenvio;

    await db.SaveChangesAsync();

    await RegistrarHistoricoFluxoAsync(
        db,
        rasp.IdRasp,
        request.IdUsuarioExecutor,
        "REENVIO_COMPLEMENTO_LG",
        statusAnterior,
        rasp.IdStatusRasp,
        observacaoReenvio,
        "FT",
        "FT_PARA_LG"
    );

    return Results.Ok(new
    {
        mensagem = "Complemento reenviado ao LG com sucesso.",
        rasp.IdRasp,
        rasp.NumeroRasp,
        rasp.IdStatusRasp,
        rasp.DataEnvioLg,
        observacaoReenvio
    });
})
.WithName("ReenviarComplementoLg");


// ==========================================================
// 20G. RASP - FT DEVOLVER COMPLEMENTO AO MT
// ==========================================================
app.MapPost("/rasp/{id:int}/ft-devolver-complemento-mt", async (
    int id,
    FtDevolverComplementoMtRequest request,
    RaspDbContext db) =>
{
    if (id <= 0)
        return Results.BadRequest("Id do RASP inválido.");

    if (request.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    if (string.IsNullOrWhiteSpace(request.MotivoComplemento))
        return Results.BadRequest(new { mensagem = "Motivo do complemento é obrigatório." });

    var rasp = await db.Rasp.FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp is null)
        return Results.NotFound(new { mensagem = "RASP não encontrado." });

    if (rasp.IsRascunho)
        return Results.BadRequest(new { mensagem = "Rascunho não pode receber devolução do FT." });

    if (rasp.IdStatusRasp != StatusRaspConstantes.ComplementoSolicitadoLg)
    {
        return Results.BadRequest(new
        {
            mensagem = "Somente RASP com complemento solicitado pelo LG ao FT pode ser devolvido ao MT.",
            statusAtual = rasp.IdStatusRasp
        });
    }

    var statusAnterior = rasp.IdStatusRasp;

    var usuarioExecutor = await db.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    var isAdmin = usuarioExecutor.IdPerfil == 1;
    var isFt = usuarioExecutor.IdPerfil == 3;

    if (!isAdmin && !isFt)
        return Results.BadRequest("Somente FT ou ADMIN pode devolver complemento ao MT.");

    var motivoComplemento = request.MotivoComplemento.Trim();

    rasp.IdStatusRasp = StatusRaspConstantes.ComplementoSolicitadoFt;
    rasp.ObservacaoFt = motivoComplemento;

    await db.SaveChangesAsync();

    await RegistrarHistoricoFluxoAsync(
        db,
        rasp.IdRasp,
        request.IdUsuarioExecutor,
        "FT_DEVOLVEU_COMPLEMENTO_MT",
        statusAnterior,
        rasp.IdStatusRasp,
        motivoComplemento,
        "FT",
        "FT_PARA_MT"
    );

    return Results.Ok(new
    {
        mensagem = "FT devolveu complemento ao MT com sucesso.",
        rasp.IdRasp,
        rasp.NumeroRasp,
        rasp.IdStatusRasp,
        motivo = motivoComplemento
    });
})
.WithName("FtDevolverComplementoMt");

// ==========================================================
// 20H. LG - LISTAR RASPS PENDENTES DE DECISÃO
// ==========================================================
app.MapGet("/lg/rasps-pendentes", async (RaspDbContext db) =>
{
    var agora = DateTime.UtcNow;

    var itens = await (
        from r in db.Rasp.AsNoTracking()

        join f in db.FornecedorRasp.AsNoTracking()
            on r.IdFornecedorRasp equals f.IdFornecedor

        join ftJoin in db.Usuarios.AsNoTracking()
            on r.IdUsuarioAprovacaoFt equals ftJoin.IdUsuario into ftLeft
        from ft in ftLeft.DefaultIfEmpty()

        where r.IdStatusRasp == StatusRaspConstantes.AguardandoLg
              && !r.IsRascunho

        orderby r.DataEnvioLg
        select new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataCriacao,
            r.ResumoOcorrencia,
            r.DescricaoProblema,

            Fornecedor = f.Nome,
            Duns = f.Duns,

            r.IdStatusRasp,
            r.DataEnvioLg,
            r.DataAprovacaoFt,

            FtResponsavel = ft != null
                ? (ft.Nome + " " + (ft.Sobrenome ?? "")).Trim()
                : null
        }
    ).ToListAsync();

    var resultado = itens.Select(x =>
    {
        int diasAguardandoLg = 0;
        double horasAguardandoLg = 0;

        if (x.DataEnvioLg.HasValue)
        {
            var tempo = agora - x.DataEnvioLg.Value;

            diasAguardandoLg = (int)Math.Floor(tempo.TotalDays);
            horasAguardandoLg = Math.Round(tempo.TotalHours, 1);

            if (diasAguardandoLg < 0)
                diasAguardandoLg = 0;

            if (horasAguardandoLg < 0)
                horasAguardandoLg = 0;
        }

        var criticidadePrazoLg =
            diasAguardandoLg >= 4
                ? "Crítico"
                : diasAguardandoLg >= 2
                    ? "Atenção"
                    : "Normal";

        return new
        {
            x.IdRasp,
            x.NumeroRasp,
            x.DataCriacao,
            x.ResumoOcorrencia,
            x.DescricaoProblema,
            x.Fornecedor,
            x.Duns,
            x.IdStatusRasp,
            x.DataEnvioLg,
            x.DataAprovacaoFt,
            x.FtResponsavel,
            DiasAguardandoLg = diasAguardandoLg,
            HorasAguardandoLg = horasAguardandoLg,
            CriticidadePrazoLg = criticidadePrazoLg
        };
    })
    .OrderByDescending(x => x.DiasAguardandoLg)
    .ThenBy(x => x.DataEnvioLg)
    .ToList();

    return Results.Ok(resultado);
})
.WithName("LgListarRaspsPendentes")
.WithTags("LG");

// ==========================================================
// 20J. LG - DASHBOARD DE PRAZO / FILA
// ==========================================================
app.MapGet("/lg/dashboard", async (RaspDbContext db) =>
{
    var agora = DateTime.Now;

    int CalcularDiasUteis(DateTime? dataInicio, DateTime dataFim)
    {
        if (!dataInicio.HasValue)
        {
            return 0;
        }

        var inicio = dataInicio.Value.Date;
        var fim = dataFim.Date;

        if (fim <= inicio)
        {
            return 0;
        }

        var diasUteis = 0;
        var data = inicio;

        while (data < fim)
        {
            data = data.AddDays(1);

            if (
                data.DayOfWeek != DayOfWeek.Saturday &&
                data.DayOfWeek != DayOfWeek.Sunday
            )
            {
                diasUteis++;
            }
        }

        return diasUteis;
    }

    var pendentes = await db.Rasp
        .AsNoTracking()
        .Where(r =>
            r.IdStatusRasp == StatusRaspConstantes.AguardandoLg &&
            !r.IsRascunho)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.DataEnvioLg,
            r.DataAprovacaoFt
        })
        .ToListAsync();

    var itens = pendentes.Select(x =>
    {
        var dataReferencia =
            x.DataEnvioLg ??
            x.DataAprovacaoFt;

        double horas = 0;

        if (dataReferencia.HasValue)
        {
            var tempo = agora - dataReferencia.Value;

            horas = Math.Round(tempo.TotalHours, 1);

            if (horas < 0)
            {
                horas = 0;
            }
        }

        var diasUteis = CalcularDiasUteis(
            dataReferencia,
            agora
        );

        var criticidade =
            diasUteis > 2
                ? "Crítico"
                : diasUteis == 2
                    ? "Atenção"
                    : "Normal";

        return new
        {
            x.IdRasp,
            x.NumeroRasp,
            DataReferenciaLg = dataReferencia,
            x.DataEnvioLg,
            x.DataAprovacaoFt,
            HorasAguardandoLg = horas,
            DiasUteisAguardandoLg = diasUteis,
            Criticidade = criticidade
        };
    }).ToList();

    var total = itens.Count;

    return Results.Ok(new
    {
        TotalAguardandoLg = total,

        MediaHorasAguardandoLg = total == 0
            ? 0
            : Math.Round(itens.Average(x => x.HorasAguardandoLg), 1),

        MaiorAtrasoHorasLg = total == 0
            ? 0
            : itens.Max(x => x.HorasAguardandoLg),

        Normal = itens.Count(x => x.Criticidade == "Normal"),

        Atencao = itens.Count(x => x.Criticidade == "Atenção"),

        Critico = itens.Count(x => x.Criticidade == "Crítico"),

        ItensCriticos = itens
            .Where(x => x.Criticidade == "Crítico")
            .OrderByDescending(x => x.DiasUteisAguardandoLg)
            .ThenByDescending(x => x.HorasAguardandoLg)
            .ToList()
    });
})
.WithName("LgDashboard")
.WithTags("LG");

// ==========================================================
// USUÁRIOS - PERFIL DO USUÁRIO
// ==========================================================
app.MapGet("/usuarios/{id:int}/perfil", async (
    int id,
    RaspDbContext db) =>
{
    var usuario = await db.Usuarios
        .AsNoTracking()
        .Where(u => u.IdUsuario == id)
        .Select(u => new
        {
            u.IdUsuario,
            u.Nome,
            u.Sobrenome,
            NomeCompleto = (u.Nome + " " + u.Sobrenome).Trim(),
            u.Email,
            u.Gmin,
            u.Administrador,
            u.IdPerfil,

            Perfil = db.PerfilRasp
                .Where(p => p.IdPerfil == u.IdPerfil)
                .Select(p => p.Nome)
                .FirstOrDefault()
        })
        .FirstOrDefaultAsync();

    if (usuario is null)
    {
        return Results.NotFound(new
        {
            erro = "Usuário não encontrado."
        });
    }

    return Results.Ok(usuario);
})
.WithName("UsuarioPerfil")
.WithTags("Usuários");



// ==========================================================
// 20K. LG - FILA OPERACIONAL
// ==========================================================
app.MapGet("/lg/fila-operacional", async (RaspDbContext db) =>
{
    var agora = DateTime.UtcNow;

    var idsReprovadosFt = await db.Set<RaspHistoricoFluxoEntity>()
    .AsNoTracking()
    .Where(h =>
        h.Acao != null &&
        h.Acao.Contains("REPROV"))
    .Select(h => h.IdRasp)
    .Distinct()
    .ToListAsync();



    var itens = await db.Rasp
        .AsNoTracking()
        .Where(r =>
            r.IdStatusRasp == StatusRaspConstantes.AguardandoLg &&
            !r.IsRascunho)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.ResumoOcorrencia,
            r.DataEnvioLg,
            r.SupplierAlert,
            r.IsSafety,
            r.IdMajorRasp,
            r.IdImpactoClienteRasp,
            

            Fornecedor = db.FornecedorRasp
                .Where(f => f.IdFornecedor == r.IdFornecedorRasp)
                .Select(f => new
                {
                    f.Nome,
                    f.Duns
                })
                .FirstOrDefault(),

            QuantidadePn = db.RaspPn
                .Count(p => p.IdRasp == r.IdRasp),

            PossuiContencao = db.RaspPn
                .Any(p =>
                    p.IdRasp == r.IdRasp &&
                    p.EmContencao)
        })
        .ToListAsync();

    var retorno = itens.Select(x =>
    {
        double horas = 0;
        int dias = 0;

        if (x.DataEnvioLg.HasValue)
        {
            var tempo = agora - x.DataEnvioLg.Value;

            horas = Math.Round(tempo.TotalHours, 1);
            dias = (int)Math.Floor(tempo.TotalDays);

            if (horas < 0)
                horas = 0;

            if (dias < 0)
                dias = 0;
        }

        var criticidade =
            dias >= 4
                ? "Crítico"
                : dias >= 2
                    ? "Atenção"
                    : "Normal";

        var foiReprovadoFt =
            idsReprovadosFt.Contains(x.IdRasp);

        return new
        {
            x.IdRasp,
            x.NumeroRasp,
            x.ResumoOcorrencia,

            fornecedor = x.Fornecedor?.Nome,
            duns = x.Fornecedor?.Duns,

            x.QuantidadePn,
            x.PossuiContencao,

            x.SupplierAlert,
            x.IsSafety,

            x.IdMajorRasp,
            x.IdImpactoClienteRasp,

            x.DataEnvioLg,

            DiasAguardandoLg = dias,
            HorasAguardandoLg = horas,

            Criticidade = criticidade,
            FoiReprovadoFt = foiReprovadoFt
        };
    })
    .OrderByDescending(x => x.FoiReprovadoFt)
    .ThenByDescending(x => x.HorasAguardandoLg)
    .ToList();

    return Results.Ok(retorno);
})
.WithName("LgFilaOperacional")
.WithTags("LG");


// ==========================================================
// 20L. LG - DETALHE OPERACIONAL RASP
// ==========================================================
app.MapGet("/lg/rasp/{id:int}", async (
    int id,
    RaspDbContext db) =>
{
    var rasp = await db.Rasp
        .AsNoTracking()
        .Where(r => r.IdRasp == id)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.ResumoOcorrencia,
            r.DescricaoProblema,
            r.DataCriacao,
            r.DataEnvioFt,
            r.DataAprovacaoFt,
            r.DataEnvioLg,
            r.DataAprovacaoLg,
            r.SupplierAlert,
            r.IsSupplierAlert,
            r.IsSafety,
            r.AprovadoLg,
            r.IdStatusRasp,
            r.IdMajorRasp,
            r.IdImpactoClienteRasp,
            r.IdImpactoQualidadeRasp,
            r.IdSetorRasp,
            r.IdOrigemFabricacaoRasp,
            r.IdAnalista,
            r.IdEmpresaSelecaoRasp,

            StatusDescricao = db.StatusRasp
                .Where(s => s.IdStatusRasp == r.IdStatusRasp)
                .Select(s => s.Descricao)
                .FirstOrDefault(),

            ImpactoClienteDescricao = db.ImpactoClienteRasp
                .Where(i => i.IdImpactoCliente == r.IdImpactoClienteRasp)
                .Select(i => i.Descricao)
                .FirstOrDefault(),

            ImpactoQualidadeDescricao = db.ImpactoQualidadeRasp
                .Where(i => i.IdImpactoQualidadeRasp == r.IdImpactoQualidadeRasp)
                .Select(i => i.Descricao)
                .FirstOrDefault(),

            OrigemFabricacaoDescricao = db.OrigemFabricacaoRasp
                .Where(o => o.IdOrigemFabricacaoRasp == r.IdOrigemFabricacaoRasp)
                .Select(o => o.Descricao)
                .FirstOrDefault(),

            SetorDescricao = db.SetorRasp
                .Where(s => s.IdSetorRasp == r.IdSetorRasp)
                .Select(s => s.Descricao)
                .FirstOrDefault(),

            Analista = db.Usuarios
                .Where(u => u.IdUsuario == r.IdAnalista)
                .Select(u => new
                {
                    u.IdUsuario,
                    NomeCompleto = ((u.Nome ?? "") + " " + (u.Sobrenome ?? "")).Trim(),
                    u.Gmin,
                    u.Email
                })
                .FirstOrDefault(),

            EmpresaSelecao = db.EmpresaSelecaoRasp
                .Where(e => e.IdEmpresaSelecao == r.IdEmpresaSelecaoRasp)
                .Select(e => new
                {
                    e.IdEmpresaSelecao,
                    e.NomeEmpresa,
                    e.TipoEmpresa
                })
                .FirstOrDefault(),

            TemSelecaoPecas = db.RaspPn
                .Any(p =>
                    p.IdRasp == r.IdRasp &&
                    (
                        p.EntrouSelecao == true ||
                        p.EmContencao == true ||
                        p.StatusSelecao > 0
                    )
                ),

            TemTrava = db.RaspPn
                .Any(p =>
                    p.IdRasp == r.IdRasp &&
                    p.TravaAtiva == true
                ),

            TemQhd = db.RaspPn
                .Any(p =>
                    p.IdRasp == r.IdRasp &&
                    p.QhdAtivo == true
                ),

            TemScrap = false,

            fornecedor = db.FornecedorRasp
                .Where(f => f.IdFornecedor == r.IdFornecedorRasp)
                .Select(f => new
                {
                    f.IdFornecedor,
                    f.Nome,
                    f.Duns,
                    f.TipoFornecedor
                })
                .FirstOrDefault(),

            pns = db.RaspPn
                .Where(p => p.IdRasp == r.IdRasp)
                .Select(p => new
                {
                    p.IdRaspPn,
                    p.Pn,
                    p.EmContencao,
                    p.QuantidadeSuspeita,
                    p.QuantidadeChecada,
                    p.QuantidadeRejeitada,
                    p.DataLoteInicial,
                    p.EntrouSelecao,
                    p.StatusSelecao,
                    p.TravaAtiva,
                    p.QhdAtivo
                })
                .ToList(),

            anotacoes = db.RaspAnotacao
                .Where(a => a.IdRasp == r.IdRasp)
                .OrderByDescending(a => a.DataHora)
                .Select(a => new
                {
                    a.IdRaspAnotacao,
                    a.IdUsuario,
                    a.TipoAnotacao,
                    a.TextoAnotacao,
                    a.DataHora
                })
                .ToList(),

            historicoFluxo = db.RaspHistoricoFluxo
                .Where(h => h.IdRasp == r.IdRasp)
                .OrderByDescending(h => h.DataHora)
                .Select(h => new
                {
                    h.IdHistoricoFluxo,
                    h.Acao,
                    h.StatusAnterior,
                    h.StatusNovo,
                    h.Observacao,
                    h.DataHora,
                    h.TempoFaseMinutos,
                    h.OrigemEvento
                })
                .ToList()
        })
        .FirstOrDefaultAsync();

    if (rasp is null)
    {
        return Results.NotFound(new
        {
            erro = "RASP não encontrado."
        });
    }

    return Results.Ok(rasp);
})
.WithName("LgDetalheRasp")
.WithTags("LG");

// ==========================================================
// 21A. FT - FILA OPERACIONAL POR APROVADOR
// ==========================================================
app.MapGet("/ft/fila-operacional/{idUsuario:int}", async (
    int idUsuario,
    RaspDbContext db) =>
{
    var usuario = await db.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

    if (usuario is null)
    {
        return Results.NotFound(new
        {
            erro = "Usuário não encontrado."
        });
    }

    if (usuario.IdPerfil == 2)
    {
        return Results.StatusCode(403);
    }

    var podeVerTodos =
        usuario.Administrador == true ||
        usuario.IdPerfil == 1 ||
        usuario.IdPerfil == 4 ||
        usuario.IdPerfil == 5 ||
        usuario.IdPerfil == 6 ||
        usuario.IdPerfil == 7 ||
        usuario.IdPerfil == 8;

    var fila = await db.Rasp
        .AsNoTracking()
        .Where(r =>
            (
                r.IdStatusRasp == 2 ||
                r.IdStatusRasp == StatusRaspConstantes.ComplementoSolicitadoLg
            ) &&
            !r.IsRascunho &&
            (
                podeVerTodos ||
                r.IdAprovadorFt == idUsuario
            ))
        .OrderByDescending(r =>
            r.IdStatusRasp == StatusRaspConstantes.ComplementoSolicitadoLg)
        .ThenBy(r => r.DataEnvioFt)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.ResumoOcorrencia,
            r.DataEnvioFt,
            r.IdAprovadorFt,
            r.IdTurnoRasp,
            r.IdStatusRasp,

            EhRetornoLg =
                r.IdStatusRasp == StatusRaspConstantes.ComplementoSolicitadoLg,

            StatusDescricao = db.StatusRasp
                .Where(s => s.IdStatusRasp == r.IdStatusRasp)
                .Select(s => s.Descricao)
                .FirstOrDefault(),

            Fornecedor = db.FornecedorRasp
                .Where(f => f.IdFornecedor == r.IdFornecedorRasp)
                .Select(f => f.Nome)
                .FirstOrDefault(),

            Turno = db.TurnoRasp
                .Where(t => t.IdTurnoRasp == r.IdTurnoRasp)
                .Select(t => t.Descricao)
                .FirstOrDefault(),

            AprovadorFt = db.Usuarios
                .Where(u => u.IdUsuario == r.IdAprovadorFt)
                .Select(u => (u.Nome + " " + u.Sobrenome).Trim())
                .FirstOrDefault()
        })
        .ToListAsync();

    return Results.Ok(fila);
})
.WithName("FtFilaOperacional")
.WithTags("FT");


// ==========================================================
// 21B. FT - DETALHE OPERACIONAL RASP
// ==========================================================
app.MapGet("/ft/rasp/{id:int}", async (
    int id,
    RaspDbContext db) =>
{
    var rasp = await db.Rasp
        .AsNoTracking()
        .Where(r => r.IdRasp == id)
        .Select(r => new
        {
            r.IdRasp,
            r.NumeroRasp,
            r.ResumoOcorrencia,
            r.DescricaoProblema,
            r.DataCriacao,
            r.DataEnvioFt,
            r.DataAprovacaoFt,
            r.DataEnvioLg,
            r.DataAprovacaoLg,
            r.SupplierAlert,
            r.IsSupplierAlert,
            r.IsSafety,
            r.AprovadoLg,
            r.IdStatusRasp,
            r.IdMajorRasp,
            r.IdImpactoClienteRasp,
            r.IdImpactoQualidadeRasp,
            r.IdSetorRasp,
            r.IdOrigemFabricacaoRasp,
            r.IdAnalista,
            r.IdEmpresaSelecaoRasp,
            r.IdAprovadorFt,

            StatusDescricao = db.StatusRasp
                .Where(s => s.IdStatusRasp == r.IdStatusRasp)
                .Select(s => s.Descricao)
                .FirstOrDefault(),

            ImpactoClienteDescricao = db.ImpactoClienteRasp
                .Where(i => i.IdImpactoCliente == r.IdImpactoClienteRasp)
                .Select(i => i.Descricao)
                .FirstOrDefault(),

            ImpactoQualidadeDescricao = db.ImpactoQualidadeRasp
                .Where(i => i.IdImpactoQualidadeRasp == r.IdImpactoQualidadeRasp)
                .Select(i => i.Descricao)
                .FirstOrDefault(),

            OrigemFabricacaoDescricao = db.OrigemFabricacaoRasp
                .Where(o => o.IdOrigemFabricacaoRasp == r.IdOrigemFabricacaoRasp)
                .Select(o => o.Descricao)
                .FirstOrDefault(),

            SetorDescricao = db.SetorRasp
                .Where(s => s.IdSetorRasp == r.IdSetorRasp)
                .Select(s => s.Descricao)
                .FirstOrDefault(),

            Analista = db.Usuarios
                .Where(u => u.IdUsuario == r.IdAnalista)
                .Select(u => new
                {
                    u.IdUsuario,
                    NomeCompleto = ((u.Nome ?? "") + " " + (u.Sobrenome ?? "")).Trim(),
                    u.Gmin,
                    u.Email
                })
                .FirstOrDefault(),

            AprovadorFt = db.Usuarios
                .Where(u => u.IdUsuario == r.IdAprovadorFt)
                .Select(u => new
                {
                    u.IdUsuario,
                    NomeCompleto = ((u.Nome ?? "") + " " + (u.Sobrenome ?? "")).Trim()
                })
                .FirstOrDefault(),

            EmpresaSelecao = db.EmpresaSelecaoRasp
                .Where(e => e.IdEmpresaSelecao == r.IdEmpresaSelecaoRasp)
                .Select(e => new
                {
                    e.IdEmpresaSelecao,
                    e.NomeEmpresa,
                    e.TipoEmpresa
                })
                .FirstOrDefault(),

            TemSelecaoPecas = db.RaspPn
                .Any(p =>
                    p.IdRasp == r.IdRasp &&
                    (
                        p.EntrouSelecao == true ||
                        p.EmContencao == true ||
                        p.StatusSelecao > 0
                    )
                ),

            TemTrava = db.RaspPn
                .Any(p =>
                    p.IdRasp == r.IdRasp &&
                    p.TravaAtiva == true
                ),

            TemQhd = db.RaspPn
                .Any(p =>
                    p.IdRasp == r.IdRasp &&
                    p.QhdAtivo == true
                ),

            fornecedor = db.FornecedorRasp
                .Where(f => f.IdFornecedor == r.IdFornecedorRasp)
                .Select(f => new
                {
                    f.IdFornecedor,
                    f.Nome,
                    f.Duns,
                    f.TipoFornecedor
                })
                .FirstOrDefault(),

            pns = db.RaspPn
                .Where(p => p.IdRasp == r.IdRasp)
                .Select(p => new
                {
                    p.IdRaspPn,
                    p.Pn,
                    p.EmContencao,
                    p.QuantidadeSuspeita,
                    p.QuantidadeChecada,
                    p.QuantidadeRejeitada,
                    p.DataLoteInicial,
                    p.EntrouSelecao,
                    p.StatusSelecao,
                    p.TravaAtiva,
                    p.QhdAtivo
                })
                .ToList(),

            anotacoes = db.RaspAnotacao
                .Where(a => a.IdRasp == r.IdRasp)
                .OrderByDescending(a => a.DataHora)
                .Select(a => new
                {
                    a.IdRaspAnotacao,
                    a.IdUsuario,
                    a.TipoAnotacao,
                    a.TextoAnotacao,
                    a.DataHora
                })
                .ToList(),

            historicoFluxo = db.RaspHistoricoFluxo
                .Where(h => h.IdRasp == r.IdRasp)
                .OrderByDescending(h => h.DataHora)
                .Select(h => new
                {
                    h.IdHistoricoFluxo,
                    h.Acao,
                    h.StatusAnterior,
                    h.StatusNovo,
                    h.Observacao,
                    h.DataHora,
                    h.TempoFaseMinutos,
                    h.OrigemEvento
                })
                .ToList()
        })
        .FirstOrDefaultAsync();

    if (rasp is null)
    {
        return Results.NotFound(new
        {
            erro = "RASP não encontrado."
        });
    }

    return Results.Ok(rasp);
})
.WithName("FtDetalheRasp")
.WithTags("FT");

// ==========================================================
// 20A.01 RASP - REPROVAÇÃO FT
// ==========================================================
app.MapPost("/rasp/{id:int}/reprovar-ft", async (
    int id,
    AprovarFtRequest request,
    RaspDbContext db) =>
{
    var rasp = await db.Rasp
        .FirstOrDefaultAsync(r => r.IdRasp == id);

    if (rasp == null)
    {
        return Results.NotFound(new
        {
            mensagem = "RASP não encontrado."
        });
    }

    if (rasp.IsRascunho)
    {
        return Results.BadRequest(new
        {
            mensagem = "Rascunho não pode ser reprovado pelo FT."
        });
    }

    if (string.IsNullOrWhiteSpace(request.ObservacaoFt))
    {
        return Results.BadRequest(new
        {
            mensagem = "Justificativa FT obrigatória para reprovação."
        });
    }

    var usuario = await db.Usuarios
        .AsNoTracking()
        .FirstOrDefaultAsync(u =>
            u.IdUsuario == request.IdUsuarioExecutor);

    if (usuario == null)
    {
        return Results.BadRequest(new
        {
            mensagem = "Usuário não encontrado."
        });
    }

    var perfilFt = usuario.IdPerfil == 3;
    var administrador = usuario.Administrador == true;

    if (!perfilFt && !administrador)
    {
        return Results.BadRequest(new
        {
            mensagem = "Acesso permitido somente para FT ou administrador."
        });
    }

    if (!administrador)
    {
        if (usuario.IdTurnoRasp != rasp.IdTurnoRasp)
        {
            return Results.BadRequest(new
            {
                mensagem = "FT sem permissão para este turno."
            });
        }
    }

    if (rasp.IdStatusRasp != 2 &&
        rasp.IdStatusRasp != StatusRaspConstantes.ComplementoSolicitadoLg)
    {
        return Results.BadRequest(new
        {
            mensagem = "RASP não está aguardando análise FT.",
            statusAtual = rasp.IdStatusRasp
        });
    }

    var statusAnterior = rasp.IdStatusRasp;

    rasp.IdStatusRasp = StatusRaspConstantes.AguardandoLg;
    rasp.DataAprovacaoFt = DateTime.UtcNow;
    rasp.DataEnvioLg = DateTime.UtcNow;
    rasp.IdUsuarioAprovacaoFt = request.IdUsuarioExecutor;
    rasp.ObservacaoFt = request.ObservacaoFt;


    await db.SaveChangesAsync();

    await RegistrarHistoricoFluxoAsync(
        db,
        rasp.IdRasp,
        request.IdUsuarioExecutor,
        "REPROVADO_FT",
        statusAnterior,
        rasp.IdStatusRasp,
        request.ObservacaoFt,
        "FT"
    );

    return Results.Ok(new
    {
        mensagem = "Procedência técnica reprovada pelo FT e enviada ao LG para ciência/decisão final.",
        rasp.IdRasp,
        rasp.NumeroRasp,
        rasp.IdStatusRasp,
        rasp.DataAprovacaoFt
    });
})
.WithName("ReprovarFt")
.WithTags("FT");





// -----------------------------------------------------------------------------
// 21. DEBUG
// -----------------------------------------------------------------------------

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

// -----------------------------------------------------------------------------
// 22. REQUESTS / RECORDS
// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------
// Criação inicial do RASP em rascunho
// -----------------------------------------------------------------------------
public record CriarRaspRequest(
    int IdFornecedorRasp,
    string DescricaoProblema,
    int IdUsuarioCriador
);

// -----------------------------------------------------------------------------
// Atualização completa do RASP em análise
// -----------------------------------------------------------------------------
public record AtualizarRaspRequest(
    int IdUsuarioExecutor,
    string DescricaoProblema,
    string? Procedencia,
    string? ObservacaoGeral,
    int? IdModeloVeiculoRasp,
    int? IdSetorRasp,
    int? IdTurnoRasp,
    int? IdOrigemFabricacaoRasp,
    int? IdPilotoRasp,
    int? IdImpactoClienteRasp,
    int? IdImpactoQualidadeRasp,
    int? IdMaiorImpactoRasp,
    int? IdMajorRasp,
    int? IdSppsClassificacaoRasp,
    int? IdSppsStatusRasp,
    int? IdEmpresaSelecaoRasp,
    int? IdContaCrRasp,
    int? IdContaCrSubcontaRasp,
    int? IdGmAliadoRasp,
    int? IdIndiceOperacionalRasp,
    bool? IniciativaFornecedor,
    bool? SupplierAlert,
    bool? Reversao,
    bool? Safety,
    bool? EmitiuPrr,
    bool? AprovadoLg,
    bool? IsSupplierAlert,
    bool? IsSafety,
    bool? IsReversao,
    bool? GeraPrr,
    string? BpTexto,
    string? BpSerie,
    DateTime? BpDatahora,
    string? BreakpointTexto,
    string? BreakpointCodigo,
    DateTime? BreakpointDatahora,
    int? IdAnalista,
    int? IdAprovadorFt,
    int? IdAprovadorLg,
    short? AnoRasp,
    DateOnly? DataFechamento,
    int? IdPerfilRasp
);


// -----------------------------------------------------------------------------
// Atualização parcial do RASP em rascunho
//
// Regras:
// - null = não altera
// - 0 em campos FK opcionais = limpa o campo
// - strings vazias podem ser interpretadas pela rota como limpeza, conforme regra
//
// Inclusões importantes:
// - ResumoOcorrencia foi adicionado para separar o resumo curto da descrição
// - IdAprovadorFt foi mantido para permitir salvar o FT automático por turno
// -----------------------------------------------------------------------------
public record AtualizarRaspRascunhoRequest(
    int IdUsuarioExecutor,
    int? IdFornecedorRasp,

    string? ResumoOcorrencia,
    string? DescricaoProblema,

    int? IdModeloVeiculoRasp,
    int? IdSetorRasp,
    int? IdTurnoRasp,
    int? IdIndiceOperacionalRasp,
    int? IdPilotoRasp,
    int? IdOrigemFabricacaoRasp,
    int? IdImpactoClienteRasp,
    int? IdImpactoQualidadeRasp,
    int? IdMaiorImpactoRasp,
    int? IdMajorRasp,

    int? IdSppsClassificacaoRasp,
    int? IdSppsStatusRasp,
    int? IdEmpresaSelecaoRasp,
    int? IdContaCrRasp,
    int? IdContaCrSubcontaRasp,
    int? IdGmAliadoRasp,

    [property: JsonPropertyName("idAprovadorFt")]
    int? IdAprovadorFt,

    string? SppsNumero,
    string? Procedencia,
    string? BpTexto,
    string? BpSerie,
    string? BreakpointTexto,
    string? BreakpointCodigo,
    string? ObservacaoGeral,
    string? RdNumero,
    string? CampanhaNumero,
    string? NomeContato,

    DateOnly? DataContato,
    DateTime? BpDatahora,
    DateTime? BreakpointDatahora,

    bool? IniciativaFornecedor,
    bool? SupplierAlert,
    bool? Reversao,
    bool? Safety,
    bool? EmitiuPrr,
    bool? AprovadoLg,
    bool? IsSupplierAlert,
    bool? IsSafety,
    bool? IsReversao,
    bool? GeraPrr
);



// -----------------------------------------------------------------------------
// Request padrão para ações de fluxo
// -----------------------------------------------------------------------------
public record AcaoFluxoRaspRequest(
    int IdUsuarioExecutor
);

public record AprovarFtRequest(
    int IdUsuarioExecutor,
    string? ObservacaoFt
);

public class AprovarLgRequest
{
    public int IdUsuarioExecutor { get; set; }

    // 1 = Não emitir SPPS
    // 2 = Emitir Supplier Alert
    // 3 = Emitir SPPS
    public int DecisaoLg { get; set; }

    public string? ObservacaoLg { get; set; }
}

    public class SolicitarComplementoFtRequest
{
    public int IdUsuarioExecutor { get; set; }

    public string MotivoComplemento { get; set; } = string.Empty;
}

    public class ReenviarComplementoFtRequest
{
    public int IdUsuarioExecutor { get; set; }

    public string? ObservacaoReenvio { get; set; }
}

    public class SolicitarComplementoLgRequest
{
    public int IdUsuarioExecutor { get; set; }

    public string MotivoComplemento { get; set; } = string.Empty;
}

    public class ReenviarComplementoLgRequest
{
    public int IdUsuarioExecutor { get; set; }

    public string? ObservacaoReenvio { get; set; }
}

    public class FtDevolverComplementoMtRequest
{
    public int IdUsuarioExecutor { get; set; }

    public string MotivoComplemento { get; set; } = string.Empty;
}



// -----------------------------------------------------------------------------
// Criação de vínculo RASP x PN
// -----------------------------------------------------------------------------
public record CriarRaspPnRequest(
    int IdRasp,
    string Pn,
    string? DataLoteInicial,
    int QuantidadeSuspeita,
    int QuantidadeChecada,
    int QuantidadeRejeitada,
    bool EmContencao,
    string Duns,
    short OrdemExibicao,
    bool EntrouSelecao,
    short StatusSelecao,
    DateTime? DataHoraEntradaSelecao,
    DateTime? DataHoraSaidaSelecao,
    bool TravaAtiva,
    DateTime? DataHoraSolicitacaoTrava,
    DateTime? DataHoraRemocaoTrava,
    bool QhdAtivo,
    DateTime? DataHoraQhd
);

// -----------------------------------------------------------------------------
// Cadastro de fornecedor
// -----------------------------------------------------------------------------
public record CriarFornecedorRaspRequest(
    string Duns,
    string Nome,
    string TipoFornecedor,
    bool Ativo
);

// -----------------------------------------------------------------------------
// Cadastro de PN no mestre
// -----------------------------------------------------------------------------
public record CriarPnRaspRequest(
    string CodigoPn,
    string NomePeca
);

// -----------------------------------------------------------------------------
// Registro de SPPS
// -----------------------------------------------------------------------------
public record RegistrarSppsRequest(
    int IdUsuarioExecutor,
    string SppsNumero,
    int? IdSppsClassificacaoRasp,
    int? IdSppsStatusRasp
);

// -----------------------------------------------------------------------------
// Atualização de arquivo do RASP
// -----------------------------------------------------------------------------
public record AtualizarRaspArquivoRequest(
    string TipoArquivo,
    string? Descricao,
    string CaminhoArquivo
);

// -----------------------------------------------------------------------------
// Atualização de anotação do RASP
// -----------------------------------------------------------------------------
public record AtualizarRaspAnotacaoRequest(
    string? TipoAnotacao,
    string TextoAnotacao
);

// -----------------------------------------------------------------------------
// Atualização de vínculo RASP x PN
// -----------------------------------------------------------------------------
public record AtualizarRaspPnRequest(
    string? DataLoteInicial,
    int QuantidadeSuspeita,
    int QuantidadeChecada,
    int QuantidadeRejeitada,
    bool EmContencao,
    string Duns,
    short OrdemExibicao,
    bool EntrouSelecao,
    short StatusSelecao,
    DateTime? DataHoraEntradaSelecao,
    DateTime? DataHoraSaidaSelecao,
    bool TravaAtiva,
    DateTime? DataHoraSolicitacaoTrava,
    DateTime? DataHoraRemocaoTrava,
    bool QhdAtivo,
    DateTime? DataHoraQhd
);

// -----------------------------------------------------------------------------
// Troca administrativa de analista
// -----------------------------------------------------------------------------
public record TrocarAnalistaRaspRequest(
    int IdUsuarioExecutor,
    int IdNovoAnalistaMt
);

// -----------------------------------------------------------------------------
// Troca administrativa de FT
// -----------------------------------------------------------------------------
public record TrocarFtRaspRequest(
    int IdUsuarioExecutor,
    int IdNovoAprovadorFt
);

// -----------------------------------------------------------------------------
// Admin - cadastro de usuário GM
// -----------------------------------------------------------------------------
public record CriarUsuarioGmAdminRequest(
    int IdUsuarioExecutor,
    string Nome,
    string Sobrenome,
    string Gmin,
    string Email,
    string Cargo,
    int IdPerfil,
    int? IdTurnoRasp,
    bool Administrador,
    string SenhaInicial
);

// -----------------------------------------------------------------------------
// Admin - atualização de usuário GM
// -----------------------------------------------------------------------------
public record AtualizarUsuarioGmAdminRequest(
    int IdUsuarioExecutor,
    string? Nome,
    string? Sobrenome,
    string? Email,
    string? Cargo,
    int? IdPerfil,
    int? IdTurnoRasp,
    bool? Ativo,
    bool? Administrador,
    string? NovaSenha
);

// -----------------------------------------------------------------------------
// Admin - cadastro de usuário terceiro
// -----------------------------------------------------------------------------
public record CriarUsuarioTerceiroAdminRequest(
    int IdUsuarioExecutor,
    string Nome,
    string Sobrenome,
    string Gmid,
    int IdEmpresaSelecao,
    string SenhaInicial
);

// -----------------------------------------------------------------------------
// Admin - atualização de usuário terceiro
// -----------------------------------------------------------------------------
public record AtualizarUsuarioTerceiroAdminRequest(
    int IdUsuarioExecutor,
    string? Nome,
    string? Sobrenome,
    int? IdEmpresaSelecao,
    bool? Ativo,
    string? NovaSenha
);


// -----------------------------------------------------------------------------
// Troca administrativa de LG
// -----------------------------------------------------------------------------
public record TrocarLgRaspRequest(
    int IdUsuarioExecutor,
    int IdNovoAprovadorLg
);

// -----------------------------------------------------------------------------
// Validação rápida de terceiro na tela de seleção
// -----------------------------------------------------------------------------
public record ValidarTerceiroSelecaoRequest(
    string Gmid,
    string Senha
);

// -----------------------------------------------------------------------------
// Validação de executor da seleção operacional
// -----------------------------------------------------------------------------
public record ValidarExecutorSelecaoRequest(
    string TipoExecutor,
    string Login,
    string Senha
);


