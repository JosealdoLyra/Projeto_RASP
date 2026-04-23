using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rasp.Api.Migrations
{
    public partial class AddResumoOcorrenciaRasp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "resumo_ocorrencia",
                table: "rasp",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "resumo_ocorrencia",
                table: "rasp");
        }
    }
}
