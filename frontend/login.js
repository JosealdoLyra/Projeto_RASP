// =========================================================
// 01. INICIALIZAÇÃO DA TELA DE LOGIN
// =========================================================
document.addEventListener("DOMContentLoaded", () => {

  // =======================================================
  // 02. CAPTURA DOS ELEMENTOS DA TELA
  // =======================================================
  const formLogin = document.getElementById("formLogin");
  const usuario = document.getElementById("usuario");
  const senha = document.getElementById("senha");
  const toggleSenha = document.getElementById("toggleSenha");

  // =======================================================
  // 03. MOSTRAR / OCULTAR SENHA
  // =======================================================
  toggleSenha.addEventListener("click", () => {

    const senhaVisivel = senha.type === "text";

    senha.type = senhaVisivel
      ? "password"
      : "text";

    toggleSenha.textContent = senhaVisivel
      ? "👁"
      : "🙈";

  });

  // =======================================================
  // 04. VALIDAÇÃO VISUAL DO LOGIN
  // =======================================================
  formLogin.addEventListener("submit", (event) => {

    event.preventDefault();

    const usuarioDigitado = usuario.value.trim();
    const senhaDigitada = senha.value.trim();

    if (!usuarioDigitado || !senhaDigitada) {
      alert("Informe usuário e senha.");
      return;
    }

    alert("Login visual validado. Backend será conectado na próxima etapa.");

  });

});
