document.addEventListener("DOMContentLoaded", () => {
  // ==========================================================
  // REFERÊNCIAS PRINCIPAIS DO DOM
  // ==========================================================
  const form = document.getElementById("raspForm");

  // Seção 2 - Dados básicos do RASP
  const dunsInput = document.getElementById("duns");
  const nomeFornecedorInput = document.getElementById("nomeFornecedor");
  const tipoFornecedorInput = document.getElementById("tipoFornecedor");
  const analistaInput = document.getElementById("analista");
  const statusInicialInput = document.getElementById("statusInicial");
  const dunsStatus = document.getElementById("dunsStatus");

  
  // Seção 4 - PN
  const addPnRowBtn = document.getElementById("addPnRow");
  const pnTableBody = document.getElementById("pnTableBody");
  const btnLimpar = document.getElementById("btnLimpar");
  const toggleMassPanelBtn = document.getElementById("toggleMassPanel");
  const massPanel = document.getElementById("massPanel");
  const importMassPnsBtn = document.getElementById("importMassPns");
  const pnsLoteTextarea = document.getElementById("pnsLoteTextarea");
  const pnStatus = document.getElementById("pnStatus");

  // ==========================================================
  // SIMULAÇÃO DE USUÁRIO LOGADO
  // ==========================================================
  const usuarioLogado = "Usuário Logado";

  // ==========================================================
  // BASE TEMPORÁRIA DE FORNECEDORES
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
  // INICIALIZAÇÃO
  // ==========================================================
  function inicializarCamposFixos() {
    analistaInput.value = usuarioLogado;
    statusInicialInput.value = "Em análise";
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

  function limparFornecedorDerivado() {
    nomeFornecedorInput.value = "";
    tipoFornecedorInput.value = "";
  }

  function definirStatusDuns(mensagem, tipo) {
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
  function processarDuns() {
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

    const fornecedor = obterFornecedorPorDuns(duns);

    if (!fornecedor) {
      definirStatusDuns(
        "DUNS não encontrado. No fluxo final, o sistema deverá oferecer a opção de cadastrar o fornecedor sem perder os dados já preenchidos.",
        "error"
      );
      return { ok: false, motivo: "nao_encontrado" };
    }

    nomeFornecedorInput.value = fornecedor.nome;
    tipoFornecedorInput.value = fornecedor.tipoFornecedor;

    definirStatusDuns(
      `Fornecedor localizado com sucesso: ${fornecedor.nome} (${fornecedor.tipoFornecedor}).`,
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
  dunsInput.addEventListener("input", () => {
    const valorNormalizado = normalizarNumero(dunsInput.value).slice(0, 9);
    dunsInput.value = valorNormalizado;

    if (valorNormalizado.length === 9) {
      processarDuns();
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

  dunsInput.addEventListener("blur", () => {
    processarDuns();
  });

  // ==========================================================
  // OBTÉM TODAS AS LINHAS DE PN DA TABELA
  // ==========================================================
  function obterRowsPn() {
    return [...document.querySelectorAll(".pn-row")];
  }

  // ==========================================================
  // GARANTE QUE SEMPRE EXISTA UM PN PRINCIPAL
  // ==========================================================
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
      </td>
      <td>
        <input
          type="text"
          class="data-lote-inicial"
          placeholder="Ex.: 12/03/2026 ou lote ABC123"
          value="${dataLoteInicial}"
        />
      </td>
      <td>
        <input
          type="number"
          class="qtd-suspeita"
          min="0"
          step="1"
          value="${qtdSuspeita}"
        />
      </td>
      <td>
        <input
          type="number"
          class="qtd-checada"
          min="0"
          step="1"
          value="${qtdChecada}"
        />
      </td>
      <td>
        <input
          type="number"
          class="qtd-rejeitada"
          min="0"
          step="1"
          value="${qtdRejeitada}"
        />
      </td>
      <td>
        <button type="button" class="danger-btn remove-row">Remover</button>
      </td>
    `;

    return tr;
  }

  // ==========================================================
  // COLETA TODOS OS PNs DIGITADOS
  // ==========================================================
  function coletarPns() {
    const rows = obterRowsPn();

    return rows
      .map((row) => {
        const principal = row.querySelector(".pn-principal").checked;
        const pn = normalizarNumero(row.querySelector(".pn-input").value);
        const dataLoteInicial = row.querySelector(".data-lote-inicial").value.trim();
        const qtdSuspeitaInicial = Number(row.querySelector(".qtd-suspeita").value || 0);
        const qtdChecadaInicial = Number(row.querySelector(".qtd-checada").value || 0);
        const qtdRejeitadaInicial = Number(row.querySelector(".qtd-rejeitada").value || 0);

        return {
          principal,
          pn,
          dataLoteInicial,
          qtdSuspeitaInicial,
          qtdChecadaInicial,
          qtdRejeitadaInicial
        };
      })
      .filter((item) => item.pn !== "");
  }

  // ==========================================================
  // REINICIA A TABELA DE PN
  // ==========================================================
  function limparTabelaPn() {
    pnTableBody.innerHTML = "";
    pnTableBody.appendChild(criarLinhaPn({ principal: true }));
    aplicarValidacoesPnEmTela();
  }

  // ==========================================================
  // ESTILO DE ERRO VISUAL NOS CAMPOS
  // ==========================================================
  function destacarErroCampo(input, comErro) {
    if (comErro) {
      input.style.borderColor = "#b02a37";
      input.style.backgroundColor = "#fff5f5";
    } else {
      input.style.borderColor = "";
      input.style.backgroundColor = "";
    }
  }

  // ==========================================================
  // VALIDAÇÕES EM TEMPO REAL DOS PNs
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

    pns.forEach((item) => {
      if (!validarPn(item.pn)) {
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
        "Existe PN incompleto ou inválido. Cada PN deve conter 8 dígitos numéricos.",
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

    definirStatusPn(
      "PN(s) preenchido(s) corretamente.",
      "success"
    );
  }


  // ==========================================================
  // EVENTO: ADICIONAR LINHA DE PN
  // ==========================================================
  addPnRowBtn.addEventListener("click", () => {
    pnTableBody.appendChild(criarLinhaPn());
    garantirPnPrincipal();
    aplicarValidacoesPnEmTela();
  });

  // ==========================================================
  // EVENTO: REMOVER LINHA DE PN
  // ==========================================================
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

  // ==========================================================
  // EVENTOS EM TEMPO REAL DA TABELA DE PN
  // ==========================================================
  pnTableBody.addEventListener("input", (event) => {
    const linha = event.target.closest(".pn-row");
    if (!linha) return;

    normalizarInputsPnDaLinha(linha);
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

  // ==========================================================
  // EVENTO: ABRIR / FECHAR IMPORTAÇÃO EM MASSA
  // ==========================================================
  toggleMassPanelBtn.addEventListener("click", () => {
    massPanel.classList.toggle("show");
  });

  // ==========================================================
  // EVENTO: IMPORTAÇÃO DE PNs EM MASSA
  // ==========================================================
  importMassPnsBtn.addEventListener("click", () => {
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

  // ==========================================================
  // EVENTO: LIMPAR FORMULÁRIO
  // ==========================================================
  btnLimpar.addEventListener("click", () => {
    form.reset();
    analistaInput.value = usuarioLogado;
    statusInicialInput.value = "Em análise";

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
    massPanel.classList.remove("show");
    pnsLoteTextarea.value = "";
  });

  // ==========================================================
  // EVENTO: SUBMISSÃO
  // ==========================================================
  form.addEventListener("submit", (event) => {
    event.preventDefault();

    const resultadoDuns = processarDuns();
    const duns = normalizarNumero(dunsInput.value);
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
      dunsInput.focus();
      return;
    }

    const resumo = document.getElementById("resumo").value.trim();
    const descricaoInicial = document.getElementById("descricaoInicial").value.trim();
    const fornecedor = nomeFornecedorInput.value.trim();
    const tipoFornecedor = tipoFornecedorInput.value.trim();
    const analista = analistaInput.value.trim();
    const statusInicial = statusInicialInput.value.trim();
    const setor = document.getElementById("setor").value;
    const origem = document.getElementById("origem").value;
    const maiorImpacto = document.getElementById("maiorImpacto").value;
    const impactoQualidade = document.getElementById("impactoQualidade").value;
    const impactoCliente = document.getElementById("impactoCliente").value;

    const pns = coletarPns();
    const erros = [];

    if (!resumo) erros.push("Preencha o Resumo da ocorrência.");
    if (!descricaoInicial) erros.push("Preencha a Descrição inicial.");
    if (!fornecedor) erros.push("O fornecedor deve ser carregado automaticamente pelo DUNS.");
    if (!tipoFornecedor) erros.push("O tipo do fornecedor deve ser carregado automaticamente pelo DUNS.");
    if (!analista) erros.push("O campo Analista deve vir preenchido pelo login.");
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

        if (pnsUnicos.has(item.pn)) {
          erros.push(`O PN ${item.pn} está repetido.`);
        } else {
          pnsUnicos.add(item.pn);
        }

        if (!item.dataLoteInicial && item.principal) {
          erros.push(`Preencha a Data/Lote inicial do PN principal na linha ${index + 1}.`);
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
      rasp: {
        resumo,
        descricaoInicial,
        fornecedor,
        duns,
        tipoFornecedor,
        analista,
        statusInicial,
        setor,
        origem,
        maiorImpacto,
        impactoQualidade,
        impactoCliente
      },
      pnsAtendimentoInicial: pns,
      contencoes: []
    };

    console.log("Payload estruturado do RASP:");
    console.log(payload);
    alert("Estrutura inicial do RASP validada com sucesso! (simulação front-end)");
  });

  inicializarCamposFixos();
  aplicarValidacoesPnEmTela();
});
