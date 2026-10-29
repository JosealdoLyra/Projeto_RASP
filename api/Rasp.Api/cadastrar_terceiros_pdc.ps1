
# ============================================================
# RASP - CADASTRO EM LOTE DE USUARIOS TERCEIROS - PDC
# ============================================================

$API = "http://localhost:5050"
$IdUsuarioExecutor = 1
$IdEmpresaSelecao = 2
$SenhaInicial = "12345678"

# Primeiro nome e segundo nome como sobrenome.
# Os quatro nomes destacados em vermelho foram excluidos.

$Usuarios = @(
    @{ Nome="ANDREZA"; Sobrenome="MARIA"; Gmid="707198606" }
    @{ Nome="CLAUDINEIA"; Sobrenome="RODRIGUES"; Gmid="671714282" }
    @{ Nome="CLAUDIO"; Sobrenome="MARCELO"; Gmid="353790047" }
    @{ Nome="CRISTIANE"; Sobrenome="DE"; Gmid="457578920" }
    @{ Nome="DOUGLAS"; Sobrenome="DA"; Gmid="576393231" }
    @{ Nome="ELAINE"; Sobrenome="CRISTINA"; Gmid="878197139" }
    @{ Nome="JANIA"; Sobrenome="MARIA"; Gmid="424331941" }
    @{ Nome="JOSE"; Sobrenome="RICARDO"; Gmid="701717915" }
    @{ Nome="LETICIA"; Sobrenome="VITORIA"; Gmid="949364192" }
    @{ Nome="MAURICEIA"; Sobrenome="FERREIRA"; Gmid="991187624" }
    @{ Nome="RAFAEL"; Sobrenome="CONRADO"; Gmid="447645755" }
    @{ Nome="RODRIGO"; Sobrenome="MACHADO"; Gmid="637548323" }
    @{ Nome="SAMUEL"; Sobrenome="OZANIR"; Gmid="153657377" }
    @{ Nome="VITOR"; Sobrenome="BELOSO"; Gmid="218535148" }
    @{ Nome="VITORIA"; Sobrenome="MIRANDA"; Gmid="979784691" }
    @{ Nome="RICHARLYSON"; Sobrenome="ISRAEL"; Gmid="913227966" }
)

Write-Host ""
Write-Host "============================================"
Write-Host " RASP - CADASTRO DE TERCEIROS - PDC"
Write-Host "============================================"
Write-Host "Administrador ID: $IdUsuarioExecutor"
Write-Host "Empresa PDC ID: $IdEmpresaSelecao"
Write-Host "Total da lista: $($Usuarios.Count)"
Write-Host ""

# Consultar usuarios existentes antes do cadastro.

try {
    $Existentes = @(
        Invoke-RestMethod `
            -Uri "$API/admin/usuarios-terceiros" `
            -Method GET `
            -ErrorAction Stop
    )
}
catch {
    Write-Host "ERRO: Nao foi possivel consultar a API." -ForegroundColor Red
    Write-Host $_.Exception.Message
    exit 1
}

# Separar quem ja existe de quem sera cadastrado.

$Pendentes = @()
$Ignorados = @()

foreach ($Usuario in $Usuarios) {

    $GmidAtual = [string]$Usuario.Gmid

    $JaExiste = @(
        $Existentes | Where-Object {
            [string]$_.gmid -eq $GmidAtual
        }
    ).Count -gt 0

    if ($JaExiste) {
        $Ignorados += $Usuario
    }
    else {
        $Pendentes += $Usuario
    }
}

Write-Host "Ja cadastrados: $($Ignorados.Count)"
Write-Host "Novos cadastros: $($Pendentes.Count)"
Write-Host ""

foreach ($Usuario in $Pendentes) {
    Write-Host "$($Usuario.Nome) $($Usuario.Sobrenome) - $($Usuario.Gmid)"
}

if ($Pendentes.Count -eq 0) {
    Write-Host ""
    Write-Host "Nenhum cadastro novo necessario."
    exit 0
}

Write-Host ""
$Confirmacao = Read-Host "Digite CADASTRAR para confirmar"

if ($Confirmacao -cne "CADASTRAR") {
    Write-Host "Operacao cancelada. Nenhum cadastro enviado."
    exit 0
}

# Enviar os cadastros para a API.

$Sucessos = 0
$Falhas = 0

foreach ($Usuario in $Pendentes) {

    $Payload = @{
        idUsuarioExecutor = $IdUsuarioExecutor
        nome = $Usuario.Nome
        sobrenome = $Usuario.Sobrenome
        gmid = $Usuario.Gmid
        idEmpresaSelecao = $IdEmpresaSelecao
        senhaInicial = $SenhaInicial
    }

    $Json = $Payload | ConvertTo-Json -Compress

    try {
        Invoke-RestMethod `
            -Uri "$API/admin/usuarios-terceiros" `
            -Method POST `
            -ContentType "application/json; charset=utf-8" `
            -Body ([System.Text.Encoding]::UTF8.GetBytes($Json)) `
            -ErrorAction Stop | Out-Null

        $Sucessos++

        Write-Host "OK: $($Usuario.Nome) $($Usuario.Sobrenome)" `
            -ForegroundColor Green
    }
    catch {
        $Falhas++

        Write-Host "FALHA: $($Usuario.Nome) $($Usuario.Sobrenome)" `
            -ForegroundColor Red

        Write-Host $_.Exception.Message
    }
}

Write-Host ""
Write-Host "============================================"
Write-Host " RESULTADO FINAL"
Write-Host "============================================"
Write-Host "Cadastrados com sucesso: $Sucessos"
Write-Host "Ja existentes: $($Ignorados.Count)"
Write-Host "Falhas: $Falhas"
Write-Host ""
Write-Host "Processamento concluido."

