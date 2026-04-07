document.addEventListener("DOMContentLoaded", () => {
  // ==========================================================
  // 01. CONFIGURAÇÕES GERAIS
  // ==========================================================
  const API_BASE_URL = "http://localhost:5050";
  const ID_USUARIO_LOGADO = 1;
  const NOME_USUARIO_LOGADO = "Usuário Logado";

  // ==========================================================
  // 02. REFERÊNCIAS PRINCIPAIS DO DOM
  // ==========================================================
  const form = document.getElementById("raspForm");
  const mensagemRasp = document.getElementById("mensagemRasp");

  // ==========================================================
  // 03. CONTROLE DE ABAS DO RASP
  // ==========================================================
  const tabs = document.querySelectorAll(".rasp-tab");
  const tabContents = document.querySelectorAll(".rasp-tab-content");

  // ==========================================================
  // 04. INDICADORES / INFORMAÇÕES DO RASP
  // ==========================================================
  const raspIndicadorBox = document.getElementById("raspIndicadorBox");
  const raspNumeroDisplay = document.getElementById("raspNumeroDisplay");
  const dataCriacaoRaspDisplay = document.getElementById("dataCriacaoRasp");
  const numeroRaspInfo = document.getElementById("numeroRasp");
  const dataCriacaoRaspInfo = document.getElementById("dataCriacaoRasp");

  // ==========================================================
  // 05. SEÇÃO 2 - DADOS BÁSICOS DO RASP
  // ==========================================================
  const dunsInput = document.getElementById("duns");
  const nomeFornecedorInput = document.getElementById("nomeFornecedor");
  const tipoFornecedorInput = document.getElementById("tipoFornecedor");
  const analistaInput = document.getElementById("analista");
  const statusInicialInput = document.getElementById("statusInicial");
  const dunsStatus = document.getElementById("dunsStatus");

  const setorSelect = document.getElementById("setor");
  const origemSelect = document.getElementById("origem");

  // ==========================================================
  // 06. SEÇÃO 3 - CLASSIFICAÇÃO INICIAL
  // ==========================================================
  const maiorImpactoSelect = document.getElementById("maiorImpacto");
  const impactoQualidadeSelect = document.getElementById("impactoQualidade");
  const impactoClienteSelect = document.getElementById("impactoCliente");

  // ==========================================================
  // 07. SEÇÃO 4 - PN
  // ==========================================================
  const addPnRowBtn = document.getElementById("addPnRow");
  const pnTableBody = document.getElementById("pnTableBody");
  const btnLimpar = document.getElementById("btnLimpar");
  const toggleMassPanelBtn = document.getElementById("toggleMassPanel");
  const massPanel = document.getElementById("massPanel");
  const importMassPnsBtn = document.getElementById("importMassPns");
  const pnsLoteTextarea = document.getElementById("pnsLoteTextarea");
  const pnStatus = document.getElementById("pnStatus");

  // ==========================================================
  // 08. MODAL DE CADASTRO DE PN
  // ==========================================================
  const modalPn = document.getElementById("modalPn");
  const modalPnCodigo = document.getElementById("modalPnCodigo");
  const modalPnDescricao = document.getElementById("modalPnDescricao");
  const salvarPnBtn = document.getElementById("salvarPn");
  const cancelarPnBtn = document.getElementById("cancelarPn");

  // ==========================================================
  // 09. MODAL DE CADASTRO DE FORNECEDOR
  // ==========================================================
  const modalFornecedor = document.getElementById("modalFornecedor");
  const modalFornecedorDuns = document.getElementById("modalFornecedorDuns");
  const modalFornecedorNome = document.getElementById("modalFornecedorNome");
  const modalFornecedorTipo = document.getElementById("modalFornecedorTipo");
  const salvarFornecedorBtn = document.getElementById("salvarFornecedor");
  const cancelarFornecedorBtn = document.getElementById("cancelarFornecedor");

  // ==========================================================
  // 10. SEÇÃO 5 - DADOS COMPLEMENTARES
  // ==========================================================
  const modeloVeiculoSelect = document.getElementById("modeloVeiculo");
  const turnoRaspSelect = document.getElementById("turnoRasp");
  const pilotoRaspSelect = document.getElementById("pilotoRasp");
  const majorRaspSelect = document.getElementById("majorRasp");

  const gmAliadoSelect = document.getElementById("gmAliado");
  const aprovadorFtInput = document.getElementById("aprovadorFt");
  const aprovadorFtIdInput = document.getElementById("aprovadorFtId");
  const iniciativaFornecedorInput = document.getElementById("iniciativaFornecedor");

  const nomeContatoInput = document.getElementById("nomeContato");
  const dataContatoInput = document.getElementById("dataContato");
  const rdNumeroInput = document.getElementById("rdNumero");
  const campanhaNumeroInput = document.getElementById("campanhaNumero");

  // ==========================================================
  // 11. SEÇÃO 6 - BP (BREAK POINT)
  // ==========================================================
  const tipoReferenciaBp = document.getElementById("tipoReferenciaBp");
  const dataBp = document.getElementById("dataBp");
  const horaBp = document.getElementById("horaBp");
  const vinBp = document.getElementById("vinBp");
  const localCelulaBp = document.getElementById("localCelulaBp");
  const comoIdentificadoBp = document.getElementById("comoIdentificadoBp");
  const bpStatus = document.getElementById("bpStatus");

  // ==========================================================
  // 12. CAMPOS DE TEXTO PRINCIPAIS
  // ==========================================================
  const resumoInput = document.getElementById("resumo");
  const descricaoInicialInput = document.getElementById("descricaoInicial");

  // ==========================================================
  // 13. ESTADO DA TELA
  // ==========================================================
  let fornecedorAtual = null;
  let linhaPnAtual = null;

  // ==========================================================
  // 14. BASE LOCAL TEMPORÁRIA DE FORNECEDORES
  // OBS:
  // Mantida apenas como apoio local / histórico.
  // O fluxo oficial usa a API.
  // ==========================================================
  const fornecedoresBase = [
    { duns: "000104992", nome: "Alcom - USA", tipoFornecedor: "IMPORTADO" },
    { duns: "000123943", nome: "DELPHI-S", tipoFornecedor: "IMPORTADO" },
    { duns: "000139956", nome: "Hella - DE", tipoFornecedor: "IMPORTADO" },
    { duns: "000140640", nome: "Receptec", tipoFornecedor: "IMPORTADO" },
    { duns: "000161521", nome: "Dalian Alps", tipoFornecedor: "IMPORTADO" },
    { duns: "000173872", nome: "Cadimex AS", tipoFornecedor: "IMPORTADO" },
    { duns: "000180208", nome: "Intier Autom", tipoFornecedor: "IMPORTADO" },
    { duns: "000187039", nome: "Arkal - ES", tipoFornecedor: "IMPORTADO" },
    { duns: "000198440", nome: "American Axl - USA", tipoFornecedor: "IMPORTADO" },
    { duns: "000222091", nome: "GPS - SJC", tipoFornecedor: "LOCAL" },
    { duns: "000222117", nome: "GM do Brasil", tipoFornecedor: "LOCAL" },
    { duns: "000236455", nome: "Nidec Mobility - Mexico", tipoFornecedor: "IMPORTADO" },
    { duns: "000245043", nome: "Werner-von-siemens", tipoFornecedor: "IMPORTADO" },
    { duns: "000249094", nome: "Delphi Hong Kong", tipoFornecedor: "IMPORTADO" },
    { duns: "000782768", nome: "HIRSCHMANN", tipoFornecedor: "IMPORTADO" },
    { duns: "897240370", nome: "Inylbra", tipoFornecedor: "LOCAL" }
  ];

  // ==========================================================
  // 15. UTILITÁRIOS GERAIS
  // ==========================================================
  function normalizarNumero(valor) {
    return String(valor || "").replace(/\D/g, "").trim();
  }

  function validarPn(pn) {
    return /^\d{8}$/.test(pn);
  }

  function validarDunsFormato(duns) {
    return /^\d{9}$/.test(duns);
  }

  function dunsEhSequenciaRepetida(duns) {
    return /^(\d)\1{8}$/.test(duns);
  }

  function obterFornecedorPorDuns(duns) {
    return fornecedoresBase.find((item) => item.duns === duns) || null;
  }

  function formatarDataPtBr(dataValor) {
    if (!dataValor) return "--/--/----";
    const data = new Date(dataValor);
    if (Number.isNaN(data.getTime())) {
      return "--/--/----";
    }
    return data.toLocaleDateString("pt-BR");
  }

  function formatarDataParaExibicao(valor) {
    if (!valor) return "--/--/----";
    const data = new Date(valor);
    if (Number.isNaN(data.getTime())) {
      return "--/--/----";
    }
    return data.toLocaleDateString("pt-BR");
  }

  function normalizarVin(valor) {
    return String(valor || "")
      .toUpperCase()
      .replace(/\s/g, "")
      .replace(/[^A-Z0-9]/g, "")
      .replace(/[IOQ]/g, "")
      .slice(0, 17);
  }

  function vinEhValido(valor) {
    return /^[A-HJ-NPR-Z0-9]{17}$/.test(valor);
  }

  // ==========================================================
  // 16. MENSAGEM DE RETORNO DO RASP
  // ==========================================================
  function mostrarMensagemRasp(texto) {
    if (!mensagemRasp) return;
    mensagemRasp.textContent = texto;
    mensagemRasp.classList.remove("oculto");
  }

  function ocultarMensagemRasp() {
    if (!mensagemRasp) return;
    mensagemRasp.textContent = "";
    mensagemRasp.classList.add("oculto");
  }

  // ==========================================================
  // 17. INDICADOR VISUAL DO RASP
  // ==========================================================
  function atualizarNumeroRaspDisplay(numeroRasp) {
    const numeroFormatado = numeroRasp || "---";
    if (raspNumeroDisplay) {
      raspNumeroDisplay.textContent = numeroFormatado;
    }
    if (numeroRaspInfo) {
      numeroRaspInfo.textContent = numeroFormatado;
    }
  }

  function atualizarDataCriacaoRaspDisplay(dataCriacao) {
    const texto = dataCriacao ? formatarDataParaExibicao(dataCriacao) : "--/--/----";
    if (dataCriacaoRaspDisplay) {
      dataCriacaoRaspDisplay.textContent = texto;
    }
    if (dataCriacaoRaspInfo) {
      dataCriacaoRaspInfo.textContent = texto;
    }
  }

  function obterNumeroRaspAtual() {
    if (!raspNumeroDisplay) return "";
    return (raspNumeroDisplay.textContent || "").trim();
  }

  function destacarCopiaRasp() {
    if (!raspIndicadorBox) return;
    raspIndicadorBox.classList.add("copiado");
    setTimeout(() => {
      raspIndicadorBox.classList.remove("copiado");
    }, 1000);
  }

  async function copiarTexto(texto) {
    try {
      if (navigator.clipboard && window.isSecureContext) {
        await navigator.clipboard.writeText(texto);
        return true;
      }
    } catch (error) {
      console.warn("Falha no clipboard moderno, tentando fallback:", error);
    }

    try {
      const textArea = document.createElement("textarea");
      textArea.value = texto;
      textArea.style.position = "fixed";
      textArea.style.left = "-9999px";
      textArea.style.top = "0";
      document.body.appendChild(textArea);
      textArea.focus();
      textArea.select();
      const copiou = document.execCommand("copy");
      document.body.removeChild(textArea);
      return copiou;
    } catch (error) {
      console.error("Falha também no fallback de cópia:", error);
      return false;
    }
  }

  // ==========================================================
  // 18. CONTROLE DAS ABAS
  // ==========================================================
  tabs.forEach((tab) => {
    tab.addEventListener("click", () => {
      const target = tab.dataset.tab;

      tabs.forEach((t) => t.classList.remove("active"));
      tabContents.forEach((c) => c.classList.remove("active"));

      tab.classList.add("active");

      const alvo = document.getElementById(`tab-${target}`);
      if (alvo) {
        alvo.classList.add("active");
      }
    });
  });

  // ==========================================================
  // 19. INICIALIZAÇÃO DOS CAMPOS FIXOS
  // ==========================================================
  function inicializarCamposFixos() {
    if (analistaInput) analistaInput.value = NOME_USUARIO_LOGADO;
    if (statusInicialInput) statusInicialInput.value = "Em análise";

    if (nomeContatoInput && !nomeContatoInput.value.trim()) {
      nomeContatoInput.value = "Não Contatado";
      nomeContatoInput.classList.add("input-placeholder");
    }

    if (dataContatoInput) {
      dataContatoInput.required = false;
    }

    atualizarNumeroRaspDisplay("");
    atualizarDataCriacaoRaspDisplay("");
  }

  // ==========================================================
  // 20. STATUS VISUAL - DUNS / PN / BP
  // ==========================================================
  function definirStatusDuns(mensagem, tipo) {
    if (!dunsStatus) return;

    dunsStatus.textContent = mensagem;
    dunsStatus.className = "duns-status";

    if (tipo === "success") {
      dunsStatus.classList.add("duns-status-success");
      return;
    }

    if (tipo === "error") {
      dunsStatus.classList.add("duns-status-error");
      return;
    }

    dunsStatus.classList.add("duns-status-neutral");
  }

  function definirStatusPn(mensagem, tipo) {
    if (!pnStatus) return;

    pnStatus.textContent = mensagem;
    pnStatus.className = "pn-status";

    if (tipo === "success") {
      pnStatus.classList.add("pn-status-success");
      return;
    }

    if (tipo === "error") {
      pnStatus.classList.add("pn-status-error");
      return;
    }

    pnStatus.classList.add("pn-status-neutral");
  }

  function definirStatusBp(mensagem, tipo) {
    if (!bpStatus) return;

    bpStatus.textContent = mensagem;
    bpStatus.className = "pn-status";

    if (tipo === "error") {
      bpStatus.classList.add("pn-status-error");
    } else if (tipo === "success") {
      bpStatus.classList.add("pn-status-success");
    } else {
      bpStatus.classList.add("pn-status-neutral");
    }
  }

  // ==========================================================
  // 21. CARGA DE DOMÍNIOS - FUNÇÕES BASE
  // ==========================================================
  function limparOpcoesSelect(selectElement, placeholder = "Selecione") {
    if (!selectElement) return;

    selectElement.innerHTML = "";

    const option = document.createElement("option");
    option.value = "";
    option.textContent = placeholder;
    selectElement.appendChild(option);
  }

  function preencherSelectComChaves(selectElement, itens, valueKey, textKey) {
    if (!selectElement) return;

    itens.forEach((item) => {
      const option = document.createElement("option");
      option.value = item[valueKey] ?? "";
      option.textContent = item[textKey] ?? "Sem descrição";
      selectElement.appendChild(option);
    });
  }

  async function carregarDominioSelect(url, selectElement, valueKey, textKey, nomeDominio) {
    if (!selectElement) {
      console.warn(`Select de ${nomeDominio} não encontrado no HTML.`);
      return;
    }

    limparOpcoesSelect(selectElement);

    try {
      const response = await fetch(url);

      if (!response.ok) {
        throw new Error(`Erro HTTP ${response.status} ao carregar ${nomeDominio}`);
      }

      const itens = await response.json();

      if (!Array.isArray(itens)) {
        throw new Error(`Resposta inválida para ${nomeDominio}.`);
      }

      preencherSelectComChaves(selectElement, itens, valueKey, textKey);
    } catch (error) {
      console.error(`Erro ao carregar ${nomeDominio}:`, error);
      limparOpcoesSelect(selectElement, `Erro ao carregar ${nomeDominio}`);
    }
  }

  function selecionarMajorPadrao() {
    if (!majorRaspSelect) return;

    const opcoes = [...majorRaspSelect.options];
    const opcaoNa = opcoes.find((option) => {
      const texto = (option.textContent || "").trim().toUpperCase();
      return texto === "N/A";
    });

    if (opcaoNa) {
      majorRaspSelect.value = opcaoNa.value;
    }
  }

  // ==========================================================
  // 22. CARGA DE DOMÍNIOS - SEÇÃO 5
  // ==========================================================
  async function carregarGmAliado() {
    if (!gmAliadoSelect) return;

    try {
      const response = await fetch(`${API_BASE_URL}/gm-aliado-rasp`);

      if (!response.ok) {
        throw new Error("Não foi possível carregar GM aliado.");
      }

      const lista = await response.json();
      gmAliadoSelect.innerHTML = '<option value="">Selecione</option>';

      lista.forEach((item) => {
        const option = document.createElement("option");
        option.value = item.idGmAliadoRasp ?? item.id_gm_aliado_rasp ?? "";
        option.textContent = item.descricao ?? "";
        gmAliadoSelect.appendChild(option);
      });
    } catch (error) {
      console.error("Erro ao carregar GM aliado:", error);
      gmAliadoSelect.innerHTML = '<option value="">Erro ao carregar</option>';
    }
  }

  async function carregarDominiosComplementares() {
    await Promise.all([
      carregarDominioSelect(
        `${API_BASE_URL}/modelo-veiculo-rasp`,
        modeloVeiculoSelect,
        "idModeloVeiculoRasp",
        "nomeModelo",
        "modelo do veículo"
      ),
      carregarDominioSelect(
        `${API_BASE_URL}/turno-rasp`,
        turnoRaspSelect,
        "idTurnoRasp",
        "descricao",
        "turno"
      ),
      carregarDominioSelect(
        `${API_BASE_URL}/piloto-rasp`,
        pilotoRaspSelect,
        "idPilotoRasp",
        "descricao",
        "piloto"
      ),
      carregarDominioSelect(
        `${API_BASE_URL}/major-rasp`,
        majorRaspSelect,
        "idMajorRasp",
        "descricao",
        "major"
      ),
      carregarGmAliado()
    ]);

    selecionarMajorPadrao();
  }

  // ==========================================================
  // 23. REGRA DO CAMPO CONTATO
  // ==========================================================
  function validarRegraContato() {
    if (!nomeContatoInput || !dataContatoInput) return;

    const nomeContato = nomeContatoInput.value.trim().toLowerCase();
    const naoContatado =
      nomeContato === "não contatado" ||
      nomeContato === "nao contatado" ||
      nomeContato === "";

    dataContatoInput.required = !naoContatado;

    if (!naoContatado) {
      dataContatoInput.setAttribute("required", "required");
    } else {
      dataContatoInput.removeAttribute("required");
    }
  }

  // ==========================================================
  // 24. MODAL - PN
  // ==========================================================
  function abrirModalPn(pn, linha) {
    if (!modalPn || !modalPnCodigo || !modalPnDescricao) return;

    linhaPnAtual = linha;
    modalPnCodigo.value = pn;
    modalPnDescricao.value = "";
    modalPn.classList.remove("hidden");
  }

  function fecharModalPn() {
    if (!modalPn) return;
    modalPn.classList.add("hidden");
  }

  // ==========================================================
  // 25. MODAL - FORNECEDOR
  // ==========================================================
  function abrirModalFornecedor(duns) {
    if (!modalFornecedor || !modalFornecedorDuns || !modalFornecedorNome || !modalFornecedorTipo) return;

    modalFornecedorDuns.value = duns;
    modalFornecedorNome.value = "";
    modalFornecedorTipo.value = "";
    modalFornecedor.classList.remove("hidden");
  }

  function fecharModalFornecedor() {
    if (!modalFornecedor) return;
    modalFornecedor.classList.add("hidden");
  }

  // ==========================================================
  // 26. API - FORNECEDOR
  // ==========================================================
  async function buscarFornecedorNaApi(duns) {
    const url = `${API_BASE_URL}/fornecedor-rasp/duns/${duns}`;

    try {
      const response = await fetch(url);

      if (response.status === 404) {
        return { ok: false, motivo: "nao_encontrado" };
      }

      if (!response.ok) {
        return { ok: false, motivo: "erro_api" };
      }

      const fornecedor = await response.json();
      return { ok: true, fornecedor };
    } catch (error) {
      console.error("Erro ao consultar fornecedor na API:", error);
      return { ok: false, motivo: "falha_conexao" };
    }
  }

  // ==========================================================
  // 27. API - PN
  // ==========================================================
  async function buscarPnNaApi(pn) {
    const url = `${API_BASE_URL}/pn-rasp/codigo/${pn}`;

    try {
      const response = await fetch(url);

      if (response.status === 404) {
        return { ok: false, motivo: "nao_encontrado" };
      }

      if (!response.ok) {
        return { ok: false, motivo: "erro_api" };
      }

      const pnData = await response.json();
      return { ok: true, pn: pnData };
    } catch (error) {
      console.error("Erro ao consultar PN na API:", error);
      return { ok: false, motivo: "falha_conexao" };
    }
  }

// ==========================================================
// 28. APROVADOR FT POR TURNO (VERSÃO FINAL)
// ==========================================================

async function buscarFtPorTurno(idTurno) {
  try {
    const response = await fetch(`${API_BASE_URL}/usuarios`);

    if (!response.ok) {
      throw new Error("Erro ao carregar usuários");
    }

    const usuarios = await response.json();

    const ft = usuarios.find(u =>
      Number(u.idPerfil) === 3 &&
      Number(u.idTurnoRasp) === Number(idTurno) &&
      u.ativo === true
    );

    return ft || null;

  } catch (error) {
    console.error("Erro buscar FT:", error);
    return null;
  }
}

async function atualizarAprovadorFtPorTurno() {
  if (!turnoRaspSelect || !aprovadorFtInput || !aprovadorFtIdInput) return;

  const idTurno = turnoRaspSelect.value;

  if (!idTurno) {
    aprovadorFtInput.value = "";
    aprovadorFtIdInput.value = "";
    return;
  }

  // 🔄 feedback visual
  aprovadorFtInput.value = "Buscando FT...";

  const ft = await buscarFtPorTurno(idTurno);

  if (ft) {
    aprovadorFtInput.value = ft.nome;
    aprovadorFtIdInput.value = ft.idUsuario;
  } else {
    aprovadorFtInput.value = "Nenhum FT vinculado ao turno";
    aprovadorFtIdInput.value = "";
  }
}

  // ==========================================================
  // 29. SALVAR PN VIA MODAL
  // ==========================================================
  async function salvarPnViaModal() {
    if (!modalPnCodigo || !modalPnDescricao || !linhaPnAtual) return;

    const pn = modalPnCodigo.value;
    const descricao = modalPnDescricao.value.trim();

    if (!descricao) {
      alert("Informe a descrição do PN.");
      return;
    }

    try {
      const response = await fetch(`${API_BASE_URL}/pn-rasp`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          codigoPn: pn,
          nomePeca: descricao
        })
      });

      if (!response.ok) {
        throw new Error("Erro ao cadastrar PN");
      }

      const novoPn = await response.json();

      linhaPnAtual.querySelector(".pn-id").value = novoPn.idPn ?? novoPn.id_pn ?? "";
      linhaPnAtual.querySelector(".pn-descricao").value =
        novoPn.nomePeca ?? novoPn.nome_peca ?? "";

      fecharModalPn();
      aplicarValidacoesPnEmTela();

      alert("PN cadastrado com sucesso!");
    } catch (error) {
      console.error(error);
      alert("Erro ao cadastrar PN.");
    }
  }

  // ==========================================================
  // 30. SALVAR FORNECEDOR VIA MODAL
  // ==========================================================
  async function salvarFornecedorViaModal() {
    if (!modalFornecedorDuns || !modalFornecedorNome || !modalFornecedorTipo) return;

    const duns = modalFornecedorDuns.value.trim();
    const nome = modalFornecedorNome.value.trim();
    const tipoFornecedor = modalFornecedorTipo.value.trim();

    if (!nome) {
      alert("Informe o nome do fornecedor.");
      return;
    }

    if (!tipoFornecedor) {
      alert("Selecione o tipo do fornecedor.");
      return;
    }

    try {
      const response = await fetch(`${API_BASE_URL}/fornecedor-rasp`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          duns,
          nome,
          tipoFornecedor,
          ativo: true
        })
      });

      if (!response.ok) {
        throw new Error("Erro ao cadastrar fornecedor");
      }

      const novoFornecedor = await response.json();
      fornecedorAtual = novoFornecedor;

      if (nomeFornecedorInput) {
        nomeFornecedorInput.value = novoFornecedor.nome ?? "";
      }

      if (tipoFornecedorInput) {
        tipoFornecedorInput.value = novoFornecedor.tipoFornecedor ?? novoFornecedor.tipo ?? "";
      }

      definirStatusDuns(
        `Fornecedor localizado com sucesso: ${nomeFornecedorInput?.value ?? ""} (${tipoFornecedorInput?.value ?? ""}).`,
        "success"
      );

      fecharModalFornecedor();
      alert("Fornecedor cadastrado com sucesso!");
    } catch (error) {
      console.error(error);
      alert("Erro ao cadastrar fornecedor.");
    }
  }

  // ==========================================================
  // 31. LIMPEZA / STATUS - FORNECEDOR
  // ==========================================================
  function limparFornecedorDerivado() {
    if (nomeFornecedorInput) nomeFornecedorInput.value = "";
    if (tipoFornecedorInput) tipoFornecedorInput.value = "";
    fornecedorAtual = null;
  }

  // ==========================================================
  // 32. PROCESSAMENTO DO DUNS
  // ==========================================================
  async function processarDuns() {
    if (!dunsInput) {
      return { ok: false, motivo: "campo_ausente" };
    }

    const duns = normalizarNumero(dunsInput.value);
    dunsInput.value = duns;

    limparFornecedorDerivado();

    if (!duns) {
      definirStatusDuns(
        "Informe um DUNS válido para localizar o fornecedor.",
        "neutral"
      );
      return { ok: false, motivo: "vazio" };
    }

    if (!validarDunsFormato(duns)) {
      definirStatusDuns(
        "O DUNS deve conter exatamente 9 dígitos numéricos.",
        "error"
      );
      return { ok: false, motivo: "formato" };
    }

    if (dunsEhSequenciaRepetida(duns)) {
      definirStatusDuns(
        "DUNS inválido. Sequências repetidas como 000000000, 111111111 e semelhantes não são permitidas.",
        "error"
      );
      return { ok: false, motivo: "sequencia" };
    }

    const resultadoApi = await buscarFornecedorNaApi(duns);

    if (!resultadoApi.ok) {
      if (resultadoApi.motivo === "nao_encontrado") {
        definirStatusDuns(
          "DUNS não encontrado no cadastro. Complete o cadastro para continuar.",
          "error"
        );
        abrirModalFornecedor(duns);
        return { ok: false, motivo: "nao_encontrado" };
      }

      if (resultadoApi.motivo === "falha_conexao") {
        definirStatusDuns(
          "Não foi possível conectar à API no momento.",
          "error"
        );
        return { ok: false, motivo: "falha_conexao" };
      }

      definirStatusDuns("Erro ao consultar fornecedor na API.", "error");
      return { ok: false, motivo: "erro_api" };
    }

    const fornecedor = resultadoApi.fornecedor;
    fornecedorAtual = fornecedor;

    if (nomeFornecedorInput) {
      nomeFornecedorInput.value = fornecedor.nomeFornecedor ?? fornecedor.nome ?? "";
    }

    if (tipoFornecedorInput) {
      tipoFornecedorInput.value = fornecedor.tipoFornecedor ?? fornecedor.tipo ?? "";
    }

    definirStatusDuns(
      `Fornecedor localizado com sucesso: ${nomeFornecedorInput?.value ?? ""} (${tipoFornecedorInput?.value ?? ""}).`,
      "success"
    );

    return {
      ok: true,
      motivo: "encontrado",
      fornecedor
    };
  }

  // ==========================================================
  // 33. TABELA DE PN - UTILITÁRIOS
  // ==========================================================
  function obterRowsPn() {
    return [...document.querySelectorAll(".pn-row")];
  }

  function garantirPnPrincipal() {
    const rows = obterRowsPn();
    const algumMarcado = rows.some((row) =>
      row.querySelector(".pn-principal").checked
    );

    if (!algumMarcado && rows.length > 0) {
      rows[0].querySelector(".pn-principal").checked = true;
    }
  }

  function criarLinhaPn({
    principal = false,
    pn = "",
    dataLoteInicial = "",
    qtdSuspeita = 0,
    qtdChecada = 0,
    qtdRejeitada = 0
  } = {}) {
    const tr = document.createElement("tr");
    tr.className = "pn-row";

    tr.innerHTML = `
      <td class="center-cell">
        <input
          type="radio"
          name="pnPrincipal"
          class="pn-principal"
          ${principal ? "checked" : ""}
        />
      </td>
      <td>
        <input
          type="text"
          class="pn-input"
          placeholder="8 dígitos"
          value="${pn}"
        />
        <input
          type="hidden"
          class="pn-id"
          value=""
        />
      </td>
      <td>
        <input
          type="text"
          class="pn-descricao system-field"
          value=""
          placeholder="Descrição do PN"
          readonly
        />
      </td>
      <td>
        <input
          type="text"
          class="data-lote-inicial"
          placeholder="Ex.: 12/03/26"
          inputmode="numeric"
          maxlength="8"
          value="${dataLoteInicial}"
        />
      </td>
      <td>
        <input
          type="number"
          class="qtd-suspeita qtd-compacta"
          min="0"
          step="1"
          value="${qtdSuspeita}"
        />
      </td>
      <td>
        <input
          type="number"
          class="qtd-checada qtd-compacta"
          min="0"
          step="1"
          value="${qtdChecada}"
        />
      </td>
      <td>
        <input
          type="number"
          class="qtd-rejeitada qtd-compacta"
          min="0"
          step="1"
          value="${qtdRejeitada}"
        />
      </td>
      <td class="center-cell">
        <button
          type="button"
          class="remove-row icon-btn"
          title="Remover linha"
          aria-label="Remover linha"
        >
          ×
        </button>
      </td>
    `;

    return tr;
  }

  function coletarPns() {
    const rows = obterRowsPn();

    return rows
      .map((row) => {
        const principal = row.querySelector(".pn-principal").checked;
        const pn = normalizarNumero(row.querySelector(".pn-input").value);
        const idPn = row.querySelector(".pn-id")?.value
          ? Number(row.querySelector(".pn-id").value)
          : null;
        const dataLoteInicial = row.querySelector(".data-lote-inicial").value.trim();
        const qtdSuspeitaInicial = Number(row.querySelector(".qtd-suspeita").value || 0);
        const qtdChecadaInicial = Number(row.querySelector(".qtd-checada").value || 0);
        const qtdRejeitadaInicial = Number(row.querySelector(".qtd-rejeitada").value || 0);

        return {
          principal,
          pn,
          idPn,
          dataLoteInicial,
          qtdSuspeitaInicial,
          qtdChecadaInicial,
          qtdRejeitadaInicial
        };
      })
      .filter((item) => item.pn !== "");
  }

  function limparPnDerivadoDaLinha(linha) {
    const pnIdInput = linha.querySelector(".pn-id");
    const pnDescricaoInput = linha.querySelector(".pn-descricao");

    if (pnIdInput) {
      pnIdInput.value = "";
    }

    if (pnDescricaoInput) {
      pnDescricaoInput.value = "";
    }
  }

  async function processarPnDaLinha(linha) {
    const pnInput = linha.querySelector(".pn-input");
    const pnIdInput = linha.querySelector(".pn-id");
    const pnDescricaoInput = linha.querySelector(".pn-descricao");

    const pn = normalizarNumero(pnInput.value).slice(0, 8);
    pnInput.value = pn;

    limparPnDerivadoDaLinha(linha);

    if (!pn) {
      return { ok: false, motivo: "vazio" };
    }

    if (!validarPn(pn)) {
      return { ok: false, motivo: "formato" };
    }

    const resultadoApi = await buscarPnNaApi(pn);

    if (!resultadoApi.ok) {
      if (resultadoApi.motivo === "nao_encontrado") {
        abrirModalPn(pn, linha);
      }
      return resultadoApi;
    }

    const pnData = resultadoApi.pn;

    pnIdInput.value = pnData.idPn ?? pnData.id_pn ?? "";
    pnDescricaoInput.value =
      pnData.nomePeca ?? pnData.nome_peca ?? pnData.descricao ?? "";

    return {
      ok: true,
      motivo: "encontrado",
      pn: pnData
    };
  }

  function limparTabelaPn() {
    if (!pnTableBody) return;

    pnTableBody.innerHTML = "";
    pnTableBody.appendChild(criarLinhaPn({ principal: true }));

    aplicarValidacoesPnEmTela();
  }

  // ==========================================================
  // 34. ESTILO DE ERRO VISUAL NOS CAMPOS
  // ==========================================================
  function destacarErroCampo(input, comErro) {
    if (!input) return;

    if (comErro) {
      input.style.borderColor = "#b02a37";
      input.style.backgroundColor = "#fff5f5";
    } else {
      input.style.borderColor = "";
      input.style.backgroundColor = "";
    }
  }

  // ==========================================================
  // 35. NORMALIZA INPUTS DA LINHA DE PN
  // ==========================================================
  function normalizarInputsPnDaLinha(linha) {
    const pnInput = linha.querySelector(".pn-input");
    const qtdSuspeitaInput = linha.querySelector(".qtd-suspeita");
    const qtdChecadaInput = linha.querySelector(".qtd-checada");
    const qtdRejeitadaInput = linha.querySelector(".qtd-rejeitada");

    pnInput.value = normalizarNumero(pnInput.value).slice(0, 8);

    [qtdSuspeitaInput, qtdChecadaInput, qtdRejeitadaInput].forEach((input) => {
      const valorNormalizado = normalizarNumero(input.value);
      input.value = valorNormalizado === "" ? "0" : valorNormalizado;
    });
  }

  // ==========================================================
  // 36. VALIDAÇÕES DA TABELA DE PN
  // ==========================================================
  function validarDuplicidadePnEmTela() {
    const rows = obterRowsPn();
    const mapa = new Map();

    rows.forEach((row) => {
      const pnInput = row.querySelector(".pn-input");
      const pn = normalizarNumero(pnInput.value);

      destacarErroCampo(pnInput, false);

      if (!pn) return;

      if (!mapa.has(pn)) {
        mapa.set(pn, []);
      }

      mapa.get(pn).push(pnInput);
    });

    mapa.forEach((inputs) => {
      if (inputs.length > 1) {
        inputs.forEach((input) => destacarErroCampo(input, true));
      }
    });
  }

  function validarPnPrincipalEmTela() {
    const rows = obterRowsPn();

    rows.forEach((row) => {
      const radioPrincipal = row.querySelector(".pn-principal");
      const dataLoteInput = row.querySelector(".data-lote-inicial");

      if (radioPrincipal.checked && !dataLoteInput.value.trim()) {
        destacarErroCampo(dataLoteInput, true);
      } else {
        destacarErroCampo(dataLoteInput, false);
      }
    });
  }

  function aplicarValidacoesPnEmTela() {
    const rows = obterRowsPn();

    rows.forEach((row) => {
      normalizarInputsPnDaLinha(row);
    });

    validarDuplicidadePnEmTela();
    validarPnPrincipalEmTela();

    const pns = coletarPns();

    if (pns.length === 0) {
      definirStatusPn(
        "Preencha pelo menos 1 PN. O PN principal deve ter Data/Lote inicial.",
        "neutral"
      );
      return;
    }

    const pnsUnicos = new Set();
    let temDuplicado = false;
    let principalSemLote = false;
    let temPnInvalido = false;
    let temDataInvalida = false;

    pns.forEach((item) => {
      if (!validarPn(item.pn)) {
        temPnInvalido = true;
      }

      if (item.pn && !item.idPn) {
        temPnInvalido = true;
      }

      if (pnsUnicos.has(item.pn)) {
        temDuplicado = true;
      } else {
        pnsUnicos.add(item.pn);
      }

      if (item.principal && !item.dataLoteInicial) {
        principalSemLote = true;
      }

      if (item.dataLoteInicial && !dataInicialEhValida(item.dataLoteInicial)) {
        temDataInvalida = true;
      }
    });

    if (temDuplicado) {
      definirStatusPn(
        "Existem PNs duplicados na tabela. Cada PN deve aparecer apenas uma vez.",
        "error"
      );
      return;
    }

    if (temPnInvalido) {
      definirStatusPn(
        "Existe PN inválido ou não cadastrado. Cada PN deve conter 8 dígitos numéricos e existir no cadastro mestre.",
        "error"
      );
      return;
    }

    if (principalSemLote) {
      definirStatusPn(
        "O PN principal precisa ter a Data/Lote inicial preenchida.",
        "error"
      );
      return;
    }

    if (temDataInvalida) {
      definirStatusPn(
        "Existe Data/Lote inicial inválida na tabela. Use o formato DD/MM/AA.",
        "error"
      );
      return;
    }

    definirStatusPn("PN(s) preenchido(s) corretamente.", "success");
  }

  // ==========================================================
  // 37. BP - CONTROLE DE VIN x LOCAL
  // ==========================================================
  function controlarTipoBp() {
    const tipo = tipoReferenciaBp?.value ?? "";

    if (vinBp) vinBp.value = "";
    if (localCelulaBp) localCelulaBp.value = "";

    if (tipo === "VIN") {
      if (vinBp) vinBp.disabled = false;
      if (localCelulaBp) localCelulaBp.disabled = true;
    } else if (tipo === "LOCAL") {
      if (vinBp) vinBp.disabled = true;
      if (localCelulaBp) localCelulaBp.disabled = false;
    } else {
      if (vinBp) vinBp.disabled = true;
      if (localCelulaBp) localCelulaBp.disabled = true;
    }
  }

  function validarBpEmTela() {
    const tipo = tipoReferenciaBp?.value ?? "";
    const data = dataBp?.value ?? "";
    const hora = horaBp?.value ?? "";
    const vin = vinBp?.value.trim() ?? "";
    const local = localCelulaBp?.value.trim() ?? "";
    const como = comoIdentificadoBp?.value.trim() ?? "";

    if (!tipo && !data && !hora && !vin && !local && !como) {
      definirStatusBp("BP não informado.", "neutral");
      return;
    }

    if (!tipo) {
      definirStatusBp("Selecione o tipo de referência do BP.", "error");
      return;
    }

    if (!data || !hora) {
      definirStatusBp("Data e hora do BP são obrigatórias.", "error");
      return;
    }

    if (tipo === "VIN" && !vin) {
      definirStatusBp("Informe o VIN para BP do tipo VIN.", "error");
      return;
    }

    if (tipo === "VIN" && !vinEhValido(vin)) {
      definirStatusBp("VIN inválido. Informe 17 caracteres alfanuméricos válidos.", "error");
      return;
    }

    if (tipo === "LOCAL" && !local) {
      definirStatusBp("Informe a Área / Célula para BP do tipo LOCAL.", "error");
      return;
    }

    if (!como) {
      definirStatusBp("Descreva como o problema foi identificado.", "error");
      return;
    }

    definirStatusBp("BP preenchido corretamente.", "success");
  }

  function coletarBp() {
    return {
      tipoReferenciaBp: tipoReferenciaBp?.value ?? "",
      dataBp: dataBp?.value ?? "",
      horaBp: horaBp?.value ?? "",
      vin: vinBp?.value.trim() || null,
      localCelula: localCelulaBp?.value.trim() || null,
      comoIdentificado: comoIdentificadoBp?.value.trim() ?? ""
    };
  }

  // ==========================================================
  // 38. FORMATAÇÃO E VALIDAÇÃO DE DATA INICIAL (DD/MM/AA)
  // ==========================================================
  function formatarValorDataLote(valor) {
    let somenteNumeros = String(valor || "").replace(/\D/g, "");

    if (somenteNumeros.length > 6) {
      somenteNumeros = somenteNumeros.slice(0, 6);
    }

    if (somenteNumeros.length >= 5) {
      return somenteNumeros.replace(/(\d{2})(\d{2})(\d{1,2})/, "$1/$2/$3");
    }

    if (somenteNumeros.length >= 3) {
      return somenteNumeros.replace(/(\d{2})(\d{1,2})/, "$1/$2");
    }

    return somenteNumeros;
  }

  function dataInicialEhValida(valor) {
    if (!valor) return false;

    const match = valor.match(/^(\d{2})\/(\d{2})\/(\d{2})$/);
    if (!match) return false;

    const dia = Number(match[1]);
    const mes = Number(match[2]);
    const ano = Number(match[3]);

    if (mes < 1 || mes > 12) return false;
    if (dia < 1) return false;

    const anoCompleto = 2000 + ano;
    const ultimoDiaDoMes = new Date(anoCompleto, mes, 0).getDate();

    return dia <= ultimoDiaDoMes;
  }

  function obterMensagemErroData(campo) {
    return campo.parentElement?.querySelector(".mensagem-erro-campo");
  }

  function mostrarErroDataInicial(campo, mensagem) {
    if (!campo) return;

    campo.classList.add("input-erro");

    let mensagemErro = obterMensagemErroData(campo);
    if (!mensagemErro) {
      mensagemErro = document.createElement("div");
      mensagemErro.className = "mensagem-erro-campo";
      campo.parentElement?.appendChild(mensagemErro);
    }

    mensagemErro.textContent = mensagem;
  }

  function limparErroDataInicial(campo) {
    if (!campo) return;

    campo.classList.remove("input-erro");

    const mensagemErro = obterMensagemErroData(campo);
    if (mensagemErro) {
      mensagemErro.remove();
    }
  }

  function aplicarMascaraDataLote() {
    document.addEventListener("input", (event) => {
      const campo = event.target;

      if (!campo.classList.contains("data-lote-inicial")) {
        return;
      }

      campo.value = formatarValorDataLote(campo.value);

      if (campo.value.length === 8 && dataInicialEhValida(campo.value)) {
        limparErroDataInicial(campo);
      }
    });

    document.addEventListener(
      "blur",
      (event) => {
        const campo = event.target;

        if (!campo.classList.contains("data-lote-inicial")) {
          return;
        }

        if (!campo.value) {
          limparErroDataInicial(campo);
          return;
        }

        if (!dataInicialEhValida(campo.value)) {
          mostrarErroDataInicial(campo, "Data inválida. Use o formato DD/MM/AA.");
          return;
        }

        limparErroDataInicial(campo);
      },
      true
    );
  }

  // ==========================================================
  // 39. LIMPEZA COMPLETA DA TELA
  // ==========================================================
  async function limparFormularioParaNovoRasp() {
    if (form) form.reset();

    if (analistaInput) analistaInput.value = NOME_USUARIO_LOGADO;
    if (statusInicialInput) statusInicialInput.value = "Em análise";

    limparFornecedorDerivado();

    definirStatusDuns(
      "Informe um DUNS válido para localizar o fornecedor.",
      "neutral"
    );

    definirStatusPn(
      "Preencha pelo menos 1 PN. O PN principal deve ter Data/Lote inicial.",
      "neutral"
    );

    limparTabelaPn();

    if (massPanel) massPanel.classList.remove("show");
    if (pnsLoteTextarea) pnsLoteTextarea.value = "";

    atualizarNumeroRaspDisplay("");
    atualizarDataCriacaoRaspDisplay("");

    if (nomeContatoInput) {
      nomeContatoInput.value = "Não Contatado";
      nomeContatoInput.classList.add("input-placeholder");
    }

    if (dataContatoInput) dataContatoInput.value = "";
    if (rdNumeroInput) rdNumeroInput.value = "";
    if (campanhaNumeroInput) campanhaNumeroInput.value = "";

    if (aprovadorFtInput) aprovadorFtInput.value = "";
    if (aprovadorFtIdInput) aprovadorFtIdInput.value = "";

    validarRegraContato();
    ocultarMensagemRasp();

    await carregarDominiosComplementares();
    await atualizarAprovadorFtPorTurno();

    controlarTipoBp();
    validarBpEmTela();
  }

  // ==========================================================
  // 40. SALVA OS PNs VINCULADOS AO RASP
  // ==========================================================
  async function salvarPnsDoRasp(idRasp, pns, dunsFornecedor) {
    const promises = pns.map(async (item, i) => {
      const payloadPn = {
        idRasp: idRasp,
        pn: item.pn,
        quantidadeSuspeita: item.qtdSuspeitaInicial,
        quantidadeChecada: item.qtdChecadaInicial,
        quantidadeRejeitada: item.qtdRejeitadaInicial,
        emContencao: false,
        duns: dunsFornecedor,
        ordemExibicao: i + 1
      };

      const response = await fetch(`${API_BASE_URL}/rasp-pn`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(payloadPn)
      });

      if (!response.ok) {
        const mensagemErro = await response.text();
        console.error("Erro API PN:", mensagemErro);
        throw new Error(`Erro ao salvar PN ${item.pn}. Detalhe: ${mensagemErro}`);
      }

      return response.json();
    });

    return await Promise.all(promises);
  }

  // ==========================================================
  // 41. ATUALIZA CAMPOS COMPLEMENTARES NO RASCUNHO
  // OBS:
  // Este payload foi ampliado com os novos campos da Seção 5.
  // Se algum campo ainda não estiver mapeado no backend,
  // o backend precisará ser ajustado para receber.
  // ==========================================================
  async function atualizarRascunhoRasp(idRasp) {
    const payloadRascunho = {
      idUsuarioExecutor: ID_USUARIO_LOGADO,
      idModeloVeiculoRasp: modeloVeiculoSelect?.value ? Number(modeloVeiculoSelect.value) : null,
      idTurnoRasp: turnoRaspSelect?.value ? Number(turnoRaspSelect.value) : null,
      idPilotoRasp: pilotoRaspSelect?.value ? Number(pilotoRaspSelect.value) : null,
      idMajorRasp: majorRaspSelect?.value ? Number(majorRaspSelect.value) : null,

      // Novos campos da Seção 5
      idGmAliadoRasp: gmAliadoSelect?.value ? Number(gmAliadoSelect.value) : null,
      idAprovadorFt: aprovadorFtIdInput?.value ? Number(aprovadorFtIdInput.value) : null,
      iniciativaFornecedor: Boolean(iniciativaFornecedorInput?.checked),

      // Campos já existentes na tela
      nomeContato: nomeContatoInput?.value?.trim() || null,
      dataContato: dataContatoInput?.value || null,
      rdNumero: rdNumeroInput?.value?.trim() || null,
      campanhaNumero: campanhaNumeroInput?.value?.trim() || null
    };

    const response = await fetch(`${API_BASE_URL}/rasp/${idRasp}/rascunho`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(payloadRascunho)
    });

    if (!response.ok) {
      const mensagemErro = await response.text();
      console.error("Erro ao atualizar rascunho do RASP:", mensagemErro);
      throw new Error(
        `Erro ao salvar dados complementares do RASP. Detalhe: ${mensagemErro}`
      );
    }

    return await response.json();
  }

  // ==========================================================
  // 42. VALIDAÇÃO GERAL DO FORMULÁRIO
  // ==========================================================
  async function validarFormularioAntesDoEnvio() {
    const resultadoDuns = await processarDuns();
    const duns = normalizarNumero(dunsInput?.value);

    const erros = [];

    // --------------------------------------------------------
    // DUNS
    // --------------------------------------------------------
    if (!duns) {
      erros.push("Preencha o DUNS.");
    } else if (!validarDunsFormato(duns)) {
      erros.push("O DUNS deve conter exatamente 9 dígitos numéricos.");
    } else if (dunsEhSequenciaRepetida(duns)) {
      erros.push("DUNS inválido. Sequências repetidas como 000000000, 111111111 e semelhantes não são permitidas.");
    } else if (!resultadoDuns.ok && resultadoDuns.motivo === "nao_encontrado") {
      erros.push("DUNS não encontrado. O fornecedor deve ser cadastrado para prosseguir.");
    } else if (!resultadoDuns.ok) {
      erros.push("O valor informado não parece ser um DUNS válido.");
    }

    // --------------------------------------------------------
    // CAMPOS PRINCIPAIS
    // --------------------------------------------------------
    const resumo = resumoInput?.value.trim() ?? "";
    const descricaoInicial = descricaoInicialInput?.value.trim() ?? "";
    const fornecedor = nomeFornecedorInput?.value.trim() ?? "";
    const tipoFornecedor = tipoFornecedorInput?.value.trim() ?? "";
    const analista = analistaInput?.value.trim() ?? "";
    const statusInicial = statusInicialInput?.value.trim() ?? "";

    const setor = setorSelect?.value ?? "";
    const origem = origemSelect?.value ?? "";
    const maiorImpacto = maiorImpactoSelect?.value ?? "";
    const impactoQualidade = impactoQualidadeSelect?.value ?? "";
    const impactoCliente = impactoClienteSelect?.value ?? "";

    let nomeContato = nomeContatoInput?.value.trim() ?? "";
    const dataContato = dataContatoInput?.value ?? "";

    const pns = coletarPns();

    if (!nomeContato) {
      nomeContato = "Não Contatado";
    }

    const nomeContatoNormalizado = nomeContato.toLowerCase();
    const contatoInformado =
      nomeContatoNormalizado !== "não contatado" &&
      nomeContatoNormalizado !== "nao contatado" &&
      nomeContatoNormalizado !== "";

    if (contatoInformado && !dataContato) {
      erros.push("Preencha a Data do contato quando houver uma pessoa contatada.");
    }

    if (!resumo) erros.push("Preencha o Resumo da ocorrência.");
    if (!descricaoInicial) erros.push("Preencha a Descrição inicial.");
    if (!fornecedor) erros.push("O fornecedor deve ser carregado automaticamente pelo DUNS.");
    if (!tipoFornecedor) erros.push("O tipo do fornecedor deve ser carregado automaticamente pelo DUNS.");
    if (!fornecedorAtual) erros.push("O fornecedor precisa ser localizado corretamente pelo DUNS antes do envio.");
    if (!analista) erros.push("O campo Analista deve vir preenchido pelo login.");
    if (!statusInicial) erros.push("O campo Status inicial deve vir preenchido pelo sistema.");
    if (!setor) erros.push("Selecione o Setor.");
    if (!origem) erros.push("Selecione a Origem.");
    if (!maiorImpacto) erros.push("Selecione o Maior impacto.");
    if (!impactoQualidade) erros.push("Selecione o Impacto qualidade.");
    if (!impactoCliente) erros.push("Selecione o Impacto cliente.");

    // --------------------------------------------------------
    // APROVADOR FT (OBRIGATÓRIO NA CRIAÇÃO)
    // --------------------------------------------------------
    if (!aprovadorFtIdInput?.value) {
      erros.push("Selecione um Turno válido para preencher automaticamente o Aprovador (FT).");
    }

    // --------------------------------------------------------
    // PNs
    // --------------------------------------------------------
    if (pns.length === 0) {
      erros.push("Informe pelo menos 1 PN.");
    } else {
      const pnsUnicos = new Set();
      let qtdPrincipais = 0;

      pns.forEach((item, index) => {
        if (item.principal) qtdPrincipais += 1;

        if (!validarPn(item.pn)) {
          erros.push(`O PN da linha ${index + 1} deve conter 8 dígitos numéricos.`);
        }

        if (item.pn && !item.idPn) {
          erros.push(`O PN da linha ${index + 1} não está cadastrado no sistema.`);
        }

        if (pnsUnicos.has(item.pn)) {
          erros.push(`O PN ${item.pn} está repetido.`);
        } else {
          pnsUnicos.add(item.pn);
        }

        if (!item.dataLoteInicial && item.principal) {
          erros.push(`Preencha a Data inicial do PN principal na linha ${index + 1}.`);
        }

        if (item.dataLoteInicial && !dataInicialEhValida(item.dataLoteInicial)) {
          erros.push(`A Data inicial da linha ${index + 1} é inválida. Use o formato DD/MM/AA.`);
        }

        if (
          item.qtdSuspeitaInicial < 0 ||
          item.qtdChecadaInicial < 0 ||
          item.qtdRejeitadaInicial < 0
        ) {
          erros.push(`As quantidades da linha ${index + 1} não podem ser negativas.`);
        }
      });

      if (qtdPrincipais === 0) erros.push("Marque 1 PN principal.");
      if (qtdPrincipais > 1) erros.push("Apenas 1 PN pode ser principal.");
    }

    return {
      ok: erros.length === 0,
      erros,
      duns,
      pns,
      descricaoInicial
    };
  }

  // ==========================================================
  // 43. CRIAÇÃO DO RASP - PAYLOAD INICIAL
  // OBS:
  // O POST /rasp continua mínimo, conforme sua API atual.
  // Os complementos seguem no PUT /rasp/{id}/rascunho.
  // ==========================================================
  function montarPayloadCriacaoRasp(descricaoInicial) {
    return {
      idFornecedorRasp:
        fornecedorAtual?.idFornecedor ??
        fornecedorAtual?.idFornecedorRasp ??
        fornecedorAtual?.id_fornecedor ??
        null,
      descricaoProblema: descricaoInicial,
      idUsuarioCriador: ID_USUARIO_LOGADO
    };
  }

  // ==========================================================
  // 44. ENVIO DO FORMULÁRIO
  // ==========================================================
  async function enviarFormularioRasp() {
    const validacao = await validarFormularioAntesDoEnvio();

    if (!validacao.ok) {
      alert(validacao.erros.join("\n"));
      aplicarValidacoesPnEmTela();
      return;
    }

    const payloadCriacao = montarPayloadCriacaoRasp(validacao.descricaoInicial);

    try {
      const responseRasp = await fetch(`${API_BASE_URL}/rasp`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(payloadCriacao)
      });

      const resultado = await responseRasp.json();

      if (!responseRasp.ok) {
        throw new Error(resultado.erro || resultado.mensagem || "Erro ao criar RASP");
      }

      console.log("RETORNO COMPLETO DO RASP:", resultado);

      const idRaspCriado =
        resultado.idRasp ??
        resultado.id_rasp ??
        resultado.id;

      const numeroRaspCriado =
        resultado.numeroRasp ??
        resultado.numero_rasp ??
        resultado.numero;

      const dataCriacaoRasp =
        resultado.dataCriacao ??
        resultado.data_criacao ??
        resultado.datacriacao;

      if (!idRaspCriado) {
        throw new Error("A API criou o RASP, mas não retornou o ID do registro.");
      }

      await salvarPnsDoRasp(idRaspCriado, validacao.pns, validacao.duns);
      await atualizarRascunhoRasp(idRaspCriado);

      atualizarNumeroRaspDisplay(numeroRaspCriado || `ID ${idRaspCriado}`);
      atualizarDataCriacaoRaspDisplay(dataCriacaoRasp);

      mostrarMensagemRasp(
        `RASP ${numeroRaspCriado || `ID ${idRaspCriado}`} criado com sucesso.`
      );

      window.scrollTo({
        top: 0,
        behavior: "smooth"
      });
    } catch (error) {
      console.error(error);
      alert(error.message || "Erro ao enviar RASP para API.");
    }
  }

  // ==========================================================
  // 45. EVENTOS - CAMPOS DE CONTATO
  // ==========================================================
  if (nomeContatoInput) {
    nomeContatoInput.addEventListener("focus", () => {
      const valor = nomeContatoInput.value.trim().toLowerCase();

      if (valor === "não contatado" || valor === "nao contatado") {
        nomeContatoInput.value = "";
        nomeContatoInput.classList.remove("input-placeholder");
      }
    });

    nomeContatoInput.addEventListener("blur", () => {
      if (!nomeContatoInput.value.trim()) {
        nomeContatoInput.value = "Não Contatado";
        nomeContatoInput.classList.add("input-placeholder");
      }
      validarRegraContato();
    });

    nomeContatoInput.addEventListener("input", () => {
      nomeContatoInput.classList.remove("input-placeholder");
      validarRegraContato();
    });
  }

  if (dataContatoInput) {
    dataContatoInput.addEventListener("change", validarRegraContato);
  }

  // ==========================================================
  // 46. EVENTOS - MODAIS
  // ==========================================================
  if (cancelarPnBtn) {
    cancelarPnBtn.addEventListener("click", fecharModalPn);
  }

  if (cancelarFornecedorBtn) {
    cancelarFornecedorBtn.addEventListener("click", fecharModalFornecedor);
  }

  if (salvarPnBtn) {
    salvarPnBtn.addEventListener("click", salvarPnViaModal);
  }

  if (salvarFornecedorBtn) {
    salvarFornecedorBtn.addEventListener("click", salvarFornecedorViaModal);
  }

  // ==========================================================
  // 47. EVENTOS - DUNS
  // ==========================================================
  if (dunsInput) {
    dunsInput.addEventListener("input", async () => {
      const valorNormalizado = normalizarNumero(dunsInput.value).slice(0, 9);
      dunsInput.value = valorNormalizado;

      if (valorNormalizado.length === 9) {
        await processarDuns();
        return;
      }

      limparFornecedorDerivado();

      if (valorNormalizado.length === 0) {
        definirStatusDuns(
          "Informe um DUNS válido para localizar o fornecedor.",
          "neutral"
        );
      } else {
        definirStatusDuns(
          "Digite os 9 dígitos do DUNS para localizar o fornecedor.",
          "neutral"
        );
      }
    });

    dunsInput.addEventListener("blur", processarDuns);
  }

  // ==========================================================
  // 48. EVENTOS - TABELA DE PNs
  // ==========================================================
  if (addPnRowBtn) {
    addPnRowBtn.addEventListener("click", () => {
      if (!pnTableBody) return;

      pnTableBody.appendChild(criarLinhaPn());
      garantirPnPrincipal();
      aplicarValidacoesPnEmTela();
    });
  }

  if (pnTableBody) {
    pnTableBody.addEventListener("click", (event) => {
      if (!event.target.classList.contains("remove-row")) return;

      const rows = obterRowsPn();

      if (rows.length === 1) {
        alert("O RASP precisa ter pelo menos 1 PN.");
        return;
      }

      const linha = event.target.closest("tr");
      const eraPrincipal = linha.querySelector(".pn-principal").checked;

      linha.remove();

      if (eraPrincipal) {
        garantirPnPrincipal();
      }

      aplicarValidacoesPnEmTela();
    });

    pnTableBody.addEventListener("input", async (event) => {
      const linha = event.target.closest(".pn-row");
      if (!linha) return;

      normalizarInputsPnDaLinha(linha);

      if (event.target.classList.contains("pn-input")) {
        const pnNormalizado = normalizarNumero(event.target.value).slice(0, 8);
        event.target.value = pnNormalizado;

        if (pnNormalizado.length === 8) {
          await processarPnDaLinha(linha);
        } else {
          limparPnDerivadoDaLinha(linha);
        }
      }

      aplicarValidacoesPnEmTela();
    });

    pnTableBody.addEventListener("change", (event) => {
      if (
        event.target.classList.contains("pn-principal") ||
        event.target.classList.contains("data-lote-inicial") ||
        event.target.classList.contains("qtd-suspeita") ||
        event.target.classList.contains("qtd-checada") ||
        event.target.classList.contains("qtd-rejeitada")
      ) {
        aplicarValidacoesPnEmTela();
      }
    });
  }

  // ==========================================================
  // 49. EVENTOS - IMPORTAÇÃO EM MASSA DE PNs
  // ==========================================================
  if (toggleMassPanelBtn) {
    toggleMassPanelBtn.addEventListener("click", () => {
      if (!massPanel) return;
      massPanel.classList.toggle("show");
    });
  }

  if (importMassPnsBtn) {
    importMassPnsBtn.addEventListener("click", () => {
      if (!pnsLoteTextarea || !pnTableBody) return;

      const texto = pnsLoteTextarea.value.trim();

      if (!texto) {
        alert("Cole pelo menos 1 PN para importar.");
        return;
      }

      const linhas = texto
        .split("\n")
        .map((linha) => normalizarNumero(linha))
        .filter((linha) => linha !== "");

      if (linhas.length === 0) {
        alert("Nenhum PN válido foi identificado no texto colado.");
        return;
      }

      const existentes = new Set(
        obterRowsPn()
          .map((row) => normalizarNumero(row.querySelector(".pn-input").value))
          .filter(Boolean)
      );

      let adicionados = 0;
      const invalidos = [];
      const duplicados = [];

      linhas.forEach((pn) => {
        if (!validarPn(pn)) {
          invalidos.push(pn);
          return;
        }

        if (existentes.has(pn)) {
          duplicados.push(pn);
          return;
        }

        existentes.add(pn);
        pnTableBody.appendChild(criarLinhaPn({ pn }));
        adicionados += 1;
      });

      garantirPnPrincipal();
      aplicarValidacoesPnEmTela();

      let mensagem = `Importação concluída.\nAdicionados: ${adicionados}`;

      if (invalidos.length > 0) {
        mensagem += `\nInválidos: ${invalidos.join(", ")}`;
      }

      if (duplicados.length > 0) {
        mensagem += `\nDuplicados ignorados: ${duplicados.join(", ")}`;
      }

      alert(mensagem);
      pnsLoteTextarea.value = "";
    });
  }

  // ==========================================================
  // 50. EVENTOS - BP
  // ==========================================================
  if (tipoReferenciaBp) {
    tipoReferenciaBp.addEventListener("change", () => {
      controlarTipoBp();
      validarBpEmTela();
    });
  }

  if (dataBp) {
    dataBp.addEventListener("change", validarBpEmTela);
  }

  if (horaBp) {
    horaBp.addEventListener("change", validarBpEmTela);
  }

  if (vinBp) {
    vinBp.addEventListener("input", () => {
      vinBp.value = normalizarVin(vinBp.value);
      validarBpEmTela();
    });
  }

  if (localCelulaBp) {
    localCelulaBp.addEventListener("input", validarBpEmTela);
  }

  if (comoIdentificadoBp) {
    comoIdentificadoBp.addEventListener("input", validarBpEmTela);
  }

  // ==========================================================
  // 51. EVENTOS - APROVADOR FT PELO TURNO
  // ==========================================================
  if (turnoRaspSelect) {
    turnoRaspSelect.addEventListener("change", atualizarAprovadorFtPorTurno);
  }

  // ==========================================================
  // 52. EVENTOS - BOTÕES GERAIS
  // ==========================================================
  if (btnLimpar) {
    btnLimpar.addEventListener("click", async () => {
      await limparFormularioParaNovoRasp();
    });
  }

  if (form) {
    form.addEventListener("submit", async (event) => {
      event.preventDefault();
      await enviarFormularioRasp();
    });
  }

  if (raspIndicadorBox) {
    raspIndicadorBox.addEventListener("click", async () => {
      const numeroAtual = obterNumeroRaspAtual();

      if (!numeroAtual || numeroAtual === "---") {
        return;
      }

      const copiou = await copiarTexto(numeroAtual);

      if (copiou) {
        destacarCopiaRasp();
        alert(`Número copiado: ${numeroAtual}`);
      } else {
        alert("Não foi possível copiar o número do RASP.");
      }
    });
  }

  // ==========================================================
  // 53. INICIALIZAÇÃO FINAL DA TELA
  // ==========================================================
  async function inicializarTelaRasp() {
    inicializarCamposFixos();
    ocultarMensagemRasp();

    aplicarMascaraDataLote();
    validarRegraContato();

    await carregarDominiosComplementares();
    await atualizarAprovadorFtPorTurno();

    aplicarValidacoesPnEmTela();
    controlarTipoBp();
    validarBpEmTela();

    definirStatusDuns(
      "Informe um DUNS válido para localizar o fornecedor.",
      "neutral"
    );
  }

  inicializarTelaRasp();
});
