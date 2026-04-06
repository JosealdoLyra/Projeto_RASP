document.addEventListener("DOMContentLoaded", () => {
  // ==========================================================
  // REFERÊNCIAS PRINCIPAIS DO DOM
  // ==========================================================
  const form = document.getElementById("raspForm");

  // ==========================================================
// REFERÊNCIAS DE ELEMENTOS DO DOM
// ==========================================================
const mensagemRasp = document.getElementById("mensagemRasp");

// ==========================================================
// SEÇÃO 6 - BP (BREAK POINT)
// ==========================================================
const tipoReferenciaBp = document.getElementById("tipoReferenciaBp");
const dataBp = document.getElementById("dataBp");
const horaBp = document.getElementById("horaBp");
const vinBp = document.getElementById("vinBp");
const localCelulaBp = document.getElementById("localCelulaBp");
const comoIdentificadoBp = document.getElementById("comoIdentificadoBp");
const bpStatus = document.getElementById("bpStatus");

  // ==========================================================
  // CONTROLE DE ABAS DO RASP
  // ==========================================================
  const tabs = document.querySelectorAll(".rasp-tab");
  const tabContents = document.querySelectorAll(".rasp-tab-content");

  // ==========================================================
  // INDICADORES / INFORMAÇÕES DO RASP
  // ==========================================================
  const raspIndicadorBox = document.getElementById("raspIndicadorBox");
  const raspNumeroDisplay = document.getElementById("raspNumeroDisplay");
  const dataCriacaoRaspDisplay = document.getElementById("dataCriacaoRasp");
  const numeroRaspInfo = document.getElementById("numeroRasp");
  const dataCriacaoRaspInfo = document.getElementById("dataCriacaoRasp");


  // ==========================================================
  // SEÇÃO 2 - DADOS BÁSICOS DO RASP
  // ==========================================================
  const dunsInput = document.getElementById("duns");
  const nomeFornecedorInput = document.getElementById("nomeFornecedor");
  const tipoFornecedorInput = document.getElementById("tipoFornecedor");
  const analistaInput = document.getElementById("analista");
  const statusInicialInput = document.getElementById("statusInicial");
  const dunsStatus = document.getElementById("dunsStatus");

  // ==========================================================
  // SEÇÃO 4 - PN
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
  // MODAL DE CADASTRO DE PN
  // ==========================================================
  const modalPn = document.getElementById("modalPn");
  const modalPnCodigo = document.getElementById("modalPnCodigo");
  const modalPnDescricao = document.getElementById("modalPnDescricao");
  const salvarPnBtn = document.getElementById("salvarPn");
  const cancelarPnBtn = document.getElementById("cancelarPn");

  // ==========================================================
  // MODAL DE CADASTRO DE FORNECEDOR
  // ==========================================================
  const modalFornecedor = document.getElementById("modalFornecedor");
  const modalFornecedorDuns = document.getElementById("modalFornecedorDuns");
  const modalFornecedorNome = document.getElementById("modalFornecedorNome");
  const modalFornecedorTipo = document.getElementById("modalFornecedorTipo");
  const salvarFornecedorBtn = document.getElementById("salvarFornecedor");
  const cancelarFornecedorBtn = document.getElementById("cancelarFornecedor");

  // ==========================================================
  // SEÇÃO 5 - DADOS COMPLEMENTARES
  // ==========================================================
  const modeloVeiculoSelect = document.getElementById("modeloVeiculo");
  const turnoRaspSelect = document.getElementById("turnoRasp");
  const pilotoRaspSelect = document.getElementById("pilotoRasp");
  const majorRaspSelect = document.getElementById("majorRasp");
  const nomeContatoInput = document.getElementById("nomeContato");
  const dataContatoInput = document.getElementById("dataContato");
  const rdNumeroInput = document.getElementById("rdNumero");
  const campanhaNumeroInput = document.getElementById("campanhaNumero");

  // ==========================================================
  // ESTADO DA TELA
  // ==========================================================
  const usuarioLogado = "Usuário Logado";
  let fornecedorAtual = null;
  let linhaPnAtual = null;

  // ==========================================================
  // BASE TEMPORÁRIA DE FORNECEDORES
  // OBS:
  // Mantida apenas como apoio local.
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
  // CONTROLE DAS ABAS
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
  // INICIALIZAÇÃO DOS CAMPOS FIXOS
  // ==========================================================
  function inicializarCamposFixos() {
    if (analistaInput) analistaInput.value = usuarioLogado;
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
  // UTILITÁRIOS
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

  // ==========================================================
// MENSAGEM DE RETORNO DO RASP
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
  // INDICADOR VISUAL DO RASP
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
// EVENTOS DO BP
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
// STATUS VISUAL DO BP
// ==========================================================
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
  // CARGA DE DOMÍNIOS
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

  async function carregarDominiosComplementares() {
    await Promise.all([
      carregarDominioSelect(
        "http://localhost:5050/modelo-veiculo-rasp",
        modeloVeiculoSelect,
        "idModeloVeiculoRasp",
        "nomeModelo",
        "modelo do veículo"
      ),
      carregarDominioSelect(
        "http://localhost:5050/turno-rasp",
        turnoRaspSelect,
        "idTurnoRasp",
        "descricao",
        "turno"
      ),
      carregarDominioSelect(
        "http://localhost:5050/piloto-rasp",
        pilotoRaspSelect,
        "idPilotoRasp",
        "descricao",
        "piloto"
      ),
      carregarDominioSelect(
        "http://localhost:5050/major-rasp",
        majorRaspSelect,
        "idMajorRasp",
        "descricao",
        "major"
      )
    ]);

    selecionarMajorPadrao();
  }

  // ==========================================================
  // REGRA DO CAMPO CONTATO
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
    dataContatoInput.addEventListener("change", () => {
      validarRegraContato();
    });
  }

  // ==========================================================
  // MODAL - PN
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
  // MODAL - FORNECEDOR
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
  // EVENTOS DOS MODAIS
  // ==========================================================
  if (cancelarPnBtn) {
    cancelarPnBtn.addEventListener("click", () => {
      fecharModalPn();
    });
  }

  if (cancelarFornecedorBtn) {
    cancelarFornecedorBtn.addEventListener("click", () => {
      fecharModalFornecedor();
    });
  }

  // ==========================================================
  // API - FORNECEDOR
  // ==========================================================
  async function buscarFornecedorNaApi(duns) {
    const url = `http://localhost:5050/fornecedor-rasp/duns/${duns}`;

    try {
      const response = await fetch(url);

      if (response.status === 404) {
        return { ok: false, motivo: "nao_encontrado" };
      }

      if (!response.ok) {
        return { ok: false, motivo: "erro_api" };
      }

      const fornecedor = await response.json();

      return {
        ok: true,
        fornecedor
      };
    } catch (error) {
      console.error("Erro ao consultar fornecedor na API:", error);
      return { ok: false, motivo: "falha_conexao" };
    }
  }

  // ==========================================================
  // API - PN
  // ==========================================================
  async function buscarPnNaApi(pn) {
    const url = `http://localhost:5050/pn-rasp/codigo/${pn}`;

    try {
      const response = await fetch(url);

      if (response.status === 404) {
        return { ok: false, motivo: "nao_encontrado" };
      }

      if (!response.ok) {
        return { ok: false, motivo: "erro_api" };
      }

      const pnData = await response.json();

      return {
        ok: true,
        pn: pnData
      };
    } catch (error) {
      console.error("Erro ao consultar PN na API:", error);
      return { ok: false, motivo: "falha_conexao" };
    }
  }

  // ==========================================================
  // SALVAR PN VIA MODAL
  // ==========================================================
  if (salvarPnBtn) {
    salvarPnBtn.addEventListener("click", async () => {
      if (!modalPnCodigo || !modalPnDescricao || !linhaPnAtual) return;

      const pn = modalPnCodigo.value;
      const descricao = modalPnDescricao.value.trim();

      if (!descricao) {
        alert("Informe a descrição do PN.");
        return;
      }

      try {
        const response = await fetch("http://localhost:5050/pn-rasp", {
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
    });
  }

  // ==========================================================
  // SALVAR FORNECEDOR VIA MODAL
  // ==========================================================
  if (salvarFornecedorBtn) {
    salvarFornecedorBtn.addEventListener("click", async () => {
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
        const response = await fetch("http://localhost:5050/fornecedor-rasp", {
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
          tipoFornecedorInput.value = novoFornecedor.tipoFornecedor ?? "";
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
    });
  }

  // ==========================================================
  // LIMPEZA / STATUS - FORNECEDOR
  // ==========================================================
  function limparFornecedorDerivado() {
    if (nomeFornecedorInput) nomeFornecedorInput.value = "";
    if (tipoFornecedorInput) tipoFornecedorInput.value = "";
    fornecedorAtual = null;
  }

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

  // ==========================================================
  // LIMPEZA / STATUS - PN
  // ==========================================================
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

  // ==========================================================
  // PROCESSAMENTO DO DUNS
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
  // EVENTOS DO CAMPO DUNS
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

    dunsInput.addEventListener("blur", async () => {
      await processarDuns();
    });
  }

  // ==========================================================
  // TABELA DE PNs - UTILITÁRIOS
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

  // ==========================================================
// CRIA UMA NOVA LINHA DE PN
// ==========================================================
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


  // ==========================================================
  // REMOÇÃO LOCAL DE LINHA DE PN
  // ==========================================================
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
  }

// ==========================================================
// COLETA TODOS OS PNs DIGITADOS NA TABELA
// ==========================================================
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



  // ==========================================================
  // LIMPEZA DOS CAMPOS DERIVADOS DO PN NA LINHA
  // ==========================================================
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

  // ==========================================================
  // PROCESSA O PN DE UMA LINHA
  // ==========================================================
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
    pnDescricaoInput.value = pnData.nomePeca ?? pnData.nome_peca ?? pnData.descricao ?? "";

    return {
      ok: true,
      motivo: "encontrado",
      pn: pnData
    };
  }

  // ==========================================================
  // REINICIA A TABELA DE PN
  // ==========================================================
  function limparTabelaPn() {
    if (!pnTableBody) return;

    pnTableBody.innerHTML = "";
    pnTableBody.appendChild(criarLinhaPn({ principal: true }));
    aplicarValidacoesPnEmTela();
  }

  // ==========================================================
  // ESTILO DE ERRO VISUAL NOS CAMPOS
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
  // NORMALIZA INPUTS DA LINHA DE PN
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
  // VALIDA DUPLICIDADE DE PN NA TELA
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

  // ==========================================================
  // VALIDA LOTE OBRIGATÓRIO DO PN PRINCIPAL
  // ==========================================================
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
// ==========================================================
// CONTROLE DE VIN x LOCAL
// ==========================================================
function controlarTipoBp() {
  const tipo = tipoReferenciaBp?.value ?? "";

  vinBp.value = "";
  localCelulaBp.value = "";

  if (tipo === "VIN") {
    vinBp.disabled = false;
    localCelulaBp.disabled = true;
  } else if (tipo === "LOCAL") {
    vinBp.disabled = true;
    localCelulaBp.disabled = false;
  } else {
    vinBp.disabled = true;
    localCelulaBp.disabled = true;
  }
}

// ==========================================================
// VALIDAÇÃO DO BP EM TELA
// ==========================================================
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


// ==========================================================
// VALIDAÇÕES GERAIS DA TABELA DE PN
// ==========================================================
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
  // EVENTO: ADICIONAR NOVA LINHA DE PN
  // ==========================================================
  if (addPnRowBtn) {
    addPnRowBtn.addEventListener("click", () => {
      if (!pnTableBody) return;

      pnTableBody.appendChild(criarLinhaPn());
      garantirPnPrincipal();
      aplicarValidacoesPnEmTela();
    });
  }
tipoReferenciaBp.addEventListener("change", () => {
  controlarTipoBp();
  validarBpEmTela();
});

dataBp.addEventListener("change", validarBpEmTela);
horaBp.addEventListener("change", validarBpEmTela);

vinBp.addEventListener("input", () => {
  vinBp.value = normalizarVin(vinBp.value);
  validarBpEmTela();
});

localCelulaBp.addEventListener("input", validarBpEmTela);
comoIdentificadoBp.addEventListener("input", validarBpEmTela);


  // ==========================================================
  // EVENTOS EM TEMPO REAL DA TABELA DE PN
  // ==========================================================
  if (pnTableBody) {
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
  // EVENTO: ABRIR / FECHAR IMPORTAÇÃO EM MASSA
  // ==========================================================
  if (toggleMassPanelBtn) {
    toggleMassPanelBtn.addEventListener("click", () => {
      if (!massPanel) return;
      massPanel.classList.toggle("show");
    });
  }

  // ==========================================================
  // EVENTO: IMPORTAÇÃO DE PNs EM MASSA
  // ==========================================================
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
  // LIMPEZA COMPLETA DA TELA
  // ==========================================================
  function limparFormularioParaNovoRasp() {
    if (form) form.reset();

    if (analistaInput) analistaInput.value = usuarioLogado;
    if (statusInicialInput) statusInicialInput.value = "Em análise";

    limparFornecedorDerivado();

    definirStatusDuns(
      "Informe um DUNS válido para localizar o fornecedor.",
      "neutral"
    );

    definirStatusPn(
      "Preencha pelo menos 1 PN. O PN principal deve ter Data/Lote Inicial.",
      "neutral"
    );

    limparTabelaPn();

    if (massPanel) massPanel.classList.remove("show");
    if (pnsLoteTextarea) pnsLoteTextarea.value = "";

    atualizarNumeroRaspDisplay("");
    atualizarDataCriacaoRaspDisplay("");
    carregarDominiosComplementares();

    if (nomeContatoInput) {
      nomeContatoInput.value = "Não Contatado";
      nomeContatoInput.classList.add("input-placeholder");
    }

    if (dataContatoInput) {
      dataContatoInput.value = "";
    }

    if (rdNumeroInput) rdNumeroInput.value = "";
    if (campanhaNumeroInput) campanhaNumeroInput.value = "";

    validarRegraContato();
  }

  // ==========================================================
  // EVENTO: BOTÃO NOVO RASP
  // ==========================================================
  if (btnLimpar) {
    btnLimpar.addEventListener("click", () => {
      limparFormularioParaNovoRasp();
    });
  }

  // ==========================================================
  // SALVA OS PNs VINCULADOS AO RASP
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

      const response = await fetch("http://localhost:5050/rasp-pn", {
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
  // ATUALIZA CAMPOS COMPLEMENTARES NO RASCUNHO
  // ==========================================================
  async function atualizarRascunhoRasp(idRasp) {
    const payloadRascunho = {
      idUsuarioExecutor: 1,
      idModeloVeiculoRasp: modeloVeiculoSelect?.value
        ? Number(modeloVeiculoSelect.value)
        : null,
      idTurnoRasp: turnoRaspSelect?.value
        ? Number(turnoRaspSelect.value)
        : null,
      idPilotoRasp: pilotoRaspSelect?.value
        ? Number(pilotoRaspSelect.value)
        : null,
      idMajorRasp: majorRaspSelect?.value
        ? Number(majorRaspSelect.value)
        : null
    };

    const response = await fetch(`http://localhost:5050/rasp/${idRasp}/rascunho`, {
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
// FORMATAÇÃO E VALIDAÇÃO DE DATA INICIAL (DD/MM/AA)
// ==========================================================
function formatarValorDataLote(valor) {
  let somenteNumeros = valor.replace(/\D/g, "");

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

  document.addEventListener("blur", (event) => {
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
  }, true);
}


// ==========================================================
// VALIDAÇÃO VISUAL DE DATA INICIAL
// ==========================================================
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


 // ==========================================================
// EVENTO: SUBMISSÃO
// ==========================================================
if (form) {
  form.addEventListener("submit", async (event) => {
    event.preventDefault();

    const resultadoDuns = await processarDuns();
    const duns = normalizarNumero(dunsInput?.value);
    const errosDuns = [];

    if (!duns) {
      errosDuns.push("Preencha o DUNS.");
    } else if (!validarDunsFormato(duns)) {
      errosDuns.push("O DUNS deve conter exatamente 9 dígitos numéricos.");
    } else if (dunsEhSequenciaRepetida(duns)) {
      errosDuns.push("DUNS inválido. Sequências repetidas como 000000000, 111111111 e semelhantes não são permitidas.");
    } else if (!resultadoDuns.ok && resultadoDuns.motivo === "nao_encontrado") {
      errosDuns.push("DUNS não encontrado. No fluxo final, o sistema deverá oferecer a opção de cadastrar o fornecedor sem perder os dados já preenchidos.");
    } else if (!resultadoDuns.ok) {
      errosDuns.push("O valor informado não parece ser um DUNS válido.");
    }

    if (errosDuns.length > 0) {
      alert(errosDuns[0]);
      if (dunsInput) dunsInput.focus();
      return;
    }

    const resumo = document.getElementById("resumo")?.value.trim() ?? "";
    const descricaoInicial = document.getElementById("descricaoInicial")?.value.trim() ?? "";
    const fornecedor = nomeFornecedorInput?.value.trim() ?? "";
    const tipoFornecedor = tipoFornecedorInput?.value.trim() ?? "";
    const analista = analistaInput?.value.trim() ?? "";
    const statusInicial = statusInicialInput?.value.trim() ?? "";
    const setor = document.getElementById("setor")?.value ?? "";
    const origem = document.getElementById("origem")?.value ?? "";
    const maiorImpacto = document.getElementById("maiorImpacto")?.value ?? "";
    const impactoQualidade = document.getElementById("impactoQualidade")?.value ?? "";
    const impactoCliente = document.getElementById("impactoCliente")?.value ?? "";
    let nomeContato = nomeContatoInput?.value.trim() ?? "";
    const dataContato = dataContatoInput?.value ?? "";
    const pns = coletarPns();
    const erros = [];

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

    if (erros.length > 0) {
      alert(erros.join("\n"));
      aplicarValidacoesPnEmTela();
      return;
    }

    const payload = {
      idFornecedorRasp:
        fornecedorAtual?.idFornecedor ??
        fornecedorAtual?.idFornecedorRasp,
      descricaoProblema: descricaoInicial,
      idUsuarioCriador: 1
    };

    try {
      const responseRasp = await fetch("http://localhost:5050/rasp", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
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

      await salvarPnsDoRasp(idRaspCriado, pns, duns);
      await atualizarRascunhoRasp(idRaspCriado);

      atualizarNumeroRaspDisplay(numeroRaspCriado || `ID ${idRaspCriado}`);
      atualizarDataCriacaoRaspDisplay(dataCriacaoRasp);

      mostrarMensagemRasp (`RASP ${numeroRaspCriado || `ID ${idRaspCriado}`} criado com sucesso.`);

      window.scrollTo({
        top: 0,
        behavior: "smooth"
      });
    } catch (error) {
      console.error(error);
      alert(error.message || "Erro ao enviar RASP para API.");
    }
  });
}

function coletarBp() {
  return {
    tipoReferenciaBp: tipoReferenciaBp.value,
    dataBp: dataBp.value,
    horaBp: horaBp.value,
    vin: vinBp.value.trim() || null,
    localCelula: localCelulaBp.value.trim() || null,
    comoIdentificado: comoIdentificadoBp.value.trim()
  };
}
// ==========================================================
// NORMALIZAÇÃO DO VIN
// ==========================================================
function normalizarVin(valor) {
  return valor
    .toUpperCase()
    .replace(/\s/g, "")
    .replace(/[^A-Z0-9]/g, "")
    .replace(/[IOQ]/g, "")
    .slice(0, 17);
}
// ==========================================================
// VALIDAÇÃO DO VIN
// ==========================================================
function vinEhValido(valor) {
  return /^[A-HJ-NPR-Z0-9]{17}$/.test(valor);
}


// ==========================================================
// INICIALIZAÇÃO FINAL DA TELA
// ==========================================================
inicializarCamposFixos();
aplicarValidacoesPnEmTela();
atualizarNumeroRaspDisplay("");
atualizarDataCriacaoRaspDisplay("");
ocultarMensagemRasp();
aplicarMascaraDataLote();
validarRegraContato();
carregarDominiosComplementares();

if (tipoReferenciaBp && dataBp && horaBp && vinBp && localCelulaBp && comoIdentificadoBp && bpStatus) {
  controlarTipoBp();
  validarBpEmTela();
}
});
