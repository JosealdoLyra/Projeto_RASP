// =========================================================
// 01. CONFIGURAÇÕES DA SESSÃO
// =========================================================
const CHAVE_SESSAO_RASP = "raspUsuarioLogado";

const TEMPO_LIMITE_INATIVIDADE = 15 * 60 * 1000; // 30 minutos

const MODO_DESENVOLVIMENTO = true;


// =========================================================
// 02. VERIFICAR SE EXISTE USUÁRIO LOGADO
// =========================================================
function obterUsuarioLogado() {
    const dados = sessionStorage.getItem(CHAVE_SESSAO_RASP);

    if (!dados) {
        return null;
    }

    try {
        return JSON.parse(dados);
    } catch {
        return null;
    }
}


// =========================================================
// 03. PROTEGER PÁGINA
// =========================================================
function protegerPagina() {

    if (MODO_DESENVOLVIMENTO) {
        return;
    }
    const usuario = obterUsuarioLogado();

    if (!usuario) {
        window.location.href = "./login.html";
        return;
    }

    iniciarControleInatividade();
}


// =========================================================
// 04. LOGOUT
// =========================================================
function sairDoSistema() {
    sessionStorage.removeItem(CHAVE_SESSAO_RASP);

    window.location.href = "./login.html";
}


// =========================================================
// 05. CONTROLE DE INATIVIDADE
// =========================================================
function iniciarControleInatividade() {
    let temporizador;

    function reiniciarTemporizador() {
        clearTimeout(temporizador);

        temporizador = setTimeout(() => {
            sessionStorage.removeItem(CHAVE_SESSAO_RASP);

            sessionStorage.setItem(
            "rasp_mensagem_logout",
            "Sessão expirada por inatividade."
            );

        window.location.href = "./login.html";

        }, TEMPO_LIMITE_INATIVIDADE);
    }

    ["click", "mousemove", "keydown", "scroll", "touchstart"].forEach((evento) => {
        document.addEventListener(evento, reiniciarTemporizador);
    });

    reiniciarTemporizador();
}
/* =========================================================
   OBTER USUÁRIO LOGADO
   ========================================================= */
function obterUsuarioLogado() {
    const usuario = localStorage.getItem("usuarioLogado");

    if (!usuario) {
        return null;
    }

    return JSON.parse(usuario);
}

// =========================================================
// 06. PREENCHER USUÁRIO LOGADO NAS TELAS
// =========================================================
function preencherUsuarioLogadoNaTela() {
    const nomeUsuario = document.getElementById("nomeUsuarioLogado");
    const perfilUsuario = document.getElementById("perfilUsuarioLogado");
    const avatarUsuario = document.getElementById("avatarUsuario");

    if (!nomeUsuario || !perfilUsuario || !avatarUsuario) {
        return;
    }

    let nome = "";
    let perfil = "";

    const todasChaves = [];

    for (let i = 0; i < sessionStorage.length; i++) {
        todasChaves.push(sessionStorage.key(i));
    }

    for (let i = 0; i < localStorage.length; i++) {
        todasChaves.push(localStorage.key(i));
    }

    todasChaves.forEach(chave => {
        const valor =
            sessionStorage.getItem(chave) ||
            localStorage.getItem(chave);

        if (!valor) return;

        try {
            const obj = JSON.parse(valor);

            nome =
                nome ||
                obj.nomeCompleto ||
                obj.nome ||
                obj.login ||
                obj.usuario ||
                "";

            perfil =
            perfil ||
            obj.nomePerfil ||
            obj.perfil ||
            obj.tipoUsuario ||
            "";

        } catch {
            if (chave.toLowerCase().includes("nome")) {
                nome = nome || valor;
            }

            if (
                chave.toLowerCase().includes("perfil") ||
                chave.toLowerCase().includes("tipo")
            ) {
                perfil = perfil || valor;
            }
        }
    });

    nomeUsuario.textContent = nome || "Usuário";
    perfilUsuario.textContent = perfil || "Perfil não identificado";

    avatarUsuario.textContent =
        (nome || "US")
            .split(" ")
            .filter(Boolean)
            .map(p => p[0])
            .join("")
            .substring(0, 2)
            .toUpperCase();
}


