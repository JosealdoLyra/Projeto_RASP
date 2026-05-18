// =========================================================
// 01. CONFIGURAÇÕES
// =========================================================
const API_BASE_URL = "http://localhost:5050";


// =========================================================
// 02. INICIALIZAÇÃO
// =========================================================
document.addEventListener("DOMContentLoaded", () => {

    // =====================================================
    // 02.01 ELEMENTOS DA TELA
    // =====================================================
    const formLogin = document.getElementById("formLogin");

    const usuario = document.getElementById("usuario");
    const senha = document.getElementById("senha");

    const toggleSenha = document.getElementById("toggleSenha");

    const lembrarUsuario = document.getElementById("lembrarUsuario");


    // =====================================================
    // 02.02 ELEMENTOS DA MENSAGEM VISUAL
    // =====================================================
    const mensagemSistema = document.getElementById("mensagemSistema");
    const mensagemTexto = document.getElementById("mensagemTexto");
    const mensagemIcone = document.getElementById("mensagemIcone");
    const mensagemOk = document.getElementById("mensagemOk");


    // =====================================================
    // 02.03 MOSTRAR MENSAGEM
    // =====================================================
    function mostrarMensagem(texto, tipo = "info", aoFechar = null) {

        mensagemTexto.textContent = texto;

        mensagemIcone.textContent =
            tipo === "sucesso"
                ? "✓"
                : tipo === "erro"
                    ? "!"
                    : "ℹ";

        mensagemSistema.classList.remove("oculto");

        mensagemOk.onclick = () => {

            mensagemSistema.classList.add("oculto");

            if (typeof aoFechar === "function") {
                aoFechar();
            }
        };
    }

    const mensagemLogout = sessionStorage.getItem("rasp_mensagem_logout");

    if (mensagemLogout) {
        sessionStorage.removeItem("rasp_mensagem_logout");

        mostrarMensagem(
            mensagemLogout,
            "info"
        );
    }


    // =====================================================
    // 02.04 MOSTRAR / OCULTAR SENHA
    // =====================================================
    toggleSenha.addEventListener("click", () => {

        const senhaVisivel = senha.type === "text";

        senha.type = senhaVisivel
            ? "password"
            : "text";

        toggleSenha.textContent = senhaVisivel
            ? "👁"
            : "🙈";
    });


    // =====================================================
    // 02.05 LEMBRAR USUÁRIO
    // =====================================================
    const usuarioSalvo = localStorage.getItem("raspUsuario");

    if (usuarioSalvo) {

        usuario.value = usuarioSalvo;

        if (lembrarUsuario) {
            lembrarUsuario.checked = true;
        }
    }


    // =====================================================
    // 02.06 LOGIN
    // =====================================================
    formLogin.addEventListener("submit", async (event) => {

        event.preventDefault();

        const loginDigitado = usuario.value.trim();
        const senhaDigitada = senha.value.trim();


        // =================================================
        // 02.06.01 VALIDAÇÕES
        // =================================================
        if (!loginDigitado || !senhaDigitada) {

            mostrarMensagem(
                "Informe usuário e senha.",
                "erro"
            );

            return;
        }


        // =================================================
        // 02.06.02 API LOGIN
        // =================================================
        try {

            const resposta = await fetch(
                `${API_BASE_URL}/login`,
                {
                    method: "POST",

                    headers: {
                        "Content-Type": "application/json"
                    },

                    body: JSON.stringify({
                        login: loginDigitado,
                        senha: senhaDigitada
                    })
                }
            );

            const dados = await resposta.json();


            // =============================================
            // LOGIN INVÁLIDO
            // =============================================
            if (!resposta.ok || !dados.sucesso) {

                mostrarMensagem(
                    dados.mensagem ||
                    "Usuário ou senha inválidos.",
                    "erro"
                );

                return;
            }


            // =============================================
            // LEMBRAR USUÁRIO
            // =============================================
            if (lembrarUsuario?.checked) {

                localStorage.setItem(
                    "raspUsuario",
                    loginDigitado
                );

            } else {

                localStorage.removeItem(
                    "raspUsuario"
                );
            }


            // =============================================
            // SALVAR SESSÃO
            // =============================================
            sessionStorage.setItem(
                "raspUsuarioLogado",
                JSON.stringify(dados)
            );


            // =============================================
            // PRIMEIRO ACESSO
            // =============================================
            if (dados.primeiroAcesso) {

                mostrarMensagem(
                    "Primeiro acesso detectado. Cadastre uma nova senha.",
                    "info",
                    () => {
                        window.location.href =
                            "./trocar-senha.html";
                    }
                );

                return;
            }


            // =============================================
            // LOGIN OK
            // =============================================
            window.location.href = "./loading.html";

        } catch (erro) {

            console.error(
                "Erro ao realizar login:",
                erro
            );

            mostrarMensagem(
                "Erro ao conectar com a API.",
                "erro"
            );
        }
    });
});
