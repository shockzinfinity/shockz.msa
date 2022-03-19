using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shockz.msa.ordering.infrastructure.Migrations
{
  public partial class InitialCreate3 : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<string>(
          name: "LastModifiedBy",
          table: "Orders",
          type: "nvarchar(max)",
          nullable: true,
          oldClrType: typeof(string),
          oldType: "nvarchar(max)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.AlterColumn<string>(
          name: "LastModifiedBy",
          table: "Orders",
          type: "nvarchar(max)",
          nullable: false,
          defaultValue: "",
          oldClrType: typeof(string),
          oldType: "nvarchar(max)",
          oldNullable: true);
    }
  }
}
