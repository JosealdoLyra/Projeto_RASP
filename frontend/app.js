document.addEventListener("DOMContentLoaded", () => {
  console.log("APP JS CARREGADO");

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
  const btnCriarRasp = form?.querySelector('button[type="submit"]');
  const btnLimpar = document.getElementById("btnLimpar");

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

  if (iniciativaFornecedorInput && impactoClienteSelect) {
  iniciativaFornecedorInput.addEventListener("change", () => {
    aplicarRegraIniciativaFornecedor();
  });
}


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

  let idRaspEmEdicao = null;
  let numeroRaspEmEdicao = null;
  let modoEdicao = false;

  let primeiroPnOriginal = null;
  let contatoOriginalTravado = false;
  let bpOriginalTravado = false;

  // ==========================================================
  // 14. BASE LOCAL TEMPORÁRIA DE FORNECEDORES
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
  function formatarDataLoteParaTela(dataIso) {
    if (!dataIso) return "";
    const data = new Date(dataIso);
    if (Number.isNaN(data.getTime())) return "";
    const dia = String(data.getDate()).padStart(2, "0");
    const mes = String(data.getMonth() + 1).padStart(2, "0");
    const ano = String(data.getFullYear()).slice(-2);
    return `${dia}/${mes}/${ano}`;
  }

  function montarDataHoraUtc(data, hora) {
    if (!data || !hora) return null;
    const dataHoraLocal = new Date(`${data}T${hora}:00`);
    if (Number.isNaN(dataHoraLocal.getTime())) return null;
    return dataHoraLocal.toISOString();
  }

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
    if (Number.isNaN(data.getTime())) return "--/--/----";
    return data.toLocaleDateString("pt-BR");
  }

  function formatarDataParaExibicao(valor) {
    if (!valor) return "--/--/----";
    const data = new Date(valor);
    if (Number.isNaN(data.getTime())) return "--/--/----";
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

  function textoTemConteudoReal(valor) {
    const texto = String(valor || "").trim().toLowerCase();
    return texto !== "" && texto !== "não contatado" && texto !== "nao contatado";
  }

  function bpTemConteudoNoRasp(rasp) {
    return !!(
      rasp?.bpTexto ||
      rasp?.bpSerie ||
      rasp?.bpDatahora ||
      rasp?.bpDataHora ||
      rasp?.bp_datahora ||
      rasp?.breakpointCodigo ||
      rasp?.breakpointTexto
    );
  }

  // ==========================================================
  // 16. CONTROLE VISUAL DO BOTÃO CRIAR RASP
  // ==========================================================
  function marcarBotaoRaspComoCriado() {
    if (btnCriarRasp) {
      btnCriarRasp.disabled = true;
      btnCriarRasp.textContent = "RASP criado";
      btnCriarRasp.classList.remove("primary-btn");
      btnCriarRasp.classList.add("success-btn");
    }

    if (btnLimpar) {
      btnLimpar.disabled = false;
      btnLimpar.classList.add("enabled-btn");
    }
  }

  function restaurarBotaoCriarRasp() {
    if (btnCriarRasp) {
      btnCriarRasp.disabled = false;
      btnCriarRasp.textContent = "Criar RASP";
      btnCriarRasp.classList.remove("success-btn");
      btnCriarRasp.classList.add("primary-btn");
    }

    if (btnLimpar) {
      btnLimpar.disabled = true;
      btnLimpar.classList.remove("enabled-btn");
    }
  }

  // ==========================================================
  // 16.1 BUSCA RÁPIDA DE RASP
  // ==========================================================
  const btnBuscarRasp = document.getElementById("btnBuscarRasp");
  const buscarRaspNumeroInput = document.getElementById("buscarRaspNumero");

  function formatarNumeroRaspBusca(valor) {
    const somenteNumeros = String(valor || "").replace(/\D/g, "").slice(0, 6);

    if (somenteNumeros.length === 0) return "";
    if (somenteNumeros.length <= 4) {
      return `${somenteNumeros}${somenteNumeros.length === 4 ? "/" : ""}`;
    }

    return `${somenteNumeros.slice(0, 4)}/${somenteNumeros.slice(4)}`;
  }

  if (buscarRaspNumeroInput) {
    buscarRaspNumeroInput.addEventListener("input", (e) => {
      e.target.value = formatarNumeroRaspBusca(e.target.value);
    });

    buscarRaspNumeroInput.addEventListener("keydown", (e) => {
      if (e.key === "Enter") {
        e.preventDefault();
        btnBuscarRasp?.click();
      }
    });
  }

  if (btnBuscarRasp && buscarRaspNumeroInput) {
    btnBuscarRasp.addEventListener("click", async () => {
      const numeroDigitado = buscarRaspNumeroInput.value.trim();

      if (!numeroDigitado) {
        alert("Informe o número do RASP.");
        return;
      }

      const numeroNormalizado = numeroDigitado.endsWith("/")
        ? numeroDigitado.slice(0, -1)
        : numeroDigitado;

      try {
        const responseBusca = await fetch(
          `${API_BASE_URL}/rasp/numero/${encodeURIComponent(numeroNormalizado)}`
        );

        if (!responseBusca.ok) {
          alert("RASP não encontrado.");
          return;
        }

        const raspEncontrado = await responseBusca.json();
        console.log("RASP encontrado:", raspEncontrado);

        const idRaspEncontrado =
          raspEncontrado.idRasp ??
          raspEncontrado.id_rasp ??
          raspEncontrado.id;

        if (!idRaspEncontrado) {
          alert("A API localizou o RASP, mas não retornou o ID.");
          return;
        }

        const responseDetalhe = await fetch(
          `${API_BASE_URL}/rasp/${idRaspEncontrado}/detalhe`
        );

        if (!responseDetalhe.ok) {
          alert("Não foi possível carregar o detalhe do RASP.");
          return;
        }

        const detalhe = await responseDetalhe.json();
        console.log("Detalhe completo do RASP:", detalhe);

        await preencherFormularioRasp(detalhe);

        console.log("ENTREI NA preencherFormularioRasp");
        console.log("DETALHE RECEBIDO:", detalhe);
        console.log("RASP RECEBIDO:", detalhe?.rasp);

        alert("RASP carregado com sucesso.");
      } catch (error) {
        console.error("Erro ao buscar RASP:", error);
        alert("Erro ao buscar RASP.");
      }
    });
  }

  function bloquearCampo(input, bloquear = true) {
  if (!input) return;

  input.readOnly = bloquear;
  input.disabled = false;

  if (bloquear) {
    input.classList.add("system-field");
  } else {
    input.classList.remove("system-field");
  }
}


 function bloquearSelect(select, bloquear = true) {
  if (!select) return;

  select.disabled = bloquear;

  if (bloquear) {
    select.classList.add("system-field");
  } else {
    select.classList.remove("system-field");
  }
}



  function travarPrimeiroPnOriginalSeNecessario() {
  if (!pnTableBody || !primeiroPnOriginal) return;

  const linhas = [...pnTableBody.querySelectorAll(".pn-row")];

  linhas.forEach((linha) => {
    const pnInput = linha.querySelector(".pn-input");
    const dataLoteInput = linha.querySelector(".data-lote-inicial");
    const removeBtn = linha.querySelector(".remove-row");
    const principalRadio = linha.querySelector(".pn-principal");
    const valorPn = normalizarNumero(pnInput?.value || "");

    if (valorPn === primeiroPnOriginal) {
      if (pnInput) {
  pnInput.readOnly = true;
  pnInput.disabled = false;
  pnInput.classList.add("system-field");
}

if (dataLoteInput) {
  dataLoteInput.readOnly = true;
  dataLoteInput.disabled = false;
  dataLoteInput.classList.add("system-field");
}

const descricaoPnInput = linha.querySelector(".pn-descricao");
if (descricaoPnInput) {
  descricaoPnInput.readOnly = true;
  descricaoPnInput.disabled = false;
  descricaoPnInput.classList.add("system-field");
}


      if (removeBtn) {
        removeBtn.disabled = true;
        removeBtn.title = "O PN original da criação não pode ser removido.";
      }

      if (principalRadio) {
        principalRadio.disabled = true;
      }
    }
  });
}


  function aplicarTravaContato() {
    bloquearCampo(nomeContatoInput, contatoOriginalTravado);
    bloquearCampo(dataContatoInput, contatoOriginalTravado);
  }

  function aplicarTravaBp() {
    bloquearSelect(tipoReferenciaBp, bpOriginalTravado);
    bloquearCampo(dataBp, bpOriginalTravado);
    bloquearCampo(horaBp, bpOriginalTravado);
    bloquearCampo(vinBp, bpOriginalTravado);
    bloquearCampo(localCelulaBp, bpOriginalTravado);
    bloquearCampo(comoIdentificadoBp, bpOriginalTravado);

    if (!bpOriginalTravado) {
      controlarTipoBp();
    }
  }

  function limparEstadoEdicao() {
    modoEdicao = false;
    idRaspEmEdicao = null;
    numeroRaspEmEdicao = null;
    primeiroPnOriginal = null;
    contatoOriginalTravado = false;
    bpOriginalTravado = false;
  }

  function aplicarRegraIniciativaFornecedor() {
  if (!iniciativaFornecedorInput || !impactoClienteSelect) return;

  if (iniciativaFornecedorInput.checked) {
    const optionFornecedor = Array.from(impactoClienteSelect.options).find((opt) => {
      const texto = (opt.textContent || "").trim().toLowerCase();
      return (
        texto === "iniciado - fornecedor" ||
        texto === "iniciado-fornecedor" ||
        texto.includes("iniciado") && texto.includes("fornecedor")
      );
    });

    if (optionFornecedor) {
      impactoClienteSelect.value = optionFornecedor.value;
    }

    impactoClienteSelect.disabled = true;
  } else {
    impactoClienteSelect.disabled = false;

    if (impactoClienteSelect.value) {
      impactoClienteSelect.value = "";
    }
  }
}



  async function preencherFormularioRasp(detalhe) {
    const rasp = detalhe?.rasp;
    const pns = detalhe?.pns || [];

    if (!rasp) {
      console.error("Detalhe do RASP veio sem objeto principal.");
      return;
    }

    idRaspEmEdicao = rasp.idRasp ?? null;
    numeroRaspEmEdicao = rasp.numeroRasp ?? null;
    modoEdicao = true;

    primeiroPnOriginal = pns.length > 0 ? normalizarNumero(pns[0].pn || "") : null;
    contatoOriginalTravado =
      textoTemConteudoReal(rasp.nomeContato) || !!rasp.dataContato;
    bpOriginalTravado = bpTemConteudoNoRasp(rasp);

    console.log("RASP para preenchimento:", rasp);
    console.log("PNs do RASP:", pns);

    // ==========================================================
    // 1) GARANTE QUE OS DOMÍNIOS ESTEJAM CARREGADOS
    // ==========================================================
    await carregarDominiosComplementares();

    // ==========================================================
    // 2) INFORMAÇÕES GERAIS
    // ==========================================================
    if (raspNumeroDisplay) raspNumeroDisplay.textContent = rasp.numeroRasp || "---";
    if (numeroRaspInfo) numeroRaspInfo.textContent = rasp.numeroRasp || "---";

    if (dataCriacaoRaspDisplay) {
      dataCriacaoRaspDisplay.textContent = formatarDataParaExibicao(rasp.dataCriacao);
    }

    if (dataCriacaoRaspInfo) {
      dataCriacaoRaspInfo.textContent = formatarDataParaExibicao(rasp.dataCriacao);
    }

    // ==========================================================
    // 3) OCORRÊNCIA
    // ==========================================================
    if (descricaoInicialInput) descricaoInicialInput.value = rasp.descricaoProblema || "";
    if (resumoInput) resumoInput.value = rasp.descricaoProblema || "";

    // ==========================================================
    // 4) FORNECEDOR
    // ==========================================================
    if (rasp.idFornecedorRasp) {
      try {
        const responseFornecedor = await fetch(
          `${API_BASE_URL}/fornecedor-rasp/${rasp.idFornecedorRasp}`
        );

        if (responseFornecedor.ok) {
          const fornecedor = await responseFornecedor.json();

          if (dunsInput) dunsInput.value = fornecedor.duns || "";
          if (nomeFornecedorInput) nomeFornecedorInput.value = fornecedor.nome || "";

          if (tipoFornecedorInput) {
            tipoFornecedorInput.value =
              fornecedor.tipoFornecedor ||
              fornecedor.tipo ||
              "";
          }

          fornecedorAtual = fornecedor;

          definirStatusDuns(
            `Fornecedor localizado com sucesso: ${fornecedor.nome || ""} (${fornecedor.tipoFornecedor || fornecedor.tipo || ""}).`,
            "success"
          );
        } else {
          console.log("Fornecedor não carregou. Status:", responseFornecedor.status);
        }
      } catch (error) {
        console.error("Erro ao buscar fornecedor:", error);
      }
    }

    // ==========================================================
    // 5) FLAGS
    // ==========================================================
    if (iniciativaFornecedorInput) {
      iniciativaFornecedorInput.checked = !!rasp.iniciativaFornecedor;
    }

    // ==========================================================
    // 6) SEÇÕES 2 / 3 / 5 - CAMPOS QUE DEPENDEM DE DOMÍNIOS
    // ==========================================================
    if (gmAliadoSelect) {
      gmAliadoSelect.value = rasp.idGmAliadoRasp ? String(rasp.idGmAliadoRasp) : "";
    }

    if (setorSelect) {
      setorSelect.value = rasp.idSetorRasp ? String(rasp.idSetorRasp) : "";
    }

    if (origemSelect) {
      origemSelect.value = rasp.idIndiceOperacionalRasp ? String(rasp.idIndiceOperacionalRasp) : "";
    }

    if (maiorImpactoSelect) {
      maiorImpactoSelect.value = rasp.idMaiorImpactoRasp ? String(rasp.idMaiorImpactoRasp) : "";
    }

    if (impactoQualidadeSelect) {
      impactoQualidadeSelect.value = rasp.idImpactoQualidadeRasp ? String(rasp.idImpactoQualidadeRasp) : "";
    }

    if (impactoClienteSelect) {
      impactoClienteSelect.value = rasp.idImpactoClienteRasp ? String(rasp.idImpactoClienteRasp) : "";
    }

    if (modeloVeiculoSelect) {
      modeloVeiculoSelect.value = rasp.idModeloVeiculoRasp ? String(rasp.idModeloVeiculoRasp) : "";
    }

    if (turnoRaspSelect) {
      turnoRaspSelect.value = rasp.idTurnoRasp ? String(rasp.idTurnoRasp) : "";
    }

    if (pilotoRaspSelect) {
      pilotoRaspSelect.value = rasp.idPilotoRasp ? String(rasp.idPilotoRasp) : "";
    }

    if (majorRaspSelect) {
  majorRaspSelect.value = rasp.idMajorRasp ? String(rasp.idMajorRasp) : "";
}

aplicarRegraIniciativaFornecedor();


    // ==========================================================
    // 7) COMPLEMENTARES
    // ==========================================================
    if (nomeContatoInput) nomeContatoInput.value = rasp.nomeContato || "";
    if (rdNumeroInput) rdNumeroInput.value = rasp.rdNumero || "";
    if (campanhaNumeroInput) campanhaNumeroInput.value = rasp.campanhaNumero || "";

    if (dataContatoInput) {
      dataContatoInput.value = rasp.dataContato || "";
    }

    if (tipoFornecedorInput) {
      if (rasp.idOrigemFabricacaoRasp === 1) {
        tipoFornecedorInput.value = "LOCAL";
      } else if (rasp.idOrigemFabricacaoRasp === 2) {
        tipoFornecedorInput.value = "IMPORTADO";
      }
    }

    // ==========================================================
    // 8) APROVADOR FT
    // ==========================================================
    await atualizarAprovadorFtPorTurno();

    if (aprovadorFtIdInput) {
      aprovadorFtIdInput.value = rasp.idAprovadorFt || "";
    }

    if (aprovadorFtInput) {
      if (!aprovadorFtInput.value && rasp.idAprovadorFt) {
        aprovadorFtInput.value = `ID ${rasp.idAprovadorFt}`;
      }
    }

    bloquearCampo(aprovadorFtIdInput, true);

    // ==========================================================
    // 9) BP
    // ==========================================================
    if (tipoReferenciaBp) {
      if (rasp.bpSerie) {
        tipoReferenciaBp.value = "VIN";
      } else if (rasp.breakpointCodigo) {
        tipoReferenciaBp.value = "LOCAL";
      } else {
        tipoReferenciaBp.value = "";
      }
    }

    const bpDataHoraValor =
      rasp.bpDatahora ||
      rasp.bpDataHora ||
      rasp.bp_datahora ||
      null;

    if (dataBp && bpDataHoraValor) {
      const dt = new Date(bpDataHoraValor);

      if (!Number.isNaN(dt.getTime())) {
        dataBp.value = dt.toISOString().slice(0, 10);

        if (horaBp) {
          horaBp.value = dt.toTimeString().slice(0, 5);
        }
      }
    } else {
      if (dataBp) dataBp.value = "";
      if (horaBp) horaBp.value = "";
    }

    if (vinBp) vinBp.value = rasp.bpSerie || "";
    if (localCelulaBp) localCelulaBp.value = rasp.breakpointCodigo || "";
    if (comoIdentificadoBp) comoIdentificadoBp.value = rasp.bpTexto || "";

    // ==========================================================
    // 10) PNs
    // ==========================================================
    if (pnTableBody) {
      pnTableBody.innerHTML = "";

      if (pns.length > 0) {
        if (typeof criarLinhaPn === "function") {
          for (let index = 0; index < pns.length; index++) {
            const pn = pns[index];

            const linha = criarLinhaPn({
            idRaspPn: pn.idRaspPn ?? pn.id_rasp_pn ?? "",
            principal: index === 0,
            pn: pn.pn || "",
            dataLoteInicial:
              pn.dataLoteInicial ||
              pn.data_lote_inicial ||
              "",
            qtdSuspeita:
              pn.quantidadeSuspeita ??
              pn.qtdSuspeita ??
              pn.qtdSuspeitaInicial ??
              0,
            qtdChecada:
              pn.quantidadeChecada ??
              pn.qtdChecada ??
              pn.qtdChecadaInicial ??
              0,
            qtdRejeitada:
              pn.quantidadeRejeitada ??
              pn.qtdRejeitada ??
              pn.qtdRejeitadaInicial ??
              0,
            emSelecao: Number(pn.statusSelecao ?? 0) === 1,
            statusSelecao: pn.statusSelecao ?? 0,
            entrouSelecao: pn.entrouSelecao ?? false,
            travaAtiva: pn.travaAtiva ?? false,
            qhdAtivo: pn.qhdAtivo ?? false,
            dataHoraEntradaSelecao: pn.dataHoraEntradaSelecao ?? null,
            dataHoraSaidaSelecao: pn.dataHoraSaidaSelecao ?? null,
            dataHoraSolicitacaoTrava: pn.dataHoraSolicitacaoTrava ?? null,
            dataHoraRemocaoTrava: pn.dataHoraRemocaoTrava ?? null,
            dataHoraQhd: pn.dataHoraQhd ?? null
          });


            pnTableBody.appendChild(linha);

            const principalRadio = linha.querySelector(".pn-principal");
            const pnInput = linha.querySelector(".pn-input");
            const descricaoPnInput = linha.querySelector(".pn-descricao");
            const dataLoteInput = linha.querySelector(".data-lote-inicial");
            const qtdSuspeitaInput = linha.querySelector(".qtd-suspeita");
            const qtdChecadaInput = linha.querySelector(".qtd-checada");
            const qtdRejeitadaInput = linha.querySelector(".qtd-rejeitada");
            const pnIdInput = linha.querySelector(".pn-id");

            if (principalRadio) {
              principalRadio.checked = !!pn.principal || index === 0;
            }

            if (pnInput) {
              pnInput.value = pn.pn || "";
            }

            if (pnIdInput) {
              pnIdInput.value = pn.idPn || pn.id_pn || "";
            }

            if (descricaoPnInput) {
              descricaoPnInput.value =
                pn.nomePeca ||
                pn.nome_peca ||
                pn.descricaoPn ||
                pn.descricao_pn ||
                pn.descricao ||
                "";
            }

            if (
              typeof processarPnDaLinha === "function" &&
              pnInput &&
              pnInput.value &&
              descricaoPnInput &&
              !descricaoPnInput.value
            ) {
              await processarPnDaLinha(linha);
            }

            if (dataLoteInput) {
              const dataLoteBruta =
                pn.dataLoteInicial ??
                pn.data_lote_inicial ??
                pn.dataLote ??
                pn.data_lote ??
                "";

              dataLoteInput.value = formatarDataLoteParaTela(dataLoteBruta);
            }

            if (qtdSuspeitaInput) {
              qtdSuspeitaInput.value =
                pn.quantidadeSuspeita ??
                pn.qtdSuspeita ??
                pn.qtdSuspeitaInicial ??
                0;
            }

            if (qtdChecadaInput) {
              qtdChecadaInput.value =
                pn.quantidadeChecada ??
                pn.qtdChecada ??
                pn.qtdChecadaInicial ??
                0;
            }

            if (qtdRejeitadaInput) {
              qtdRejeitadaInput.value =
                pn.quantidadeRejeitada ??
                pn.qtdRejeitada ??
                pn.qtdRejeitadaInicial ??
                0;
            }
          }

          garantirPnPrincipal();
          aplicarValidacoesPnEmTela();
          travarPrimeiroPnOriginalSeNecessario();
        }
      } else {
        console.log("Nenhum PN retornado no detalhe.");
      }
    }

    aplicarTravaContato();
    aplicarTravaBp();
    validarBpEmTela();

    // ==========================================================
    // 11) CONTROLE VISUAL
    // ==========================================================
    mostrarMensagemRasp(`RASP ${rasp.numeroRasp} carregado para edição.`);
    restaurarBotaoCriarRasp();

    if (btnCriarRasp) {
      btnCriarRasp.textContent = "Salvar alterações";
    }

    if (btnLimpar) {
      btnLimpar.disabled = false;
      btnLimpar.classList.add("enabled-btn");
    }
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

      console.log(`Total de opções em ${nomeDominio}:`, selectElement.options.length);
      console.log(
        `Última opção de ${nomeDominio}:`,
        selectElement.options[selectElement.options.length - 1]?.text
      );
      console.log(
        `Todas as opções de ${nomeDominio}:`,
        [...selectElement.options].map((o) => o.text)
      );
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
  // 22. CARGA DE DOMÍNIOS COMPLEMENTARES
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
      carregarDominioSelect(
        `${API_BASE_URL}/gm-aliado-rasp`,
        gmAliadoSelect,
        "idGmAliadoRasp",
        "descricao",
        "GM aliado"
      ),
      carregarDominioSelect(
        `${API_BASE_URL}/setor-rasp`,
        setorSelect,
        "idSetorRasp",
        "descricao",
        "setor"
      ),
      carregarDominioSelect(
        `${API_BASE_URL}/indice-operacional-rasp`,
        origemSelect,
        "idIndiceOperacionalRasp",
        "descricao",
        "origem"
      ),
      carregarDominioSelect(
        `${API_BASE_URL}/maior-impacto-rasp`,
        maiorImpactoSelect,
        "idMaiorImpactoRasp",
        "descricao",
        "maior impacto"
      ),
      carregarDominioSelect(
        `${API_BASE_URL}/impacto-qualidade-rasp`,
        impactoQualidadeSelect,
        "idImpactoQualidadeRasp",
        "descricao",
        "impacto qualidade"
      ),
      carregarDominioSelect(
        `${API_BASE_URL}/impacto-cliente-rasp`,
        impactoClienteSelect,
        "idImpactoCliente",
        "descricao",
        "impacto cliente"
      )
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
  // 28. APROVADOR FT POR TURNO
  // ==========================================================
  async function buscarFtPorTurno(idTurno) {
    try {
      const response = await fetch(`${API_BASE_URL}/usuarios`);

      if (!response.ok) {
        throw new Error("Erro ao carregar usuários");
      }

      const usuarios = await response.json();

      const ft = usuarios.find(
        (u) =>
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
      definirStatusDuns("Informe um DUNS válido para localizar o fornecedor.", "neutral");
      return { ok: false, motivo: "vazio" };
    }

    if (!validarDunsFormato(duns)) {
      definirStatusDuns("O DUNS deve conter exatamente 9 dígitos numéricos.", "error");
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
        definirStatusDuns("Não foi possível conectar à API no momento.", "error");
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
  idRaspPn = "",
  principal = false,
  pn = "",
  dataLoteInicial = "",
  qtdSuspeita = 0,
  qtdChecada = 0,
  qtdRejeitada = 0,
  emSelecao = false,
  statusSelecao = 0,
  entrouSelecao = false,
  travaAtiva = false,
  qhdAtivo = false,
  dataHoraEntradaSelecao = null,
  dataHoraSaidaSelecao = null,
  dataHoraSolicitacaoTrava = null,
  dataHoraRemocaoTrava = null,
  dataHoraQhd = null
} = {}) {
  const tr = document.createElement("tr");
  tr.className = "pn-row";

  tr.dataset.idRaspPn = idRaspPn ? String(idRaspPn) : "";
  tr.dataset.statusSelecao = String(statusSelecao ?? 0);
  tr.dataset.entrouSelecao = String(!!entrouSelecao);
  tr.dataset.travaAtiva = String(!!travaAtiva);
  tr.dataset.qhdAtivo = String(!!qhdAtivo);
  tr.dataset.dataHoraEntradaSelecao = dataHoraEntradaSelecao || "";
  tr.dataset.dataHoraSaidaSelecao = dataHoraSaidaSelecao || "";
  tr.dataset.dataHoraSolicitacaoTrava = dataHoraSolicitacaoTrava || "";
  tr.dataset.dataHoraRemocaoTrava = dataHoraRemocaoTrava || "";
  tr.dataset.dataHoraQhd = dataHoraQhd || "";

  const textoBadge =
    Number(statusSelecao) === 2
      ? "Encerrada"
      : (Number(statusSelecao) === 1 || entrouSelecao || emSelecao)
        ? "Em seleção"
        : "Fora";

  const classeBadge =
    Number(statusSelecao) === 1 || entrouSelecao || emSelecao
      ? "pn-badge-selecao"
      : "pn-badge-neutro";

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
      <span class="pn-badge ${classeBadge}">
        ${textoBadge}
      </span>
    </td>

    <td class="center-cell">
      <button
        type="button"
        class="btn-detalhes-linha"
        title="Ver detalhes operacionais do PN"
        aria-label="Ver detalhes operacionais do PN"
      >
        Detalhes
      </button>
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
    if (bpOriginalTravado) return;

    const tipo = tipoReferenciaBp?.value ?? "";

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
  // 38. FORMATAÇÃO E VALIDAÇÃO DE DATA INICIAL
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

    limparEstadoEdicao();

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

    if (gmAliadoSelect) gmAliadoSelect.value = "";
    if (setorSelect) setorSelect.value = "";
    if (origemSelect) origemSelect.value = "";
    if (maiorImpactoSelect) maiorImpactoSelect.value = "";
    if (impactoQualidadeSelect) impactoQualidadeSelect.value = "";

    if (impactoClienteSelect) {
      impactoClienteSelect.value = "";
      impactoClienteSelect.disabled = false;
    }
    if (iniciativaFornecedorInput) {
      iniciativaFornecedorInput.checked = false;
    }

    if (modeloVeiculoSelect) modeloVeiculoSelect.value = "";
    if (turnoRaspSelect) turnoRaspSelect.value = "";
    if (pilotoRaspSelect) pilotoRaspSelect.value = "";

    if (majorRaspSelect) {
      majorRaspSelect.value = "";
      selecionarMajorPadrao();
    }

    if (tipoReferenciaBp) tipoReferenciaBp.value = "";
    if (dataBp) dataBp.value = "";
    if (horaBp) horaBp.value = "";
    if (vinBp) vinBp.value = "";
    if (localCelulaBp) localCelulaBp.value = "";
    if (comoIdentificadoBp) comoIdentificadoBp.value = "";

    aplicarTravaContato();
    aplicarTravaBp();

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
        dataLoteInicial: item.dataLoteInicial || null,
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
  // ==========================================================
  async function atualizarRascunhoRasp(idRasp) {
    console.log("Tipo fornecedor:", tipoFornecedorInput?.value);

    const payloadRascunho = {
      idUsuarioExecutor: ID_USUARIO_LOGADO,
      idModeloVeiculoRasp: modeloVeiculoSelect?.value
        ? Number(modeloVeiculoSelect.value)
        : null,
      idSetorRasp: setorSelect?.value
        ? Number(setorSelect.value)
        : null,
      idTurnoRasp: turnoRaspSelect?.value
        ? Number(turnoRaspSelect.value)
        : null,
      idIndiceOperacionalRasp: origemSelect?.value
        ? Number(origemSelect.value)
        : null,
      idOrigemFabricacaoRasp:
        tipoFornecedorInput?.value === "IMPORTADO"
          ? 2
          : tipoFornecedorInput?.value === "LOCAL"
            ? 1
            : null,
      idPilotoRasp: pilotoRaspSelect?.value
        ? Number(pilotoRaspSelect.value)
        : null,
      idMaiorImpactoRasp: maiorImpactoSelect?.value
        ? Number(maiorImpactoSelect.value)
        : null,
      idImpactoQualidadeRasp: impactoQualidadeSelect?.value
        ? Number(impactoQualidadeSelect.value)
        : null,
      idImpactoClienteRasp: impactoClienteSelect?.value
        ? Number(impactoClienteSelect.value)
        : null,
      idMajorRasp: majorRaspSelect?.value
        ? Number(majorRaspSelect.value)
        : null,
      idGmAliadoRasp: gmAliadoSelect?.value
        ? Number(gmAliadoSelect.value)
        : null,
      rdNumero: rdNumeroInput?.value?.trim() || null,
      campanhaNumero: campanhaNumeroInput?.value?.trim() || null,
      nomeContato: nomeContatoInput?.value?.trim() || null,
      dataContato: dataContatoInput?.value || null,
      iniciativaFornecedor: iniciativaFornecedorInput?.checked ?? false,
      bpTexto: comoIdentificadoBp?.value?.trim() || null,
      bpSerie: vinBp?.value?.trim() || null,
      bpDatahora: montarDataHoraUtc(dataBp?.value, horaBp?.value),
      breakpointCodigo: localCelulaBp?.value?.trim() || null
    };

    console.log("payloadRascunho =>", payloadRascunho);

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

    console.log("VALIDAÇÃO COMBOS:", {
      setor,
      origem,
      maiorImpacto,
      impactoQualidade,
      impactoCliente
    });

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

    if (!aprovadorFtIdInput?.value) {
      erros.push("Selecione um Turno válido para preencher automaticamente o Aprovador (FT).");
    }

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
      return false;
    }

    try {
      // ======================================================
      // MODO EDIÇÃO
      // ======================================================
      if (modoEdicao && idRaspEmEdicao) {
        const payloadEdicao = {
          idUsuarioExecutor: ID_USUARIO_LOGADO,
          descricaoProblema: validacao.descricaoInicial,

          idModeloVeiculoRasp: modeloVeiculoSelect?.value
            ? Number(modeloVeiculoSelect.value)
            : null,
          idSetorRasp: setorSelect?.value
            ? Number(setorSelect.value)
            : null,
          idTurnoRasp: turnoRaspSelect?.value
            ? Number(turnoRaspSelect.value)
            : null,
          idIndiceOperacionalRasp: origemSelect?.value
            ? Number(origemSelect.value)
            : null,
          idOrigemFabricacaoRasp:
            tipoFornecedorInput?.value === "IMPORTADO"
              ? 2
              : tipoFornecedorInput?.value === "LOCAL"
                ? 1
                : null,
          idPilotoRasp: pilotoRaspSelect?.value
            ? Number(pilotoRaspSelect.value)
            : null,
          idMaiorImpactoRasp: maiorImpactoSelect?.value
            ? Number(maiorImpactoSelect.value)
            : null,
          idImpactoQualidadeRasp: impactoQualidadeSelect?.value
            ? Number(impactoQualidadeSelect.value)
            : null,
          idImpactoClienteRasp: impactoClienteSelect?.value
            ? Number(impactoClienteSelect.value)
            : null,
          idMajorRasp: majorRaspSelect?.value
            ? Number(majorRaspSelect.value)
            : null,
          idGmAliadoRasp: gmAliadoSelect?.value
            ? Number(gmAliadoSelect.value)
            : null,

          rdNumero: rdNumeroInput?.value?.trim() || null,
          campanhaNumero: campanhaNumeroInput?.value?.trim() || null,
          nomeContato: nomeContatoInput?.value?.trim() || null,
          dataContato: dataContatoInput?.value || null,
          iniciativaFornecedor: iniciativaFornecedorInput?.checked ?? false,

          bpTexto: comoIdentificadoBp?.value?.trim() || null,
          bpSerie: vinBp?.value?.trim() || null,
          bpDatahora: montarDataHoraUtc(dataBp?.value, horaBp?.value),
          breakpointCodigo: localCelulaBp?.value?.trim() || null
        };

        console.log("payloadEdicao =>", payloadEdicao);

        const responseEdicao = await fetch(`${API_BASE_URL}/rasp/${idRaspEmEdicao}/rascunho`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify(payloadEdicao)
        });

        if (!responseEdicao.ok) {
          const mensagemErro = await responseEdicao.text();
          console.error("Erro ao atualizar RASP em edição:", mensagemErro);
          throw new Error(`Erro ao atualizar RASP. Detalhe: ${mensagemErro}`);
        }

        await responseEdicao.json();

        atualizarNumeroRaspDisplay(numeroRaspEmEdicao || `ID ${idRaspEmEdicao}`);
        mostrarMensagemRasp(
          `RASP ${numeroRaspEmEdicao || `ID ${idRaspEmEdicao}`} atualizado com sucesso.`
        );

        if (btnCriarRasp) {
          btnCriarRasp.textContent = "Salvar alterações";
          btnCriarRasp.disabled = false;
          btnCriarRasp.classList.remove("success-btn");
          btnCriarRasp.classList.add("primary-btn");
        }

        window.scrollTo({
          top: 0,
          behavior: "smooth"
        });

        return true;
      }

      // ======================================================
      // MODO CRIAÇÃO
      // ======================================================
      const payloadCriacao = montarPayloadCriacaoRasp(validacao.descricaoInicial);

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

      idRaspEmEdicao = idRaspCriado;
      numeroRaspEmEdicao = numeroRaspCriado || null;
      modoEdicao = true;

      if (validacao.pns.length > 0) {
        primeiroPnOriginal = normalizarNumero(validacao.pns[0].pn || "");
      }

      contatoOriginalTravado =
        textoTemConteudoReal(nomeContatoInput?.value) || !!dataContatoInput?.value;

      bpOriginalTravado =
        !!(comoIdentificadoBp?.value?.trim() ||
        vinBp?.value?.trim() ||
        localCelulaBp?.value?.trim() ||
        dataBp?.value ||
        horaBp?.value);

      aplicarTravaContato();
      aplicarTravaBp();
      travarPrimeiroPnOriginalSeNecessario();

      atualizarNumeroRaspDisplay(numeroRaspCriado || `ID ${idRaspCriado}`);
      atualizarDataCriacaoRaspDisplay(dataCriacaoRasp);

      mostrarMensagemRasp(
        `RASP ${numeroRaspCriado || `ID ${idRaspCriado}`} criado com sucesso.`
      );

      marcarBotaoRaspComoCriado();

      window.scrollTo({
        top: 0,
        behavior: "smooth"
      });

      return true;
    } catch (error) {
      console.error(error);
      alert(error.message || "Erro ao enviar RASP para API.");
      return false;
    }
  }

 // ==========================================================
  // 38.1 API - CONTROLE DE SELEÇÃO DO PN
  // ==========================================================
  async function entrarSelecaoPn(idRaspPn, travaAtiva = false) {
    const response = await fetch(`${API_BASE_URL}/rasp-pn/${idRaspPn}/entrar-selecao`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        travaAtiva: !!travaAtiva
      })
    });

    const respostaTexto = await response.text();
    let respostaJson = null;

    try {
      respostaJson = respostaTexto ? JSON.parse(respostaTexto) : null;
    } catch {
      respostaJson = null;
    }

    if (!response.ok) {
      throw new Error(
        respostaJson?.mensagem ||
        respostaTexto ||
        "Erro ao colocar PN em seleção."
      );
    }

    return respostaJson;
  }

  async function sairSelecaoPn(idRaspPn, removerTrava = false) {
    const response = await fetch(`${API_BASE_URL}/rasp-pn/${idRaspPn}/sair-selecao`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        removerTrava: !!removerTrava
      })
    });

    const respostaTexto = await response.text();
    let respostaJson = null;

    try {
      respostaJson = respostaTexto ? JSON.parse(respostaTexto) : null;
    } catch {
      respostaJson = null;
    }

    if (!response.ok) {
      throw new Error(
        respostaJson?.mensagem ||
        respostaTexto ||
        "Erro ao encerrar seleção do PN."
      );
    }

    return respostaJson;
  }
 
  // ==========================================================
  // 45. EVENTOS - CAMPOS DE CONTATO
  // ==========================================================
  if (nomeContatoInput) {
    nomeContatoInput.addEventListener("focus", () => {
      if (contatoOriginalTravado) return;

      const valor = nomeContatoInput.value.trim().toLowerCase();

      if (valor === "não contatado" || valor === "nao contatado") {
        nomeContatoInput.value = "";
        nomeContatoInput.classList.remove("input-placeholder");
      }
    });

    nomeContatoInput.addEventListener("blur", () => {
      if (contatoOriginalTravado) return;

      if (!nomeContatoInput.value.trim()) {
        nomeContatoInput.value = "Não Contatado";
        nomeContatoInput.classList.add("input-placeholder");
      }

      validarRegraContato();
    });

    nomeContatoInput.addEventListener("input", () => {
      if (contatoOriginalTravado) return;

      nomeContatoInput.classList.remove("input-placeholder");
      validarRegraContato();
    });
  }

  if (dataContatoInput) {
    dataContatoInput.addEventListener("change", () => {
      if (contatoOriginalTravado) return;
      validarRegraContato();
    });
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
  // Finalidade:
  // - remover linha
  // - processar PN digitado
  // - validar principal / lote / quantidades
  // - abrir e fechar o detalhe operacional do PN
  // ==========================================================
  if (addPnRowBtn) {
    addPnRowBtn.addEventListener("click", () => {
      if (!pnTableBody) return;

      pnTableBody.appendChild(criarLinhaPn({ principal: false }));
      garantirPnPrincipal();
      aplicarValidacoesPnEmTela();
      travarPrimeiroPnOriginalSeNecessario();
    });
  }

  if (pnTableBody) {
    pnTableBody.addEventListener("click", (event) => {
      const alvo = event.target;

      // ------------------------------------------------------
      // 48.1 ABRIR / FECHAR DETALHES DO PN
      // ------------------------------------------------------
      const botaoDetalhes = alvo.closest(".btn-detalhes-linha");
      if (botaoDetalhes) {
        toggleDetalhePnLinha(botaoDetalhes);
        return;
      }

      // ------------------------------------------------------
      // 48.2 REMOVER LINHA
      // ------------------------------------------------------
      if (!alvo.classList.contains("remove-row")) {
        return;
      }

      const linha = alvo.closest(".pn-row");
      if (!linha) return;

      const valorPn = normalizarNumero(
        linha.querySelector(".pn-input")?.value || ""
      );

      if (primeiroPnOriginal && valorPn === primeiroPnOriginal) {
        alert("O PN original da criação não pode ser removido.");
        return;
      }

      const rows = obterRowsPn();

      if (rows.length === 1) {
        alert("O RASP precisa ter pelo menos 1 PN.");
        return;
      }

      const linhaDetalhe = linha.nextElementSibling;
      const eraPrincipal = linha.querySelector(".pn-principal")?.checked;

      // Remove também a linha de detalhe, se existir
      if (linhaDetalhe && linhaDetalhe.classList.contains("pn-detalhe-row")) {
        linhaDetalhe.remove();
      }

      linha.remove();

      if (eraPrincipal) {
        garantirPnPrincipal();
      }

      aplicarValidacoesPnEmTela();
      travarPrimeiroPnOriginalSeNecessario();
    });

    pnTableBody.addEventListener("input", async (event) => {
      const linha = event.target.closest(".pn-row");
      if (!linha) return;

      const pnInputAtual = linha.querySelector(".pn-input");
      const valorPnAtual = normalizarNumero(pnInputAtual?.value || "");

      // ------------------------------------------------------
      // 48.3 BLOQUEIO DO PN ORIGINAL
      // ------------------------------------------------------
      if (
        event.target.classList.contains("pn-input") &&
        primeiroPnOriginal &&
        valorPnAtual === primeiroPnOriginal &&
        pnInputAtual?.readOnly
      ) {
        pnInputAtual.value = primeiroPnOriginal;
        return;
      }

      normalizarInputsPnDaLinha(linha);

      // ------------------------------------------------------
      // 48.4 PROCESSAMENTO AUTOMÁTICO DO PN
      // ------------------------------------------------------
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
      travarPrimeiroPnOriginalSeNecessario();
    });

    pnTableBody.addEventListener("change", (event) => {
      const linha = event.target.closest(".pn-row");
      if (!linha) return;

      const pnInput = linha.querySelector(".pn-input");
      const valorPn = normalizarNumero(pnInput?.value || "");

      // ------------------------------------------------------
      // 48.5 MANTÉM O PN ORIGINAL COMO PRINCIPAL/BLOQUEADO
      // ------------------------------------------------------
      if (
        event.target.classList.contains("pn-principal") &&
        primeiroPnOriginal &&
        valorPn === primeiroPnOriginal
      ) {
        event.target.checked = true;
      }

      // ------------------------------------------------------
      // 48.6 REVALIDAÇÃO DOS CAMPOS DA LINHA
      // ------------------------------------------------------
      if (
        event.target.classList.contains("pn-principal") ||
        event.target.classList.contains("data-lote-inicial") ||
        event.target.classList.contains("qtd-suspeita") ||
        event.target.classList.contains("qtd-checada") ||
        event.target.classList.contains("qtd-rejeitada")
      ) {
        aplicarValidacoesPnEmTela();
        travarPrimeiroPnOriginalSeNecessario();
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
      travarPrimeiroPnOriginalSeNecessario();

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
      if (bpOriginalTravado) return;
      controlarTipoBp();
      validarBpEmTela();
    });
  }

  if (dataBp) {
    dataBp.addEventListener("change", () => {
      if (bpOriginalTravado) return;
      validarBpEmTela();
    });
  }

  if (horaBp) {
    horaBp.addEventListener("change", () => {
      if (bpOriginalTravado) return;
      validarBpEmTela();
    });
  }

  if (vinBp) {
    vinBp.addEventListener("input", () => {
      if (bpOriginalTravado) return;
      vinBp.value = normalizarVin(vinBp.value);
      validarBpEmTela();
    });
  }

  if (localCelulaBp) {
    localCelulaBp.addEventListener("input", () => {
      if (bpOriginalTravado) return;
      validarBpEmTela();
    });
  }

  if (comoIdentificadoBp) {
    comoIdentificadoBp.addEventListener("input", () => {
      if (bpOriginalTravado) return;
      validarBpEmTela();
    });
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
      restaurarBotaoCriarRasp();
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
  aplicarRegraIniciativaFornecedor();
  await atualizarAprovadorFtPorTurno();

  // ========================================================
  // GARANTE 1 LINHA INICIAL NA SEÇÃO 4
  // ========================================================
  if (pnTableBody && pnTableBody.children.length === 0) {
    pnTableBody.appendChild(criarLinhaPn({ principal: true }));
  }

  aplicarValidacoesPnEmTela();
  aplicarTravaContato();
  aplicarTravaBp();
  controlarTipoBp();
  validarBpEmTela();
  aplicarRegraIniciativaFornecedor();

  definirStatusDuns(
    "Informe um DUNS válido para localizar o fornecedor.",
    "neutral"
  );
}


  inicializarTelaRasp();
});

 /* =========================================================
   FUNÇÃO: ABRIR / FECHAR DETALHE DO PN
   Regra:
   - abre somente a linha de detalhe ligada ao botão clicado
   - fecha as demais
   - QHD nasce travado no front
   ========================================================= */

// ==========================================================
// ETAPA 2 - DETALHE RECOLHÍVEL POR LINHA DE PN
// ==========================================================
function criarLinhaDetalhePn() {
  const tr = document.createElement("tr");
  tr.className = "pn-detalhe-row";

  tr.innerHTML = `
    <td colspan="10" class="pn-detalhe-cell">
      <div class="pn-detalhe-wrap">
        <div class="pn-detalhe-header">
          <h4>Controle operacional do PN</h4>
          <span>Seleção, trava e QHD no nível do item</span>
        </div>

        <div class="pn-detalhe-grid">
          <div class="pn-card-mini">
            <h5>Seleção</h5>

            <div class="campo">
              <label>Entrou em seleção</label>
              <div class="toggle-line">
                <input type="checkbox" class="pn-entrou-selecao" />
                <span>Ativar seleção</span>
              </div>
            </div>

            <div class="campo">
              <label>Status</label>
              <div class="input-readonly pn-status-selecao-texto">Fora da seleção</div>
            </div>

            <div class="campo">
              <label>Entrada em seleção</label>
              <div class="input-readonly pn-data-entrada-selecao">-</div>
            </div>

            <div class="campo">
              <label>Saída da seleção</label>
              <div class="input-readonly pn-data-saida-selecao">-</div>
            </div>
          </div>

          <div class="pn-card-mini">
            <h5>Trava</h5>

            <div class="campo">
              <label>Trava ativa</label>
              <div class="toggle-line">
                <input type="checkbox" class="pn-trava-ativa" />
                <span>Ativar trava</span>
              </div>
            </div>

            <div class="campo">
              <label>Solicitação da trava</label>
              <div class="input-readonly pn-data-solicitacao-trava">-</div>
            </div>

            <div class="campo">
              <label>Remoção da trava</label>
              <div class="input-readonly pn-data-remocao-trava">-</div>
            </div>
          </div>

          <div class="pn-card-mini">
            <h5>QHD</h5>

            <div class="campo">
              <label>QHD ativo</label>
              <div class="toggle-line">
                <input
                  type="checkbox"
                  class="pn-qhd-ativo"
                  disabled
                  title="Somente FT, LG ou ADMIN podem ativar o QHD"
                />
                <span>Ativar QHD</span>
              </div>
            </div>

            <div class="campo">
              <label>Data/hora QHD</label>
              <div class="input-readonly pn-datahora-qhd">-</div>
            </div>
          </div>
        </div>

        <div class="pn-alerta-mini">
          A seleção pode ser acionada antes da aprovação do RASP. Após a saída,
          o sistema permitirá ajustes por até 1 hora.
        </div>
      </div>
    </td>
  `;

  return tr;
}

function formatarDataHoraDetalhe(valor) {
  if (!valor) return "-";

  const data = new Date(valor);

  if (Number.isNaN(data.getTime())) return "-";

  return data.toLocaleString("pt-BR");
}

function aplicarEstadoVisualSelecaoNaLinha(linhaPrincipal, dados = {}) {
  if (!linhaPrincipal) return;

  const badgeSelecao = linhaPrincipal.querySelector(".pn-badge");
  const botaoDetalhes = linhaPrincipal.querySelector(".btn-detalhes-linha");
  const linhaDetalhe = linhaPrincipal.nextElementSibling;

  const statusSelecao = Number(dados.statusSelecao ?? 0);
  const travaAtiva = !!dados.travaAtiva;
  const entrouSelecao = !!dados.entrouSelecao;

  if (badgeSelecao) {
    badgeSelecao.classList.remove("pn-badge-neutro", "pn-badge-selecao");

    if (statusSelecao === 1 || entrouSelecao) {
      badgeSelecao.classList.add("pn-badge-selecao");
      badgeSelecao.textContent = "Em seleção";
    } else if (statusSelecao === 2) {
      badgeSelecao.classList.add("pn-badge-neutro");
      badgeSelecao.textContent = "Encerrada";
    } else {
      badgeSelecao.classList.add("pn-badge-neutro");
      badgeSelecao.textContent = "Fora";
    }
  }

  if (!linhaDetalhe || !linhaDetalhe.classList.contains("pn-detalhe-row")) return;

  const chkSelecao = linhaDetalhe.querySelector(".pn-entrou-selecao");
  const chkTrava = linhaDetalhe.querySelector(".pn-trava-ativa");
  const txtStatus = linhaDetalhe.querySelector(".pn-status-selecao-texto");
  const txtEntrada = linhaDetalhe.querySelector(".pn-data-entrada-selecao");
  const txtSaida = linhaDetalhe.querySelector(".pn-data-saida-selecao");
  const txtSolicTrava = linhaDetalhe.querySelector(".pn-data-solicitacao-trava");
  const txtRemTrava = linhaDetalhe.querySelector(".pn-data-remocao-trava");
  const chkQhd = linhaDetalhe.querySelector(".pn-qhd-ativo");
  const txtQhd = linhaDetalhe.querySelector(".pn-datahora-qhd");

  if (chkSelecao) {
    chkSelecao.checked = statusSelecao === 1 || entrouSelecao;
    chkSelecao.disabled = statusSelecao === 2;
  }

  if (chkTrava) {
    chkTrava.checked = travaAtiva;
    chkTrava.disabled = statusSelecao === 2;
  }

  if (txtStatus) {
    if (statusSelecao === 1 || entrouSelecao) {
      txtStatus.textContent = "Em seleção";
    } else if (statusSelecao === 2) {
      txtStatus.textContent = "Seleção encerrada";
    } else {
      txtStatus.textContent = "Fora da seleção";
    }
  }

  if (txtEntrada) {
    txtEntrada.textContent = formatarDataHoraDetalhe(dados.datahoraEntradaSelecao);
  }

  if (txtSaida) {
    txtSaida.textContent = formatarDataHoraDetalhe(dados.datahoraSaidaSelecao);
  }

  if (txtSolicTrava) {
    txtSolicTrava.textContent = formatarDataHoraDetalhe(dados.datahoraSolicitacaoTrava);
  }

  if (txtRemTrava) {
    txtRemTrava.textContent = formatarDataHoraDetalhe(dados.datahoraRemocaoTrava);
  }

  if (chkQhd) {
    chkQhd.checked = !!dados.qhdAtivo;
  }

  if (txtQhd) {
    txtQhd.textContent = formatarDataHoraDetalhe(dados.datahoraQhd);
  }

  if (botaoDetalhes && statusSelecao === 2) {
    botaoDetalhes.title = "Seleção encerrada para este PN";
  }
}


function obterDataHoraAtualFormatada() {
  const agora = new Date();

  return agora.toLocaleString("pt-BR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit"
  });
}

function vincularEventosDetalhePn(linhaDetalhe, linhaPrincipal) {
  if (!linhaDetalhe || !linhaPrincipal) return;

  const chkSelecao = linhaDetalhe.querySelector(".pn-entrou-selecao");
  const statusSelecao = linhaDetalhe.querySelector(".pn-status-selecao-texto");
  const dataEntradaSelecao = linhaDetalhe.querySelector(".pn-data-entrada-selecao");
  const dataSaidaSelecao = linhaDetalhe.querySelector(".pn-data-saida-selecao");

  const chkTrava = linhaDetalhe.querySelector(".pn-trava-ativa");
  const dataSolicitacaoTrava = linhaDetalhe.querySelector(".pn-data-solicitacao-trava");
  const dataRemocaoTrava = linhaDetalhe.querySelector(".pn-data-remocao-trava");

  const badgeSelecaoLinha = linhaPrincipal.querySelector(".pn-badge");

  if (chkSelecao) {
    chkSelecao.addEventListener("change", () => {
      const dataHoraAtual = obterDataHoraAtualFormatada();

      if (chkSelecao.checked) {
        if (statusSelecao) statusSelecao.textContent = "Em seleção";
        if (dataEntradaSelecao) dataEntradaSelecao.textContent = dataHoraAtual;
        if (dataSaidaSelecao) dataSaidaSelecao.textContent = "-";

        if (badgeSelecaoLinha) {
          badgeSelecaoLinha.textContent = "Em seleção";
          badgeSelecaoLinha.classList.remove("pn-badge-neutro");
          badgeSelecaoLinha.classList.add("pn-badge-selecao");
        }
      } else {
        if (statusSelecao) statusSelecao.textContent = "Fora da seleção";
        if (dataSaidaSelecao) dataSaidaSelecao.textContent = dataHoraAtual;

        if (badgeSelecaoLinha) {
          badgeSelecaoLinha.textContent = "Fora";
          badgeSelecaoLinha.classList.remove("pn-badge-selecao");
          badgeSelecaoLinha.classList.add("pn-badge-neutro");
        }
      }
    });
  }

  if (chkTrava) {
    chkTrava.addEventListener("change", () => {
      const dataHoraAtual = obterDataHoraAtualFormatada();

      if (chkTrava.checked) {
        if (dataSolicitacaoTrava) dataSolicitacaoTrava.textContent = dataHoraAtual;
        if (dataRemocaoTrava) dataRemocaoTrava.textContent = "-";
      } else {
        if (dataRemocaoTrava) dataRemocaoTrava.textContent = dataHoraAtual;
      }
    });
  }
}


function fecharTodosDetalhesPn() {
  const detalhes = document.querySelectorAll(".pn-detalhe-row");
  const botoes = document.querySelectorAll(".btn-detalhes-linha");

  detalhes.forEach((linha) => linha.classList.remove("ativo"));
  botoes.forEach((btn) => {
    btn.textContent = "Detalhes";
  });
}

function toggleDetalhePnLinha(botao) {
  const linhaPrincipal = botao.closest(".pn-row");
  if (!linhaPrincipal) return;

  let linhaDetalhe = linhaPrincipal.nextElementSibling;

  if (!linhaDetalhe || !linhaDetalhe.classList.contains("pn-detalhe-row")) {
    linhaDetalhe = criarLinhaDetalhePn();
    linhaPrincipal.insertAdjacentElement("afterend", linhaDetalhe);
    vincularEventosDetalhePn(linhaDetalhe, linhaPrincipal);
  }

  const vaiAbrir = !linhaDetalhe.classList.contains("ativo");

  fecharTodosDetalhesPn();

  if (vaiAbrir) {
    linhaDetalhe.classList.add("ativo");
    botao.textContent = "Fechar";

    const estadoAtual = obterEstadoSelecaoDaLinha(linhaPrincipal);
    aplicarEstadoVisualSelecaoNaLinha(linhaPrincipal, estadoAtual);
  }
}


