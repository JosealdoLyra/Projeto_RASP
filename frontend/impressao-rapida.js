// =========================================================
// IMPRESSÃO RÁPIDA RASP
// =========================================================

const API_RASP = "http://localhost:5050";

function abrirModalImpressaoRasp() {
  const modal = document.createElement("div");

  modal.innerHTML = `
    <div id="modalImpressaoRasp" style="
      position:fixed;
      inset:0;
      background:rgba(0,0,0,.65);
      display:flex;
      align-items:center;
      justify-content:center;
      z-index:9999;
      font-family:Segoe UI, Arial, sans-serif;
    ">
      <div style="
        width:420px;
        background:#061b3a;
        border:1px solid #2f94ff;
        border-radius:14px;
        box-shadow:0 0 35px rgba(47,148,255,.45);
        padding:24px;
        color:white;
      ">
        <h2 style="margin-bottom:8px;color:#5bb6ff;">Impressão RASP</h2>

        <p style="font-size:14px;color:#c8d6ee;margin-bottom:18px;">
          Informe o número do RASP para abrir a impressão.
        </p>

        <input id="numeroRaspImpressao" type="text" placeholder="Ex: 0078 ou 0078/26"
        oninput="formatarNumeroRasp(this)"
        maxlength="7"

          style="
            width:100%;
            height:44px;
            border-radius:8px;
            border:1px solid #2f94ff;
            background:#020817;
            color:white;
            padding:0 12px;
            font-size:16px;
            outline:none;
          " />

        <div style="
          display:flex;
          justify-content:flex-end;
          gap:12px;
          margin-top:22px;
        ">
          <button onclick="fecharModalImpressaoRasp()" style="
            height:38px;
            padding:0 18px;
            border-radius:8px;
            border:1px solid #91a4c2;
            background:transparent;
            color:#c8d6ee;
            cursor:pointer;
            font-weight:700;
          ">Cancelar</button>

          <button onclick="confirmarImpressaoRasp()" style="
            height:38px;
            padding:0 22px;
            border-radius:8px;
            border:1px solid #2f94ff;
            background:#0f6dff;
            color:white;
            cursor:pointer;
            font-weight:800;
          ">Abrir</button>
        </div>
      </div>
    </div>
  `;

  document.body.appendChild(modal);

  setTimeout(() => {
    document.getElementById("numeroRaspImpressao").focus();
  }, 100);
}

function fecharModalImpressaoRasp() {
  const modal = document.getElementById("modalImpressaoRasp");
  if (modal) {
    modal.remove();
  }
}

async function confirmarImpressaoRasp() {
  const campo = document.getElementById("numeroRaspImpressao");
  const numeroRasp = campo.value.trim();

  if (!numeroRasp) {
    alert("Informe o número do RASP.");
    campo.focus();
    return;
  }

  try {
    const resposta = await fetch(
      `${API_RASP}/rasp/numero/${encodeURIComponent(numeroRasp)}`
    );

    if (!resposta.ok) {
      alert("RASP não encontrado.");
      campo.focus();
      return;
    }

    const rasp = await resposta.json();

    window.location.href =
      `${API_RASP}/imprimir_rasp.html?id=${rasp.idRasp}`;

  } catch (erro) {
    alert("Erro ao consultar o RASP.");
  }
}
function formatarNumeroRasp(campo) {
  let valor = campo.value.replace(/\D/g, "");

  if (valor.length > 4) {
    valor = valor.substring(0, 4) + "/" + valor.substring(4, 6);
  }

  campo.value = valor;
}

