using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Rasp.Api.Data;
using Rasp.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Configuração de serviços da aplicação
// -----------------------------------------------------------------------------

// Swagger / OpenAPI para documentação e testes dos endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro do DbContext com PostgreSQL
builder.Services.AddDbContext<RaspDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Swagger habilitado no ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// -----------------------------------------------------------------------------
// STATUS RASP
// -----------------------------------------------------------------------------

// GET /status-rasp
// Lista todos os status do fluxo do RASP em ordem lógica de processo.
app.MapGet("/status-rasp", async (RaspDbContext db) =>
{
    var itens = await db.StatusRasp
        .OrderBy(s => s.OrdemFluxo)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarStatusRasp");

// GET /status-rasp/{id}
// Retorna um status específico pelo id.
app.MapGet("/status-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.StatusRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Status não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterStatusRaspPorId");

// -----------------------------------------------------------------------------
// PN RASP
// -----------------------------------------------------------------------------

// GET /pn-rasp
// Lista todos os PNs cadastrados no cadastro mestre.
app.MapGet("/pn-rasp", async (RaspDbContext db) =>
{
    var itens = await db.PnRasp
        .OrderBy(p => p.CodigoPn)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarPnRasp");

// GET /pn-rasp/{id}
// Retorna um PN específico pelo id.
app.MapGet("/pn-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.PnRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("PN não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterPnRaspPorId");

// GET /pn-rasp/codigo/{codigoPn}
// Busca um PN pelo código, validando:
// - obrigatório
// - exatamente 8 caracteres
// - somente números
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

// POST /pn-rasp
// Cadastra um novo PN no cadastro mestre.
//
// Regras:
// - PN obrigatório
// - PN com 8 dígitos numéricos
// - Nome da peça obrigatório
// - Não permitir duplicidade de código
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

// -----------------------------------------------------------------------------
// FORNECEDOR RASP
// -----------------------------------------------------------------------------

// GET /fornecedor-rasp
// Lista todos os fornecedores cadastrados.
app.MapGet("/fornecedor-rasp", async (RaspDbContext db) =>
{
    var itens = await db.FornecedorRasp
        .OrderBy(f => f.Nome)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarFornecedorRasp");

// GET /fornecedor-rasp/duns/{duns}
// Busca fornecedor pelo DUNS, validando:
// - obrigatório
// - exatamente 9 caracteres
// - somente números
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

// GET /fornecedor-rasp/{id}
// Retorna um fornecedor pelo id.
app.MapGet("/fornecedor-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.FornecedorRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Fornecedor não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterFornecedorRaspPorId");

// POST /fornecedor-rasp
// Cadastra um fornecedor.
//
// Regras:
// - DUNS obrigatório, com 9 dígitos numéricos
// - Nome obrigatório
// - TipoFornecedor deve ser LOCAL ou IMPORTADO
// - IdPais válido
// - Não permitir DUNS duplicado
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

// -----------------------------------------------------------------------------
// PERFIL RASP
// -----------------------------------------------------------------------------

// GET /perfil-rasp
// Lista os perfis do sistema (ADMIN, ANALISTA, FT, LG, etc.).
app.MapGet("/perfil-rasp", async (RaspDbContext db) =>
{
    var itens = await db.PerfilRasp
        .OrderBy(p => p.Nome)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarPerfilRasp");

// GET /perfil-rasp/{id}
// Retorna um perfil específico pelo id.
app.MapGet("/perfil-rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.PerfilRasp.FindAsync(id);

    return item is null
        ? Results.NotFound("Perfil não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterPerfilRaspPorId");

// -----------------------------------------------------------------------------
// USUÁRIOS
// -----------------------------------------------------------------------------

// GET /usuarios
// Lista usuários cadastrados no sistema.
app.MapGet("/usuarios", async (RaspDbContext db) =>
{
    var itens = await db.Usuarios
        .OrderBy(u => u.Nome)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarUsuarios");

// GET /usuarios/{id}
// Retorna um usuário específico pelo id.
app.MapGet("/usuarios/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.Usuarios.FindAsync(id);

    return item is null
        ? Results.NotFound("Usuário não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterUsuarioPorId");

// -----------------------------------------------------------------------------
// RASP
// -----------------------------------------------------------------------------

// GET /rasp
// Lista os RASPs cadastrados, do mais recente para o mais antigo.
app.MapGet("/rasp", async (RaspDbContext db) =>
{
    var itens = await db.Rasp
        .OrderByDescending(r => r.IdRasp)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRasp");

// GET /rasp/{id}
// Retorna um RASP específico pelo id.
app.MapGet("/rasp/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.Rasp.FindAsync(id);

    return item is null
        ? Results.NotFound("RASP não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterRaspPorId");

// POST /rasp
// Cria um novo RASP em rascunho.
//
// Regras:
// - fornecedor deve existir
// - usuário criador deve existir e estar ativo
// - descrição do problema obrigatória
//
// Efeito esperado:
// - cria o RASP com status 1 (Em análise)
// - marca como rascunho
// - grava o autor em id_analista_mt
// - gera numero_rasp no formato 000X/YY
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

    var insertSql = """
        INSERT INTO rasp
            (numero_rasp, data_criacao, hora_criacao, id_fornecedor_rasp, descricao_problema, id_status_rasp, is_rascunho, percentual_completude, id_analista_mt)
        VALUES
            ('TEMP', CURRENT_DATE, CURRENT_TIME, @id_fornecedor, @descricao, 1, true, 0, @id_usuario_criador)
        RETURNING id_rasp;
        """;

    int idRasp;
    await using (var cmd = new NpgsqlCommand(insertSql, conn, tx))
    {
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

    var updateSql = """
        UPDATE rasp
        SET numero_rasp = LPAD(@id::text, 4, '0') || '/' || to_char(CURRENT_DATE, 'YY')
        WHERE id_rasp = @id;
        """;

    await using (var cmd2 = new NpgsqlCommand(updateSql, conn, tx))
    {
        cmd2.Parameters.AddWithValue("id", idRasp);
        await cmd2.ExecuteNonQueryAsync();
    }

    await tx.CommitAsync();

    return Results.Created($"/rasp/{idRasp}", new { id_rasp = idRasp });
})
.WithName("CriarRasp");

// PUT /rasp/{id}
// Atualiza o conteúdo principal do RASP enquanto ele ainda estiver em análise.
//
// Regras:
// - somente RASP em status 1 pode ser editado
// - usuário executor precisa existir e estar ativo
// - ADMIN pode editar
// - ANALISTA só pode editar se for o autor do RASP
// - outro analista apenas visualiza, não edita
//
// Nesta etapa, além dos blocos anteriores, também atualizamos:
// - bloco de BP / Breakpoint
app.MapPut("/rasp/{id:int}", async (int id, AtualizarRaspRequest req, RaspDbContext db) =>
{
    var item = await db.Rasp.FindAsync(id);

    if (item is null)
        return Results.NotFound("RASP não encontrado.");

    if (item.IdStatusRasp != 1)
        return Results.BadRequest("Edição permitida somente para RASP em análise.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    // Perfil 1 = ADMIN
    var isAdmin = usuarioExecutor.IdPerfil == 1;

    if (!isAdmin)
    {
        if (!item.IdAnalistaMt.HasValue)
            return Results.BadRequest("RASP sem autor definido. Edição não permitida.");

        if (item.IdAnalistaMt.Value != req.IdUsuarioExecutor)
            return Results.BadRequest("Somente o autor do RASP ou o ADMIN pode editar este registro.");
    }

    var descricaoProblema = (req.DescricaoProblema ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(descricaoProblema))
        return Results.BadRequest("DescricaoProblema é obrigatória.");

    // Atualização do conteúdo textual principal
    item.DescricaoProblema = descricaoProblema;
    item.Procedencia = string.IsNullOrWhiteSpace(req.Procedencia) ? null : req.Procedencia.Trim();
    item.ObservacaoGeral = string.IsNullOrWhiteSpace(req.ObservacaoGeral) ? null : req.ObservacaoGeral.Trim();

    // Atualização do primeiro bloco de campos estruturais do formulário
    item.IdModeloVeiculoRasp = req.IdModeloVeiculoRasp;
    item.IdSetorRasp = req.IdSetorRasp;
    item.IdTurnoRasp = req.IdTurnoRasp;
    item.IdOrigemFabricacaoRasp = req.IdOrigemFabricacaoRasp;
    item.IdPilotoRasp = req.IdPilotoRasp;

    // Atualização do bloco de impactos do processo
    item.IdImpactoClienteRasp = req.IdImpactoClienteRasp;
    item.IdImpactoQualidadeRasp = req.IdImpactoQualidadeRasp;
    item.IdMaiorImpactoRasp = req.IdMaiorImpactoRasp;
    item.IdMajorRasp = req.IdMajorRasp;

    // Atualização do bloco de classificações e vínculos operacionais
    item.IdSppsClassificacaoRasp = req.IdSppsClassificacaoRasp;
    item.IdSppsStatusRasp = req.IdSppsStatusRasp;
    item.IdEmpresaSelecaoRasp = req.IdEmpresaSelecaoRasp;
    item.IdContaCrRasp = req.IdContaCrRasp;
    item.IdContaCrSubcontaRasp = req.IdContaCrSubcontaRasp;
    item.IdGmAliadoRasp = req.IdGmAliadoRasp;

    // Atualização do bloco de flags / indicadores booleanos
    item.IniciativaFornecedor = req.IniciativaFornecedor;
    item.SupplierAlert = req.SupplierAlert;
    item.Reversao = req.Reversao;
    item.Safety = req.Safety;
    item.EmitiuPrr = req.EmitiuPrr;
    item.AprovadoLg = req.AprovadoLg;
    item.IsSupplierAlert = req.IsSupplierAlert;
    item.IsSafety = req.IsSafety;
    item.IsReversao = req.IsReversao;
    item.GeraPrr = req.GeraPrr;

    // Atualização do bloco de BP / Breakpoint
    item.BpTexto = string.IsNullOrWhiteSpace(req.BpTexto) ? null : req.BpTexto.Trim();
    item.BpSerie = string.IsNullOrWhiteSpace(req.BpSerie) ? null : req.BpSerie.Trim();
    item.BpDatahora = req.BpDatahora;
    item.BreakpointTexto = string.IsNullOrWhiteSpace(req.BreakpointTexto) ? null : req.BreakpointTexto.Trim();
    item.BreakpointCodigo = string.IsNullOrWhiteSpace(req.BreakpointCodigo) ? null : req.BreakpointCodigo.Trim();
    item.BreakpointDatahora = req.BreakpointDatahora;


    // Atualização do bloco de responsáveis e fechamento
    item.IdAnalista = req.IdAnalista;
    item.IdAprovadorFt = req.IdAprovadorFt;
    item.IdAprovadorLg = req.IdAprovadorLg;
    item.AnoRasp = req.AnoRasp;
    item.DataFechamento = req.DataFechamento;
    item.IdPerfilRasp = req.IdPerfilRasp;
    item.IdIndiceOperacionalRasp = req.IdIndiceOperacionalRasp;
    
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
            $"Erro de banco ao salvar o RASP. Constraint: {pgEx.ConstraintName}");
    }
})
.WithName("AtualizarRasp");

// POST /rasp/{id}/enviar-ft
// Transição do fluxo: Em análise (MT) -> Em avaliação FT.
//
// Regras:
// - o RASP precisa existir
// - o RASP precisa estar em status 1
// - o executor precisa existir e estar ativo
// - ADMIN pode executar
// - não ADMIN só pode executar se for o autor do RASP
// - o documento ainda precisa estar como rascunho
//
// Efeito esperado:
// - id_status_rasp = 2
// - is_rascunho = false
app.MapPost("/rasp/{id:int}/enviar-ft", async (int id, AcaoFluxoRaspRequest req, RaspDbContext db, IConfiguration config) =>
{
    var item = await db.Rasp.FindAsync(id);

    if (item is null)
        return Results.NotFound("RASP não encontrado.");

    if (item.IdStatusRasp != 1)
        return Results.BadRequest("Somente RASP em análise pode ser enviado para FT.");

    if (req.IdUsuarioExecutor <= 0)
        return Results.BadRequest("IdUsuarioExecutor inválido.");

    var usuarioExecutor = await db.Usuarios
        .FirstOrDefaultAsync(u => u.IdUsuario == req.IdUsuarioExecutor);

    if (usuarioExecutor is null)
        return Results.BadRequest("Usuário executor não existe.");

    if (!usuarioExecutor.Ativo)
        return Results.BadRequest("Usuário executor está inativo.");

    // Perfis atuais da tabela perfil_rasp:
    // 1 = ADMIN
    // 2 = ANALISTA
    // 3 = FT
    // 4 = LG
    var isAdmin = usuarioExecutor.IdPerfil == 1;

    if (!isAdmin)
    {
        if (!item.IdAnalistaMt.HasValue)
            return Results.BadRequest("RASP sem autor definido. Envio não permitido.");

        if (item.IdAnalistaMt.Value != req.IdUsuarioExecutor)
            return Results.BadRequest("Somente o autor do RASP ou o ADMIN pode enviar este registro para FT.");
    }

    if (!item.IsRascunho)
        return Results.BadRequest("Este RASP já foi enviado e não está mais como rascunho.");

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
                id_status_rasp = 2
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
.WithName("EnviarRaspParaFt");

// POST /rasp/{id}/enviar-lg
// Transição do fluxo: Em avaliação FT -> Em avaliação LG.
//
// Regras:
// - o RASP precisa existir
// - o RASP precisa estar em status 2
// - o executor precisa existir e estar ativo
// - ADMIN pode executar
// - no fluxo normal, FT também pode executar
//
// Efeito esperado:
// - id_status_rasp = 3
// - is_rascunho permanece false
app.MapPost("/rasp/{id:int}/enviar-lg", async (int id, AcaoFluxoRaspRequest req, RaspDbContext db, IConfiguration config) =>
{
    var item = await db.Rasp.FindAsync(id);

    if (item is null)
        return Results.NotFound("RASP não encontrado.");

    if (item.IdStatusRasp != 2)
        return Results.BadRequest("Somente RASP em avaliação FT pode ser enviado para LG.");

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
        return Results.BadRequest("Somente FT ou ADMIN pode enviar este registro para LG.");

    if (item.IsRascunho)
        return Results.BadRequest("RASP em avaliação não pode estar como rascunho.");

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
                id_status_rasp = 3
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
.WithName("EnviarRaspParaLg");

// POST /rasp/{id}/retornar-ft
// Transição administrativa: Em avaliação LG -> Em avaliação FT.
//
// Regras:
// - o RASP precisa existir
// - o RASP precisa estar em status 3
// - o executor precisa existir e estar ativo
// - somente ADMIN pode executar
//
// Efeito esperado:
// - id_status_rasp = 2
// - is_rascunho permanece false
app.MapPost("/rasp/{id:int}/retornar-ft", async (int id, AcaoFluxoRaspRequest req, RaspDbContext db, IConfiguration config) =>
{
    var item = await db.Rasp.FindAsync(id);

    if (item is null)
        return Results.NotFound("RASP não encontrado.");

    if (item.IdStatusRasp != 3)
        return Results.BadRequest("Somente RASP em avaliação LG pode retornar para FT.");

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
        return Results.BadRequest("Somente ADMIN pode retornar este registro para FT.");

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
                id_status_rasp = 2
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
.WithName("RetornarRaspParaFt");

// POST /rasp/{id}/retornar-analise
// Transição administrativa:
// - Em avaliação FT -> Em análise
// - Em avaliação LG -> Em análise
//
// Regras:
// - o RASP precisa existir
// - o RASP precisa estar em status 2 ou 3
// - o executor precisa existir e estar ativo
// - somente ADMIN pode executar
//
// Efeito esperado:
// - id_status_rasp = 1
// - is_rascunho = false
//
// Observação:
// o documento volta para análise, mas não volta a ser rascunho.
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

// -----------------------------------------------------------------------------
// RASP_PN
// -----------------------------------------------------------------------------

// GET /rasp-pn
// Lista vínculos entre RASP e PN.
app.MapGet("/rasp-pn", async (RaspDbContext db) =>
{
    var itens = await db.RaspPn
        .OrderByDescending(rp => rp.IdRaspPn)
        .ToListAsync();

    return Results.Ok(itens);
})
.WithName("ListarRaspPn");

// GET /rasp-pn/{id}
// Retorna um vínculo específico pelo id.
app.MapGet("/rasp-pn/{id:int}", async (int id, RaspDbContext db) =>
{
    var item = await db.RaspPn.FindAsync(id);

    return item is null
        ? Results.NotFound("RASP PN não encontrado.")
        : Results.Ok(item);
})
.WithName("ObterRaspPnPorId");

// POST /rasp-pn
// Vincula um PN a um RASP.
//
// Regras:
// - RASP deve existir
// - PN deve existir no cadastro mestre
// - DUNS deve existir no cadastro de fornecedor
// - quantidades não podem ser negativas
// - ordem de exibição maior que zero
// - não permitir duplicidade do mesmo PN no mesmo RASP
app.MapPost("/rasp-pn", async (CriarRaspPnRequest req, RaspDbContext db) =>
{
    if (req.IdRasp <= 0)
        return Results.BadRequest("IdRasp inválido.");

    var raspExiste = await db.Rasp
        .AnyAsync(r => r.IdRasp == req.IdRasp);

    if (!raspExiste)
        return Results.BadRequest("RASP informado não existe.");

    var pn = (req.Pn ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(pn))
        return Results.BadRequest("Pn é obrigatório.");

    if (pn.Length != 8)
        return Results.BadRequest("Pn deve ter exatamente 8 caracteres.");

    if (!pn.All(char.IsDigit))
        return Results.BadRequest("Pn deve conter somente números.");

    var pnExiste = await db.PnRasp
        .AnyAsync(p => p.CodigoPn == pn);

    if (!pnExiste)
        return Results.BadRequest("PN informado não existe no cadastro.");

    var duns = (req.Duns ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(duns))
        return Results.BadRequest("Duns é obrigatório.");

    if (duns.Length != 9)
        return Results.BadRequest("Duns deve ter exatamente 9 caracteres.");

    if (!duns.All(char.IsDigit))
        return Results.BadRequest("Duns deve conter somente números.");

    var dunsExiste = await db.FornecedorRasp
        .AnyAsync(f => f.Duns == duns);

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

// -----------------------------------------------------------------------------
// DEBUG
// -----------------------------------------------------------------------------

// GET /debug-model
// Endpoint de apoio para conferência do mapeamento do EF Core.
// Útil em desenvolvimento para validar entidades e nomes de tabela.
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
// REQUESTS / RECORDS
// -----------------------------------------------------------------------------

// Criação inicial do RASP em rascunho, já gravando o autor.
public record CriarRaspRequest(
    int IdFornecedorRasp,
    string DescricaoProblema,
    int IdUsuarioCriador
);

// Atualização do conteúdo principal do RASP em análise.
// Nesta etapa, além dos blocos já existentes, também permitimos atualizar:
// - bloco de responsáveis e fechamento
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
    int? IdPerfilRasp,
    int? IdIndiceOperacionalRasp
);

// Request padrão para ações de transição do fluxo do RASP.
// Informa quem está executando a ação para a API validar perfil e permissão.
public record AcaoFluxoRaspRequest(
    int IdUsuarioExecutor
);

// Vínculo de um PN com um RASP.
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

// Cadastro de fornecedor.
public record CriarFornecedorRaspRequest(
    string Duns,
    string Nome,
    string TipoFornecedor,
    bool Ativo,
    int IdPais
);

// Cadastro de PN no mestre.
public record CriarPnRaspRequest(
    string CodigoPn,
    string NomePeca
);