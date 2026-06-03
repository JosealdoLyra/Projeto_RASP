/* =====================================================
   IMPRESSÃO RASP
   ===================================================== */

function abrirImpressaoRasp(idRasp) {

    if (!idRasp) {
        alert("ID do RASP não informado.");
        return;
    }

    window.open(
        `/imprimir_rasp.html?id=${idRasp}`,
        "_blank"
    );
}
