using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantSystem.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddAuditLogsIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS idx_audit_logs_table_name ON audit_logs(table_name);");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS idx_audit_logs_record_id ON audit_logs(record_id);");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS idx_audit_logs_created_at ON audit_logs(created_at);");
        migrationBuilder.Sql("CREATE INDEX IF NOT EXISTS idx_audit_logs_table_record ON audit_logs(table_name, record_id);");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX IF EXISTS idx_audit_logs_table_record;");
        migrationBuilder.Sql("DROP INDEX IF EXISTS idx_audit_logs_created_at;");
        migrationBuilder.Sql("DROP INDEX IF EXISTS idx_audit_logs_record_id;");
        migrationBuilder.Sql("DROP INDEX IF EXISTS idx_audit_logs_table_name;");
    }
}
