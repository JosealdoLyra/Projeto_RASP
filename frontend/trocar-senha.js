// =========================================================
// 01. CONFIGURAÇÕES
// =========================================================
const API_BASE_URL = "http://localhost:5050";


// =========================================================
// 02. INICIALIZAÇÃO
// =========================================================
document.addEventListener("DOMContentLoaded", () => {

    // =====================================================
    // 02.01 ELEMENTOS DO FORMULÁRIO
    // =====================================================
    const form = document.getElementById("formTrocarSenha");

    const login = document.getElementById("login");
    const senhaAtual = document.getElementById("senhaAtual");
    const novaSenha = document.getElementById("novaSenha");
    const confirmarNovaSenha = document.getElementById("confirmarNovaSenha");


    // =====================================================
    // 02.02 ELEMENTOS DA MENSAGEM VISUAL
    // =====================================================
    const mensagemSistema = document.getElementById("mensagemSistema");
    const mensagemTexto = document.getElementById("mensagemTexto");
    const mensagemIcone = document.getElementById("mensagemIcone");
    const mensagemOk = document.getElementById("mensagemOk");


    // =====================================================
    // 02.03 FUNÇÃO DE MENSAGEM PADRÃO RASP
    // =====================================================
    function mostrarMensagem(texto, tipo = "info", aoFechar = null) {
        mensagemTexto.textContent = texto;

        mensagemIcone.textContent = tipo === "sucesso"
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


    // =====================================================
    // 02.04 SUBMIT - TROCAR SENHA
    // =====================================================
    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const loginDigitado = login.value.trim();
        const senhaAtualDigitada = senhaAtual.value.trim();
        const novaSenhaDigitada = novaSenha.value.trim();
        const confirmarSenhaDigitada = confirmarNovaSenha.value.trim();


        // =================================================
        // 02.04.01 VALIDAÇÕES
        // =================================================
        if (
            !loginDigitado ||
            !senhaAtualDigitada ||
            !novaSenhaDigitada ||
            !confirmarSenhaDigitada
        ) {
            mostrarMensagem("Preencha todos os campos.", "erro");
            return;
        }

        if (novaSenhaDigitada.length < 8) {
            mostrarMensagem(
                "A nova senha deve possuir no mínimo 8 caracteres.",
                "erro"
            );
            return;
        }

        if (novaSenhaDigitada !== confirmarSenhaDigitada) {
            mostrarMensagem(
                "A confirmação da senha não confere.",
                "erro"
            );
            return;
        }


        // =================================================
        // 02.04.02 CHAMADA API
        // =================================================
        try {
            const resposta = await fetch(
                `${API_BASE_URL}/login/trocar-senha`,
                {
                    method: "POST",

                    headers: {
                        "Content-Type": "application/json"
                    },

                    body: JSON.stringify({
                        login: loginDigitado,
                        senhaAtual: senhaAtualDigitada,
                        novaSenha: novaSenhaDigitada,
                        confirmarNovaSenha: confirmarSenhaDigitada
                    })
                }
            );

            const dados = await resposta.json();

            if (!resposta.ok) {
                mostrarMensagem(
                    dados.mensagem ||
                    dados ||
                    "Não foi possível alterar a senha.",
                    "erro"
                );
                return;
            }

            mostrarMensagem(
                "Senha alterada com sucesso.",
                "sucesso",
                () => {
                    window.location.href = "./login.html";
                }
            );

        } catch (erro) {
            console.error("Erro ao trocar senha:", erro);

            mostrarMensagem(
                "Erro ao conectar com a API.",
                "erro"
            );
        }
    });
});
